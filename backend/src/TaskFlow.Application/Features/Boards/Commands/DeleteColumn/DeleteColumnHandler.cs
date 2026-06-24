using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Features.Boards.Commands.DeleteColumn
{
    public sealed class DeleteColumnHandler
        : IRequestHandler<DeleteColumnCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IProjectAuthorizationService _auth;

        public DeleteColumnHandler(
            IApplicationDbContext context,
            IProjectAuthorizationService auth)
        {
            _context = context;
            _auth = auth;
        }

        public async Task Handle(DeleteColumnCommand request, CancellationToken cancellationToken)
        {
            BoardColumn column = await _context.BoardColumns
                .Include(x => x.Board)
                .FirstOrDefaultAsync(x => x.Id == request.ColumnId, cancellationToken)
                ?? throw new KeyNotFoundException("Column not found");

            await _auth.EnsureAdminAsync(column.Board.ProjectId, cancellationToken);

            _context.BoardColumns.Remove(column);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
