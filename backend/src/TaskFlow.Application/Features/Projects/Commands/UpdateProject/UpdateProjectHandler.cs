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
        private readonly IActivityLogger _activityLogger;

        public UpdateProjectHandler(
            ICurrentUserService currentUser,
            IApplicationDbContext context,
            IActivityLogger activityLogger)
        {
            _currentUser = currentUser;
            _context = context;
            _activityLogger = activityLogger;
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

            if (project.OwnerId != _currentUser.UserId)
            {
                throw new UnauthorizedAccessException(
                    "You are not authorized to update this project.");
            }

            project.Name = request.Name;
            project.Description = request.Description;

            await _activityLogger.LogAsync(
                _currentUser.UserId, "ProjectUpdated", "Project",
                project.Id, $"Updated project \"{project.Name}\"", project.Id, project.Name, null, null, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

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
