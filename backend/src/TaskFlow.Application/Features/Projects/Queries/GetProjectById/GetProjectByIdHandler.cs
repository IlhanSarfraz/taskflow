using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Features.Auth.DTOs.Project;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Features.Projects.Queries.GetProjectById
{
    public sealed class GetProjectByIdHandler
        : IRequestHandler<GetProjectByIdQuery, ProjectResponse>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public GetProjectByIdHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<ProjectResponse> Handle(
            GetProjectByIdQuery request,
            CancellationToken cancellationToken)
        {
            Project? project = await _context.Projects
                .AsNoTracking()
                .FirstOrDefaultAsync(
                x => x.Id == request.Id &&
                (
                    x.OwnerId == _currentUser.UserId ||
                    x.Members.Any(m => m.UserId == _currentUser.UserId)
                ),
                cancellationToken) ??
                throw new KeyNotFoundException(
                    $"Project '{request.Id}' not found");

            return new ProjectResponse(
                project.Id,
                project.Name,
                project.Key,
                project.Description,
                project.OwnerId,
                project.CreatedAtUtc);
        }
    }
}
