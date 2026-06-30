using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Features.Tasks.Commands.UpdateComment
{
    public sealed class UpdateCommentHandler
        : IRequestHandler<UpdateCommentCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;
        private readonly IActivityLogger _activityLogger;

        public UpdateCommentHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUser,
            IActivityLogger activityLogger)
        {
            _context = context;
            _currentUser = currentUser;
            _activityLogger = activityLogger;
        }

        public async Task Handle(
            UpdateCommentCommand request,
            CancellationToken cancellationToken)
        {
            TaskComment comment = await _context.TaskComments
                .Include(x => x.Task)
                    .ThenInclude(t => t.Project)
                .Include(x => x.Task)
                    .ThenInclude(t => t.BoardColumn)
                        .ThenInclude(bc => bc.Board)
                .FirstOrDefaultAsync(
                    x => x.Id == request.CommentId,
                    cancellationToken)
                ?? throw new KeyNotFoundException("Comment not found.");

            if (comment.UserId != _currentUser.UserId)
                throw new UnauthorizedAccessException(
                    "You can only edit your own comments.");

            comment.Content = request.Content;

            await _activityLogger.LogAsync(
                _currentUser.UserId,
                "CommentUpdated",
                "Task",
                comment.TaskId,
                $"Edited a comment on task \"{comment.Task.Title}\"",
                comment.Task.ProjectId,
                comment.Task.Project.Name,
                comment.Task.BoardColumn.BoardId,
                comment.Task.BoardColumn.Board.Name,
                cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}