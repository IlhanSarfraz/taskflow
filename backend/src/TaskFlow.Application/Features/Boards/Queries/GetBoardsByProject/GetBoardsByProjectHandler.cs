using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Features.Boards.DTOs;

namespace TaskFlow.Application.Features.Boards.Queries.GetBoardsByProject
{
    public sealed class GetBoardsByProjectHandler
        : IRequestHandler<GetBoardsByProjectQuery, List<BoardSummaryResponse>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public GetBoardsByProjectHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<List<BoardSummaryResponse>> Handle(
            GetBoardsByProjectQuery request,
            CancellationToken cancellationToken)
        {
            bool isProjectOwner = await _context.Projects
                .AnyAsync(
                    x => x.Id == request.ProjectId &&
                         x.OwnerId == _currentUser.UserId,
                    cancellationToken);

            if (!isProjectOwner)
                throw new KeyNotFoundException("Project not found.");

            return await _context.Boards
                .AsNoTracking()
                .Where(x => x.ProjectId == request.ProjectId)
                .OrderBy(x => x.Name)
                .Select(x => new BoardSummaryResponse(
                    x.Id,
                    x.Name,
                    x.ProjectId))
                .ToListAsync(cancellationToken);
        }
    }
}
