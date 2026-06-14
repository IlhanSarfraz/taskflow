using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Features.Boards.Commands.ReorderColumns
{
    public sealed class ReorderColumnsHandler
        : IRequestHandler<ReorderColumnsCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IProjectAuthorizationService _auth;

        public ReorderColumnsHandler(
            IApplicationDbContext context,
            IProjectAuthorizationService auth)
        {
            _context = context;
            _auth = auth;
        }

        public async Task Handle(
            ReorderColumnsCommand request,
            CancellationToken cancellationToken)
        {
            Board board = await _context.Boards
                .FirstOrDefaultAsync(x => x.Id == request.BoardId, cancellationToken)
                ?? throw new KeyNotFoundException("Board not found");

            await _auth.EnsureMemberAsync(board.ProjectId, cancellationToken);

            List<BoardColumn> columns = await _context.BoardColumns
                .Where(x => x.BoardId == request.BoardId)
                .ToListAsync(cancellationToken);

            // STEP 1: move everything out of conflict range
            foreach (BoardColumn? column in columns)
            {
                column.Order = column.Order + 1000;
            }

            await _context.SaveChangesAsync(cancellationToken);

            // STEP 2: apply final order safely
            for (int i = 0; i < request.OrderedColumnIds.Count; i++)
            {
                BoardColumn column = columns.First(x => x.Id == request.OrderedColumnIds[i]);
                column.Order = i;
            }

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
