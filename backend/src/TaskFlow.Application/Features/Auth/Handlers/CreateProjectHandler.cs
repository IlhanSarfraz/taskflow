using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Features.Auth.Commands.CreateProject;
using TaskFlow.Application.Features.Auth.DTOs.Project;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Features.Auth.Handlers
{
    public sealed class CreateProjectHandler
        : IRequestHandler<CreateProjectCommand, ProjectResponse>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public CreateProjectHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
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

            await _context.SaveChangesAsync();

            return new ProjectResponse(
                project.Id,
                project.Name,
                project.Key,
                project.Description,
                project.OwnerId);
        }
    }
}
