using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Features.Auth.Commands.CreateProject;
using TaskFlow.Application.Features.Auth.DTOs.Project;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.Features.Auth.Handlers
{
    public sealed class CreateProjectHandler
        : IRequestHandler<CreateProjectCommand, ProjectResponse>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IActivityLogger _activityLogger;

        public CreateProjectHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUserService,
            IActivityLogger activityLogger)
        {
            _context = context;
            _currentUserService = currentUserService;
            _activityLogger = activityLogger;
        }

        public async Task<ProjectResponse> Handle(
            CreateProjectCommand request,
            CancellationToken cancellationToken)
        {
            bool keyExists = await _context.Projects
            .AnyAsync(
                x => x.Key == request.Key,
                cancellationToken);

            if (keyExists)
                throw new InvalidOperationException(
                    $"Project key '{request.Key}' already exists.");


            Project project = new()
            {
                Name = request.Name,
                Key = request.Key.ToUpper(),
                Description = request.Description,
                OwnerId = _currentUserService.UserId
            };

            _context.Projects.Add(project);

            _context.ProjectMembers.Add(
                new ProjectMember
                {
                    Project = project,
                    UserId = _currentUserService.UserId,
                    Role = ProjectMemberRole.Admin
                });

            await _activityLogger.LogAsync(
                _currentUserService.UserId, "ProjectCreated", "Project",
                project.Id, $"Created project \"{project.Name}\"", project.Id, project.Name, null, null, cancellationToken);

            await _context.SaveChangesAsync(
                cancellationToken);

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
