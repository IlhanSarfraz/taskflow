using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Features.Projects.DTOs;
using TaskFlow.Application.Features.Tasks.Dtos;

using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Features.Tasks.Commands.AssignTask;

public sealed class AssignTaskHandler
    : IRequestHandler<AssignTaskCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public AssignTaskHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(
        AssignTaskCommand request,
        CancellationToken cancellationToken)
    {
        TaskItem? task = await _context.Tasks
            .Include(x => x.Project)
            .Include(x => x.Assignments)
            .FirstOrDefaultAsync(
                x => x.Id == request.TaskId &&
                    x.Project.OwnerId == _currentUser.UserId,
                cancellationToken)
            ?? throw new KeyNotFoundException("Task not found.");

        List<Guid> assigneeIds = request.AssigneeIds
            .Distinct()
            .ToList();

        if (assigneeIds.Count > 0)
        {
            int memberCount = await _context.ProjectMembers
                .CountAsync(x =>
                    x.ProjectId == task.ProjectId &&
                    assigneeIds.Contains(x.UserId),
                    cancellationToken);

            if (memberCount != assigneeIds.Count)
                throw new InvalidOperationException(
                    "One or more users are not members of this project.");
        }

        _context.TaskAssignments.RemoveRange(
            await _context.TaskAssignments
                .Where(x => x.TaskId == task.Id)
                .ToListAsync(cancellationToken));

        task.Assignments.Clear();

        foreach (Guid assigneeId in assigneeIds)
        {
            task.Assignments.Add(new TaskAssignment
            {
                TaskId = task.Id,
                UserId = assigneeId
            });
        }

        task.AssigneeId = assigneeIds.FirstOrDefault();

        await _context.SaveChangesAsync(cancellationToken);
    }
}
