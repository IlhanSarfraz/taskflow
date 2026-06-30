using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Features.Tasks.Commands.MoveTask
{
    public sealed class MoveTaskHandler
        : IRequestHandler<MoveTaskCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;
        private readonly IActivityLogger _activityLogger;

        public MoveTaskHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUser,
            IActivityLogger activityLogger)
        {
            _context = context;
            _currentUser = currentUser;
            _activityLogger = activityLogger;
        }

        public async Task Handle(
            MoveTaskCommand request,
            CancellationToken cancellationToken)
        {
            TaskItem task = await _context.Tasks
                .Include(x => x.Project)
                .FirstOrDefaultAsync(
                    x => x.Id == request.TaskId &&
                    (
                        x.Project.OwnerId == _currentUser.UserId ||
                        x.Project.Members.Any(m => m.UserId == _currentUser.UserId)
                    ),
                    cancellationToken)
                ?? throw new KeyNotFoundException("Task not found.");

            BoardColumn targetColumn = await _context.BoardColumns
                .Include(x => x.Board)
                .FirstOrDefaultAsync(
                    x => x.Id == request.TargetColumnId,
                    cancellationToken)
                ?? throw new KeyNotFoundException("Target column not found.");

            if (targetColumn.Board.ProjectId != task.ProjectId)
                throw new InvalidOperationException("Cannot move tasks across projects.");

            // OLD COLUMN — remove + reindex
            List<TaskItem> oldColumnTasks = await _context.Tasks
                .Where(x => x.BoardColumnId == task.BoardColumnId)
                .OrderBy(x => x.Order)
                .ToListAsync(cancellationToken);

            oldColumnTasks.RemoveAll(x => x.Id == task.Id);

            for (int i = 0; i < oldColumnTasks.Count; i++)
                oldColumnTasks[i].Order = i;

            // MOVE TASK
            task.BoardColumnId = request.TargetColumnId;

            if (targetColumn.IsDoneColumn)
                task.CompletedAtUtc ??= DateTime.UtcNow;
            else
                task.CompletedAtUtc = null;

            // TARGET COLUMN — add + reindex
            List<TaskItem> targetTasks = await _context.Tasks
                .Where(x => x.BoardColumnId == request.TargetColumnId)
                .OrderBy(x => x.Order)
                .ToListAsync(cancellationToken);

            targetTasks.RemoveAll(x => x.Id == task.Id);
            targetTasks.Add(task);

            for (int i = 0; i < targetTasks.Count; i++)
                targetTasks[i].Order = i;

            await _activityLogger.LogAsync(
                _currentUser.UserId,
                "TaskMoved",
                "Task",
                task.Id,
                $"Moved task \"{task.Title}\" to \"{targetColumn.Name}\"",
                task.ProjectId,
                task.Project.Name,
                targetColumn.BoardId,
                targetColumn.Board.Name,
                cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}