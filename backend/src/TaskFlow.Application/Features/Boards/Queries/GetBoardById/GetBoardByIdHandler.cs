using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Features.Boards.DTOs;
using TaskFlow.Application.Features.Tasks.Dtos;

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
            BoardDetailsResponse? board = await _context.Boards
                .AsNoTracking()
                .Where(x =>
                    x.Id == request.BoardId &&
                    x.Project.OwnerId == _currentUser.UserId)
                .Select(b => new BoardDetailsResponse(
                    b.Id,
                    b.Name,
                    b.ProjectId,
                    b.Columns
                        .OrderBy(c => c.Order)
                        .Select(c => new BoardColumnResponse(
                            c.Id,
                            c.Name,
                            c.Order,
                            c.Tasks
                                .OrderBy(t => t.Order)
                                .Select(t => new TaskSummaryResponse(
                                    t.Id,
                                    t.Title,
                                    t.Priority,
                                    t.DueDate,
                                    t.Assignments
                                        .OrderBy(a => a.User.FirstName)
                                        .Select(a =>
                                            (a.User.FirstName.Substring(0, 1) + a.User.LastName.Substring(0, 1)).ToUpper())
                                        .FirstOrDefault()
                                        ?? (t.AssigneeId == null
                                            ? null
                                            : _context.Users
                                                .Where(u => u.Id == t.AssigneeId)
                                                .Select(u =>
                                                    (u.FirstName.Substring(0, 1) + u.LastName.Substring(0, 1)).ToUpper())
                                                .FirstOrDefault())
                                ))
                                .ToList()
                        ))
                        .ToList()
                ))
                .FirstOrDefaultAsync(cancellationToken);

            if (board is null)
                throw new KeyNotFoundException("Board not found.");

            return board;
        }
    }
}
