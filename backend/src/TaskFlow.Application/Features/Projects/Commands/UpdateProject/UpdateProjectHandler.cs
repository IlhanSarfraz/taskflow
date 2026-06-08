using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Features.Auth.DTOs.Project;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Features.Projects.Commands.UpdateProject
{
    public sealed class UpdateProjectHandler
        : IRequestHandler<UpdateProjectCommand, ProjectResponse>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public UpdateProjectHandler(
            ICurrentUserService currentUser,
            IApplicationDbContext context)
        {
            _currentUser = currentUser;
            _context = context;
        }

        public async Task<ProjectResponse> Handle(
            UpdateProjectCommand request,
            CancellationToken cancellationToken)
        {
            Project? project = await _context.Projects
                .FirstOrDefaultAsync(
                x => x.Id == request.Id &&
                x.OwnerId == _currentUser.UserId) ??
                throw new KeyNotFoundException(
                    $"Project '{request.Id}' not found");

            project.Name = request.Name;
            project.Description = request.Description;

            await _context.SaveChangesAsync(cancellationToken);

            return new ProjectResponse(
                project.Id,
                project.Name,
                project.Key,
                project.Description,
                project.OwnerId);
        }
    }
}
