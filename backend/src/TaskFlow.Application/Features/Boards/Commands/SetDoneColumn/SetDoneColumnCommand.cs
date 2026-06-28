using MediatR;

namespace TaskFlow.Application.Features.Boards.Commands.SetDoneColumn;

public sealed record SetDoneColumnCommand(
    Guid BoardId,
    Guid ColumnId
) : IRequest;