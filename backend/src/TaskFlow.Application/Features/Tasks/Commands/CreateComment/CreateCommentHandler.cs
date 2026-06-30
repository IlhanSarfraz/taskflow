using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Features.Tasks.Dtos;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Features.Tasks.Commands.CreateComment
{
    public sealed class CreateCommentHandler
        : IRequestHandler<CreateCommentCommand, CommentResponse>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;
        private readonly IProjectAuthorizationService _auth;
        private readonly IActivityLogger _activityLogger;

        public CreateCommentHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUser,
            IProjectAuthorizationService auth,
            IActivityLogger activityLogger)
        {
            _context = context;
            _currentUser = currentUser;
            _auth = auth;
            _activityLogger = activityLogger;
        }

        public async Task<CommentResponse> Handle(
            CreateCommentCommand request,
            CancellationToken cancellationToken)
        {
            await _auth.EnsureTaskMemberAsync(
                request.TaskId,
                cancellationToken);

            TaskItem task = await _context.Tasks
                .Include(x => x.Project)
                .Include(x => x.BoardColumn)
                    .ThenInclude(bc => bc.Board)
                .FirstOrDefaultAsync(
                    x => x.Id == request.TaskId,
                    cancellationToken)
                ?? throw new KeyNotFoundException("Task not found.");

            User user = await _context.Users
                .FirstOrDefaultAsync(
                    x => x.Id == _currentUser.UserId,
                    cancellationToken)
                ?? throw new KeyNotFoundException("User not found.");

            TaskComment comment = new()
            {
                TaskId = request.TaskId,
                UserId = _currentUser.UserId,
                Content = request.Content
            };

            _context.TaskComments.Add(comment);

            await _activityLogger.LogAsync(
                _currentUser.UserId,
                "CommentAdded",
                "Task",
                request.TaskId,
                $"Commented on task \"{task.Title}\"",
                task.ProjectId,
                task.Project.Name,
                task.BoardColumn.BoardId,
                task.BoardColumn.Board.Name,
                cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return new CommentResponse(
                comment.Id,
                comment.UserId,
                $"{user.FirstName} {user.LastName}",
                comment.Content,
                comment.CreatedAtUtc);
        }
    }
}
