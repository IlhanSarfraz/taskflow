using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Features.Tasks.Dtos;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Features.Tasks.Commands.CreateTask
{
    public sealed class CreateTaskHandler
        : IRequestHandler<CreateTaskCommand, TaskResponse>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public CreateTaskHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<TaskResponse> Handle(
            CreateTaskCommand request,
            CancellationToken cancellationToken)
        {
            // Validate project ownership\
            bool projectExists = await _context.Projects
                        .AnyAsync(
                            x => x.Id == request.ProjectId &&
                                 x.OwnerId == _currentUser.UserId,
                            cancellationToken);

            if (!projectExists)
                throw new KeyNotFoundException("Project not found.");

            // Validate column belongs to project
            bool columnExists = await _context.BoardColumns
                .AnyAsync(
                    x => x.Id == request.BoardColumnId &&
                         x.Board.ProjectId == request.ProjectId,
                    cancellationToken);

            if (!columnExists)
                throw new KeyNotFoundException("Column not found.");

            int nextOrder = await _context.Tasks
                .Where(x => x.BoardColumnId == request.BoardColumnId)
                .CountAsync(cancellationToken);

            TaskItem task = new()
            {
                Title = request.Title,
                Description = request.Description,
                Priority = request.Priority,
                DueDate = request.DueDate,
                ProjectId = request.ProjectId,
                BoardColumnId = request.BoardColumnId,
                Order = nextOrder
            };

            _context.Tasks.Add(task);

            await _context.SaveChangesAsync(cancellationToken);

            return new TaskResponse(
                task.Id,
                task.Title,
                task.Description,
                task.Priority,
                task.DueDate,
                task.ProjectId,
                task.BoardColumnId);
        }
    }
}
