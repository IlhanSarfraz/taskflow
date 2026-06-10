using MediatR;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.Features.Tasks.Commands.UpdateTask
{
    public sealed record UpdateTaskCommand(
        Guid TaskId,
        string Title,
        string? Description,
        TaskPriority Priority,
        DateTime? DueDate)
        : IRequest;
}
