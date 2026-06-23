using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Features.Projects.Commands.DeleteProject
{
    public sealed class DeleteProjectHandler
        : IRequestHandler<DeleteProjectCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public DeleteProjectHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task Handle(
            DeleteProjectCommand request,
            CancellationToken cancellationToken)
        {
            Project project = await _context.Projects
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
                ?? throw new KeyNotFoundException(
                    $"Project '{request.Id}' not found.");

            bool isOwner = project.OwnerId == _currentUser.UserId;

            bool isAdmin = await _context.ProjectMembers
                .AnyAsync(x =>
                    x.ProjectId == request.Id &&
                    x.UserId == _currentUser.UserId &&
                    x.Role == Domain.Enums.ProjectMemberRole.Admin,
                    cancellationToken);

            if (!isOwner && !isAdmin)
            {
                throw new UnauthorizedAccessException(
                    "You are not allowed to delete this project.");
            }

            _context.Projects.Remove(project);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}