using MediatR;
using TaskFlow.Application.Features.Tasks.Dtos;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.Features.Tasks.Queries.GetProjectTasks
{
    public sealed record GetProjectTasksQuery(
        Guid ProjectId,
        Guid? ColumnId,
        TaskPriority? Priority,
        int Page = 1,
        int PageSize = 20)
        : IRequest<List<TaskDetailsResponse>>;
}
