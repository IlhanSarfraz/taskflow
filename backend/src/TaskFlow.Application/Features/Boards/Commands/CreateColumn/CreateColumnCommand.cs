using MediatR;

namespace TaskFlow.Application.Features.Boards.Commands.CreateColumn
{
    public sealed record CreateColumnCommand(
        Guid BoardId,
        string Name,
        int Order
        ) : IRequest<Guid>;
}
