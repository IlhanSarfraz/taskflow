using MediatR;
using TaskFlow.Application.Features.Tasks.Dtos;

namespace TaskFlow.Application.Features.Tasks.Commands.CreateComment
{
    public sealed record CreateCommentCommand(
        Guid TaskId,
        string Content)
        : IRequest<CommentResponse>;
}