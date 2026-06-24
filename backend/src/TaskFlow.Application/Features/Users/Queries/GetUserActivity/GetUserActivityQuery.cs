using MediatR;
using TaskFlow.Application.Features.Users.DTOs;

namespace TaskFlow.Application.Features.Users.Queries.GetUserActivity
{
    public sealed record GetUserActivityQuery(int Page, int PageSize)
        : IRequest<List<ActivityLogResponse>>;
}
