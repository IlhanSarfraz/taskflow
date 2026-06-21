using MediatR;
using TaskFlow.Application.Features.Tasks.Dtos;

namespace TaskFlow.Application.Features.Tasks.Queries.GetTaskDetailPage;

public sealed record GetTaskDetailPageQuery(Guid TaskId)
    : IRequest<TaskDetailPageResponse>;
