using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Features.Boards.DTOs;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.Features.Boards.Commands.CreateBoard
{
    public sealed class CreateBoardHandler
        : IRequestHandler<CreateBoardCommand, BoardResponse>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public CreateBoardHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<BoardResponse> Handle(
            CreateBoardCommand request,
            CancellationToken cancellationToken)
        {
            bool isAdmin = await _context.ProjectMembers
            .AnyAsync(x =>
                x.ProjectId == request.ProjectId &&
                x.UserId == _currentUser.UserId &&
                x.Role == ProjectMemberRole.Admin,
                cancellationToken);

            if (!isAdmin)
                throw new UnauthorizedAccessException("You are not allowed to add members.");


            Project? project = await _context.Projects
                .FirstOrDefaultAsync(x => x.Id == request.ProjectId &&
                (
                    x.OwnerId == _currentUser.UserId ||
                    x.Members.Any(m => m.UserId == _currentUser.UserId)
                ),
                cancellationToken) ??
                throw new KeyNotFoundException("Project not found");

            Board board = new()
            {
                Name = request.Name,
                ProjectId = request.ProjectId
            };

            _context.Boards.Add(board);

            List<BoardColumn> columns = new()
            {
                new BoardColumn { Name = "To Do", Order = 0, Board = board },
                new BoardColumn { Name = "In Progress", Order = 1, Board = board },
                new BoardColumn { Name = "Done", Order = 2, Board = board }
            };

            _context.BoardColumns.AddRange(columns);

            await _context.SaveChangesAsync(cancellationToken);

            return new BoardResponse(
                board.Id,
                board.Name,
                board.ProjectId);
        }
    }
}
