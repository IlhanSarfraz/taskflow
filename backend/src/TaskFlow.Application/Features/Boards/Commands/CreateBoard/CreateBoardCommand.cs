using MediatR;
using TaskFlow.Application.Features.Boards.DTOs;

namespace TaskFlow.Application.Features.Boards.Commands.CreateBoard
{
    public sealed record CreateBoardCommand(
        Guid ProjectId,
        string Name)
        : IRequest<BoardResponse>;
}
