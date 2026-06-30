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
        private readonly IActivityLogger _activityLogger;

        public CreateTaskHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUser,
            IActivityLogger activityLogger)
        {
            _context = context;
            _currentUser = currentUser;
            _activityLogger = activityLogger;
        }

        public async Task<TaskResponse> Handle(
            CreateTaskCommand request,
            CancellationToken cancellationToken)
        {
            Project project = await _context.Projects
                .FirstOrDefaultAsync(
                    x => x.Id == request.ProjectId &&
                         (
                             x.OwnerId == _currentUser.UserId ||
                             x.Members.Any(m => m.UserId == _currentUser.UserId)
                         ),
                    cancellationToken)
                ?? throw new KeyNotFoundException("Project not found.");

            BoardColumn column = await _context.BoardColumns
                .Include(x => x.Board)
                .FirstOrDefaultAsync(
                    x => x.Id == request.BoardColumnId &&
                         x.Board.ProjectId == request.ProjectId,
                    cancellationToken)
                ?? throw new KeyNotFoundException("Column not found.");

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

            await _activityLogger.LogAsync(
                _currentUser.UserId,
                "TaskCreated",
                "Task",
                task.Id,
                $"Created task \"{task.Title}\"",
                project.Id,
                project.Name,
                column.BoardId,
                column.Board.Name,
                cancellationToken);

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
