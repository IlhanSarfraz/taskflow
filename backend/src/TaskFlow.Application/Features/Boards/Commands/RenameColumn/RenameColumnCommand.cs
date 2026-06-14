using MediatR;

namespace TaskFlow.Application.Features.Boards.Commands.RenameColumn;

public sealed record RenameColumnCommand(
    Guid ColumnId,
    string Name
) : IRequest;