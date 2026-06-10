using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Features.Tasks.Commands.UpdateTask
{
    public sealed class UpdateTaskHandler
        : IRequestHandler<UpdateTaskCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public UpdateTaskHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task Handle(
            UpdateTaskCommand request,
            CancellationToken cancellationToken)
        {
            TaskItem? task = await _context.Tasks
                .Include(x => x.Project)
                .FirstOrDefaultAsync(
                    x => x.Id == request.TaskId &&
                         x.Project.OwnerId == _currentUser.UserId,
                    cancellationToken)
                ?? throw new KeyNotFoundException("Task not found.");

            task.Title = request.Title;
            task.Description = request.Description;
            task.Priority = request.Priority;
            task.DueDate = request.DueDate;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
