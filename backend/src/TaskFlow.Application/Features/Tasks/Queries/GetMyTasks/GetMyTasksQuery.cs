using MediatR;
using TaskFlow.Application.Features.Tasks.Dtos;

namespace TaskFlow.Application.Features.Tasks.Queries.GetMyTasks
{
    public sealed record GetMyTasksQuery()
        : IRequest<List<TaskDetailsResponse>>;
}
