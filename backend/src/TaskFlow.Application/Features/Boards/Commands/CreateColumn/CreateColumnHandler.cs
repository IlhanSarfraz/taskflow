using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Features.Boards.Commands.CreateColumn;

public sealed class CreateColumnHandler
    : IRequestHandler<CreateColumnCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IProjectAuthorizationService _auth;

    public CreateColumnHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IProjectAuthorizationService auth)
    {
        _context = context;
        _currentUser = currentUser;
        _auth = auth;
    }

    public async Task<Guid> Handle(
        CreateColumnCommand request,
        CancellationToken cancellationToken)
    {
        Board? board = await _context.Boards
            .Include(x => x.Project)
            .FirstOrDefaultAsync(x => x.Id == request.BoardId, cancellationToken);

        await _auth.EnsureMemberAsync(board!.ProjectId, cancellationToken);

        BoardColumn column = new()
        {
            BoardId = request.BoardId,
            Name = request.Name,
            Order = request.Order
        };

        _context.BoardColumns.Add(column);

        await _context.SaveChangesAsync(cancellationToken);

        return column.Id;
    }
}