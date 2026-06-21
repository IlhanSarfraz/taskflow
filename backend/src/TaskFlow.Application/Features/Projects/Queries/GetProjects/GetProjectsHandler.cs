using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Features.Auth.DTOs.Project;

namespace TaskFlow.Application.Features.Projects.Queries.GetProjects
{
    public sealed class GetProjectsHandler
        : IRequestHandler<GetProjectsQuery,
            List<ProjectResponse>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public GetProjectsHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<List<ProjectResponse>> Handle(
            GetProjectsQuery request,
            CancellationToken cancellationToken)
        {
            return await _context.Projects
                .AsNoTracking()
                .Where(x => x.OwnerId == _currentUser.UserId)
                .OrderBy(x => x.Name)
                .Select(x => new ProjectResponse(
                    x.Id,
                    x.Name,
                    x.Key,
                    x.Description,
                    x.OwnerId,
                    x.CreatedAtUtc))
                .ToListAsync(cancellationToken);
        }
    }
}
