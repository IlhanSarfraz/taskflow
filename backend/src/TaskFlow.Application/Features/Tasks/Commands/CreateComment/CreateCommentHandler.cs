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

        public CreateCommentHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUser,
            IProjectAuthorizationService auth)
        {
            _context = context;
            _currentUser = currentUser;
            _auth = auth;
        }

        public async Task<CommentResponse> Handle(
            CreateCommentCommand request,
            CancellationToken cancellationToken)
        {
            await _auth.EnsureTaskMemberAsync(
                request.TaskId,
                cancellationToken);

            bool taskExists = await _context.Tasks
                .AnyAsync(
                    x => x.Id == request.TaskId,
                    cancellationToken);

            if (!taskExists)
                throw new KeyNotFoundException(
                    "Task not found.");

            TaskComment comment = new()
            {
                TaskId = request.TaskId,
                UserId = _currentUser.UserId,
                Content = request.Content
            };

            _context.TaskComments.Add(comment);

            await _context.SaveChangesAsync(
                cancellationToken);

            User user = await _context.Users
               .FirstAsync(
                   x => x.Id == _currentUser.UserId,
                   cancellationToken);

            return new CommentResponse(
                comment.Id,
                comment.UserId,
                $"{user.FirstName} {user.LastName}",
                comment.Content,
                comment.CreatedAtUtc);
        }
    }
}
