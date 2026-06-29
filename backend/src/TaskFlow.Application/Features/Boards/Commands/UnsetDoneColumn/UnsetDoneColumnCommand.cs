using MediatR;

namespace TaskFlow.Application.Features.Boards.Commands.UnsetDoneColumn;

public sealed record UnsetDoneColumnCommand(Guid BoardId)
    : IRequest;