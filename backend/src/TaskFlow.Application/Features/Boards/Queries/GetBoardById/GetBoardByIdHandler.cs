using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Features.Boards.DTOs;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Features.Boards.Queries.GetBoardById
{
    public sealed class GetBoardByIdHandler
        : IRequestHandler<GetBoardByIdQuery, BoardDetailsResponse>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public GetBoardByIdHandler(IApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<BoardDetailsResponse> Handle(
            GetBoardByIdQuery request,
            CancellationToken cancellationToken)
        {
            Board board = await _context.Boards
                .AsNoTracking()
                .Include(x => x.Project)
                .Include(x => x.Columns)
                .FirstOrDefaultAsync(
                    x => x.Id == request.BoardId &&
                         x.Project.OwnerId == _currentUser.UserId,
                    cancellationToken)
                ?? throw new KeyNotFoundException("Board not found.");

            return new BoardDetailsResponse(
                board.Id,
                board.Name,
                board.ProjectId,
                board.Columns
                    .OrderBy(x => x.Order)
                    .Select(x => new BoardColumnResponse(
                        x.Id,
                        x.Name,
                        x.Order))
                    .ToList());
        }
    }
}
