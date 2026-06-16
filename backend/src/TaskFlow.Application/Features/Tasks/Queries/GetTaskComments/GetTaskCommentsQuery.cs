using MediatR;
using TaskFlow.Application.Features.Tasks.Dtos;

namespace TaskFlow.Application.Features.Tasks.Queries.GetTaskComments
{
    public sealed record GetTaskCommentsQuery(
        Guid TaskId)
        : IRequest<List<CommentResponse>>;
}
