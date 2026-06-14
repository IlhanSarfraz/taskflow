using MediatR;

namespace TaskFlow.Application.Features.Boards.Commands.ReorderColumns;

public sealed record ReorderColumnsCommand(
    Guid BoardId,
    List<Guid> OrderedColumnIds
) : IRequest;