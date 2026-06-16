using MediatR;

namespace TaskFlow.Application.Features.Tasks.Commands.UpdateComment
{
    public sealed record UpdateCommentCommand(
        Guid CommentId,
        string Content)
        : IRequest;
}