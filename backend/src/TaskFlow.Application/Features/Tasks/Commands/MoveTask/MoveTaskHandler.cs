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

            BoardColumn? targetColumn = await _context.BoardColumns
                .Include(x => x.Board)
                .FirstOrDefaultAsync(
                    x => x.Id == request.TargetColumnId,
                    cancellationToken)
                ?? throw new KeyNotFoundException("Target column not found.");

            if (targetColumn.Board.ProjectId != task.ProjectId)
                throw new InvalidOperationException(
                    "Cannot move task across projects.");

            task.BoardColumnId = targetColumn.Id;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
