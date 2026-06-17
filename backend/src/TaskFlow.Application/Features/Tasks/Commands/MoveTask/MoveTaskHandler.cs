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

        public MoveTaskHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task Handle(
            MoveTaskCommand request,
            CancellationToken cancellationToken)
        {
            TaskItem task = await _context.Tasks
                .Include(x => x.Project)
                .FirstOrDefaultAsync(
                    x => x.Id == request.TaskId &&
                         x.Project.OwnerId == _currentUser.UserId,
                    cancellationToken)
                ?? throw new KeyNotFoundException("Task not found.");

            BoardColumn targetColumn = await _context.BoardColumns
                .FirstOrDefaultAsync(
                    x => x.Id == request.TargetColumnId,
                    cancellationToken)
                ?? throw new KeyNotFoundException("Target column not found.");

            bool sameProject = await _context.BoardColumns
                .AnyAsync(x =>
                    x.Id == request.TargetColumnId &&
                    x.Board.ProjectId == task.ProjectId,
                    cancellationToken);

            if (!sameProject)
                throw new InvalidOperationException("Cannot move across projects.");

            // =========================
            // OLD COLUMN - REMOVE + REINDEX
            // =========================
            List<TaskItem> oldColumnTasks = await _context.Tasks
                .Where(x => x.BoardColumnId == task.BoardColumnId)
                .OrderBy(x => x.Order)
                .ToListAsync(cancellationToken);

            oldColumnTasks.RemoveAll(x => x.Id == task.Id);

            for (int i = 0; i < oldColumnTasks.Count; i++)
                oldColumnTasks[i].Order = i;

            // =========================
            // MOVE TASK
            // =========================
            task.BoardColumnId = request.TargetColumnId;

            // =========================
            // TARGET COLUMN - ADD + REINDEX
            // =========================
            List<TaskItem> targetTasks = await _context.Tasks
                .Where(x => x.BoardColumnId == request.TargetColumnId)
                .OrderBy(x => x.Order)
                .ToListAsync(cancellationToken);

            targetTasks.RemoveAll(x => x.Id == task.Id);
            targetTasks.Add(task);

            for (int i = 0; i < targetTasks.Count; i++)
                targetTasks[i].Order = i;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
