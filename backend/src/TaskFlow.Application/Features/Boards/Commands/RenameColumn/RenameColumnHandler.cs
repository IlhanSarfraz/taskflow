using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Features.Boards.Commands.RenameColumn
{
    public sealed class RenameColumnHandler
        : IRequestHandler<RenameColumnCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IProjectAuthorizationService _auth;

        public RenameColumnHandler(
            IApplicationDbContext context,
            IProjectAuthorizationService auth)
        {
            _context = context;
            _auth = auth;
        }

        public async Task Handle(
            RenameColumnCommand request,
            CancellationToken cancellationToken)
        {
            BoardColumn column = await _context.BoardColumns
                .Include(x => x.Board)
                .FirstOrDefaultAsync(x =>
                    x.Id == request.ColumnId,
                    cancellationToken)
                ?? throw new KeyNotFoundException();

            await _auth.EnsureAdminAsync(column.Board.ProjectId, cancellationToken);

            column.Name = request.Name;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
