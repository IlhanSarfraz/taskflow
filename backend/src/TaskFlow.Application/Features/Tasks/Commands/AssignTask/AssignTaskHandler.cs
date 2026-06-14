using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Features.Tasks.Commands.AssignTask
{
    public sealed class AssignTaskHandler
        : IRequestHandler<AssignTaskCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public AssignTaskHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task Handle(
            AssignTaskCommand request,
            CancellationToken cancellationToken)
        {
            TaskItem? task = await _context.Tasks
                .Include(x => x.Project)
                .FirstOrDefaultAsync(
                    x => x.Id == request.TaskId &&
                        x.Project.OwnerId == _currentUser.UserId,
                    cancellationToken)
                ?? throw new KeyNotFoundException("Task not found.");

            bool memberExists = await _context.ProjectMembers
                .AnyAsync(x =>
                    x.ProjectId == task.ProjectId &&
                    x.UserId == request.AssigneeId,
                    cancellationToken);

            if (!memberExists)
                throw new InvalidOperationException(
                    "User is not a member of this project.");

            task.AssigneeId = request.AssigneeId;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
