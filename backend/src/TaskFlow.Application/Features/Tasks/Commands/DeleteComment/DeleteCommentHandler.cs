using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.Features.Tasks.Commands.DeleteComment
{
    public sealed class DeleteCommentHandler
        : IRequestHandler<DeleteCommentCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public DeleteCommentHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task Handle(
            DeleteCommentCommand request,
            CancellationToken cancellationToken)
        {
            TaskComment comment = await _context.TaskComments
                .Include(x => x.Task)
                .FirstOrDefaultAsync(
                    x => x.Id == request.CommentId,
                    cancellationToken)
                ?? throw new KeyNotFoundException();

            bool isAdmin = await _context.ProjectMembers
                .AnyAsync(x =>
                    x.ProjectId == comment.Task.ProjectId &&
                    x.UserId == _currentUser.UserId &&
                    x.Role == ProjectMemberRole.Admin,
                    cancellationToken);

            bool canDelete =
                comment.UserId == _currentUser.UserId ||
                isAdmin;

            if (!canDelete)
                throw new UnauthorizedAccessException();

            _context.TaskComments.Remove(comment);

            await _context.SaveChangesAsync(
                cancellationToken);
        }
    }
}