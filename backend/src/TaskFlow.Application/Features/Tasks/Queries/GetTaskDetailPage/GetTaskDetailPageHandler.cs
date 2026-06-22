using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Features.Projects.DTOs;
using TaskFlow.Application.Features.Tasks.Dtos;

namespace TaskFlow.Application.Features.Tasks.Queries.GetTaskDetailPage;

public sealed class GetTaskDetailPageHandler
    : IRequestHandler<GetTaskDetailPageQuery, TaskDetailPageResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetTaskDetailPageHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<TaskDetailPageResponse> Handle(
        GetTaskDetailPageQuery request,
        CancellationToken cancellationToken)
    {
        var taskRow = await _context.Tasks
            .AsNoTracking()
            .Where(x =>
                x.Id == request.TaskId &&
                (
                    x.Project.OwnerId == _currentUser.UserId ||
                    x.Project.Members.Any(m => m.UserId == _currentUser.UserId)
                ))
            .Select(x => new
            {
                x.Id,
                x.Title,
                x.Description,
                x.Priority,
                x.DueDate,
                x.ProjectId,
                ProjectName = x.Project.Name,
                x.BoardColumnId,
                ColumnName = x.BoardColumn.Name,
                BoardId = x.BoardColumn.BoardId,
                Assignees = x.Assignments
                    .OrderBy(a => a.User.FirstName)
                    .Select(a => new TaskAssigneeDto(
                        a.UserId,
                        a.User.FirstName + " " + a.User.LastName,
                        (a.User.FirstName.Substring(0, 1) + a.User.LastName.Substring(0, 1)).ToUpper()))
                    .ToList()
            })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new KeyNotFoundException("Task not found.");

        List<CommentResponse> comments = await _context.TaskComments
            .AsNoTracking()
            .Where(x => x.TaskId == request.TaskId)
            .OrderBy(x => x.CreatedAtUtc)
            .Select(x => new CommentResponse(
                x.Id,
                x.UserId,
                x.User.FirstName + " " + x.User.LastName,
                x.Content,
                x.CreatedAtUtc))
            .ToListAsync(cancellationToken);

        List<ProjectMemberResponse> members = await _context.ProjectMembers
            .AsNoTracking()
            .Where(x => x.ProjectId == taskRow.ProjectId)
            .Select(x => new ProjectMemberResponse(
                x.UserId,
                x.User.FirstName + " " + x.User.LastName,
                x.User.Email,
                x.Role.ToString()))
            .ToListAsync(cancellationToken);

        var project = await _context.Projects
            .AsNoTracking()
            .Include(x => x.Owner)
            .FirstOrDefaultAsync(x => x.Id == taskRow.ProjectId, cancellationToken);

        if (project is not null && members.All(m => m.UserId != project.OwnerId))
        {
            members.Insert(0, new ProjectMemberResponse(
                project.OwnerId,
                project.Owner.FirstName + " " + project.Owner.LastName,
                project.Owner.Email,
                "Owner"));
        }

        List<TaskAssigneeDto> assignees = taskRow.Assignees;

        if (assignees.Count == 0 && project is not null)
        {
            Guid? legacyAssigneeId = await _context.Tasks
                .AsNoTracking()
                .Where(x => x.Id == request.TaskId)
                .Select(x => x.AssigneeId)
                .FirstOrDefaultAsync(cancellationToken);

            if (legacyAssigneeId.HasValue)
            {
                var legacyUser = await _context.Users
                    .AsNoTracking()
                    .Where(u => u.Id == legacyAssigneeId.Value)
                    .Select(u => new TaskAssigneeDto(
                        u.Id,
                        u.FirstName + " " + u.LastName,
                        (u.FirstName.Substring(0, 1) + u.LastName.Substring(0, 1)).ToUpper()))
                    .FirstOrDefaultAsync(cancellationToken);

                if (legacyUser is not null)
                    assignees = [legacyUser];
            }
        }

        TaskDetailsResponse task = new(
            taskRow.Id,
            taskRow.Title,
            taskRow.Description,
            taskRow.Priority,
            taskRow.DueDate,
            taskRow.ProjectId,
            taskRow.ProjectName,
            taskRow.BoardColumnId,
            taskRow.ColumnName,
            taskRow.BoardId,
            assignees);

        return new TaskDetailPageResponse(task, comments, members);
    }
}
