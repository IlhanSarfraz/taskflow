using MediatR;

namespace TaskFlow.Application.Features.Tasks.Commands.DeleteComment
{
    public sealed record DeleteCommentCommand(
        Guid CommentId)
        : IRequest;
}
