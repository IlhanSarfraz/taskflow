using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Features.Tasks.Commands.UpdateTask
{
    public sealed class UpdateTaskHandler
        : IRequestHandler<UpdateTaskCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;
        private readonly IActivityLogger _activityLogger;

        public UpdateTaskHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUser,
            IActivityLogger activityLogger)
        {
            _context = context;
            _currentUser = currentUser;
            _activityLogger = activityLogger;
        }

        public async Task Handle(
            UpdateTaskCommand request,
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

            task.Title = request.Title;
            task.Description = request.Description;
            task.Priority = request.Priority;
            task.DueDate = request.DueDate;

            await _activityLogger.LogAsync(
                _currentUser.UserId,
                "TaskUpdated",
                "Task",
                task.Id,
                $"Updated task \"{task.Title}\"",
                task.ProjectId,
                task.Project.Name,
                task.BoardColumn.BoardId,
                task.BoardColumn.Board.Name,
                cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
