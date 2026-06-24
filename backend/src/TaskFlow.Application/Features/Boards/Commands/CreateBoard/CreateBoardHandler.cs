using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Constants;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Features.Boards.DTOs;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Features.Boards.Commands.CreateBoard
{
    public sealed class CreateBoardHandler
        : IRequestHandler<CreateBoardCommand, BoardResponse>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;
        private readonly IActivityLogger _activityLogger;

        public CreateBoardHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUser,
            IActivityLogger activityLogger)
        {
            _context = context;
            _currentUser = currentUser;
            _activityLogger = activityLogger;
        }

        public async Task<BoardResponse> Handle(
            CreateBoardCommand request,
            CancellationToken cancellationToken)
        {
            bool hasAccess = await _context.Projects
              .AnyAsync(x =>
                  x.Id == request.ProjectId &&
                  (
                      x.OwnerId == _currentUser.UserId ||
                      x.Members.Any(m => m.UserId == _currentUser.UserId)
                  ),
                  cancellationToken);

            if (!hasAccess)
            {
                throw new UnauthorizedAccessException(
                    "You are not authorized to create boards.");
            }


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

            List<BoardColumn> columns =
            [
                new BoardColumn { Name = BoardColumnNames.ToDo,        Order = 0, Board = board },
                new BoardColumn { Name = BoardColumnNames.InProgress,  Order = 1, Board = board },
                new BoardColumn { Name = BoardColumnNames.Done,        Order = 2, Board = board }
            ];

            _context.BoardColumns.AddRange(columns);

            await _activityLogger.LogAsync(
                _currentUser.UserId, "BoardCreated", "Board",
                board.Id, $"Created board \"{board.Name}\"", cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return new BoardResponse(
                board.Id,
                board.Name,
                board.ProjectId);
        }
    }
}
