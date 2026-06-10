using MediatR;
using TaskFlow.Application.Features.Tasks.Dtos;

namespace TaskFlow.Application.Features.Tasks.Queries.GetTaskById
{
    public sealed record GetTaskByIdQuery(Guid TaskId)
        : IRequest<TaskDetailsResponse>;
}
