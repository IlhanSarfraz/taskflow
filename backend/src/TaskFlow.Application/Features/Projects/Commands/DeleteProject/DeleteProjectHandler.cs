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
            Project? project = await _context.Projects
                .FirstOrDefaultAsync(x => x.Id == request.Id &&
                x.OwnerId == _currentUser.UserId,
                cancellationToken) ??
                throw new KeyNotFoundException(
                    $"Project '{request.Id}' not found.");

            _context.Projects.Remove(project);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
