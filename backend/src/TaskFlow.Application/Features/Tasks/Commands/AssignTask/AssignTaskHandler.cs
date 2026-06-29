using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;

using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Features.Tasks.Commands.AssignTask;

public sealed class AssignTaskHandler
    : IRequestHandler<AssignTaskCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IActivityLogger _activityLogger;

    public AssignTaskHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IActivityLogger activityLogger)
    {
        _context = context;
        _currentUser = currentUser;
        _activityLogger = activityLogger;
    }

    public async Task Handle(
        AssignTaskCommand request,
        CancellationToken cancellationToken)
    {
        TaskItem task = await _context.Tasks
            .Include(x => x.Project)
                .ThenInclude(p => p.Members)
            .Include(x => x.Assignments)
            .FirstOrDefaultAsync(
                x => x.Id == request.TaskId &&
                     (
                         x.Project.OwnerId == _currentUser.UserId ||
                         x.Project.Members.Any(m => m.UserId == _currentUser.UserId)
                     ),
                cancellationToken)
            ?? throw new KeyNotFoundException("Task not found.");

        List<Guid> assigneeIds = request.AssigneeIds
            .Distinct()
            .ToList();

        // Owner + every project member (Admins and Members)
        HashSet<Guid> validAssignees = task.Project.Members
            .Select(m => m.UserId)
            .Append(task.Project.OwnerId)
            .ToHashSet();

        if (assigneeIds.Any(id => !validAssignees.Contains(id)))
        {
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

        await _activityLogger.LogAsync(
            _currentUser.UserId,
            "TaskAssigned",
            "Task",
            task.Id,
            $"Updated assignments for task \"{task.Title}\"",
            cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
