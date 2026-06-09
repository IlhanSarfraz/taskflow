using MediatR;
using TaskFlow.Application.Features.Boards.DTOs;

namespace TaskFlow.Application.Features.Boards.Queries.GetBoardById
{
    public sealed record GetBoardByIdQuery(Guid BoardId)
        : IRequest<BoardDetailsResponse>;
}
