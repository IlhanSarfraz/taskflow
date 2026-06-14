using MediatR;

namespace TaskFlow.Application.Features.Boards.Commands.DeleteColumn;

public sealed record DeleteColumnCommand(
    Guid ColumnId
) : IRequest;