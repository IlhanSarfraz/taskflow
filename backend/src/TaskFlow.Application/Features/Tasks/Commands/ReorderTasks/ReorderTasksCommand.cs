using MediatR;

namespace TaskFlow.Application.Features.Tasks.Commands.ReorderTasks
{
    public sealed record ReorderTasksCommand(
        Guid ColumnId,
        List<Guid> OrderedTaskIds
    ) : IRequest;
}
