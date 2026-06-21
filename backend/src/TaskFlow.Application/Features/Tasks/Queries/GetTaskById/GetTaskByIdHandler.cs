using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Features.Tasks.Dtos;

namespace TaskFlow.Application.Features.Tasks.Queries.GetTaskById;

public sealed class GetTaskByIdHandler
    : IRequestHandler<GetTaskByIdQuery, TaskDetailsResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetTaskByIdHandler(
        ICurrentUserService currentUser,
        IApplicationDbContext context)
    {
        _currentUser = currentUser;
        _context = context;
    }

    public async Task<TaskDetailsResponse> Handle(
        GetTaskByIdQuery request,
        CancellationToken cancellationToken)
    {
        TaskDetailsResponse? task = await _context.Tasks
            .AsNoTracking()
            .Where(x =>
                x.Id == request.TaskId &&
                x.Project.OwnerId == _currentUser.UserId)
            .Select(x => new TaskDetailsResponse(
                x.Id,
                x.Title,
                x.Description,
                x.Priority,
                x.DueDate,
                x.ProjectId,
                x.Project.Name,
                x.BoardColumnId,
                x.BoardColumn.Name,
                x.BoardColumn.BoardId,
                x.Assignments
                    .OrderBy(a => a.User.FirstName)
                    .Select(a => new TaskAssigneeDto(
                        a.UserId,
                        a.User.FirstName + " " + a.User.LastName,
                        (a.User.FirstName.Substring(0, 1) + a.User.LastName.Substring(0, 1)).ToUpper()))
                    .ToList()))
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new KeyNotFoundException("Task not found.");

        return task;
    }
}
