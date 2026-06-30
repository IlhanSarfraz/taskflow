using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Features.Tasks.Commands.DeleteTask
{
    public sealed class DeleteTaskHandler
        : IRequestHandler<DeleteTaskCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;
        private readonly IActivityLogger _activityLogger;

        public DeleteTaskHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUser,
            IActivityLogger activityLogger)
        {
            _context = context;
            _currentUser = currentUser;
            _activityLogger = activityLogger;
        }

        public async Task Handle(
            DeleteTaskCommand request,
            CancellationToken cancellationToken)
        {
            TaskItem task = await _context.Tasks
                .Include(x => x.Project)
                .Include(x => x.BoardColumn)
                    .ThenInclude(bc => bc.Board)
                .FirstOrDefaultAsync(
                    x => x.Id == request.TaskId &&
                       (
                           x.Project.OwnerId == _currentUser.UserId ||
                           x.Project.Members.Any(m => m.UserId == _currentUser.UserId)
                       ),
                    cancellationToken)
                ?? throw new KeyNotFoundException("Task not found.");

            await _activityLogger.LogAsync(
                _currentUser.UserId,
                "TaskDeleted",
                "Task",
                task.Id,
                $"Deleted task \"{task.Title}\"",
                task.ProjectId,
                task.Project.Name,
                task.BoardColumn.BoardId,
                task.BoardColumn.Board.Name,
                cancellationToken);

            _context.Tasks.Remove(task);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
