using MediatR;
using TaskFlow.Application.Features.Boards.DTOs;

namespace TaskFlow.Application.Features.Boards.Queries.GetBoardsByProject
{
    public sealed record GetBoardsByProjectQuery(Guid ProjectId) :
        IRequest<List<BoardSummaryResponse>>;
}
