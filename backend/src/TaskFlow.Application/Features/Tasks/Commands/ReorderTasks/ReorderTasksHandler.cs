using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Features.Tasks.Commands.ReorderTasks
{
    public sealed class ReorderTasksHandler
        : IRequestHandler<ReorderTasksCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IProjectAuthorizationService _auth;

        public ReorderTasksHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUser,
            IProjectAuthorizationService auth)
        {
            _context = context;
            _auth = auth;
        }

        public async Task Handle(ReorderTasksCommand request, CancellationToken cancellationToken)
        {
            BoardColumn column = await _context.BoardColumns
                .Include(x => x.Board)
                .FirstOrDefaultAsync(x => x.Id == request.ColumnId, cancellationToken)
                ?? throw new KeyNotFoundException("Column not found.");

            await _auth.EnsureMemberAsync(column.Board.ProjectId, cancellationToken);

            List<TaskItem> tasks = await _context.Tasks
                .Where(x => x.BoardColumnId == request.ColumnId)
                .ToListAsync(cancellationToken);

            Dictionary<Guid, TaskItem> taskMap = tasks.ToDictionary(x => x.Id);

            for (int i = 0; i < request.OrderedTaskIds.Count; i++)
            {
                Guid taskId = request.OrderedTaskIds[i];

                if (!taskMap.TryGetValue(taskId, out TaskItem? task))
                    continue;

                task.Order = i;
            }

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
