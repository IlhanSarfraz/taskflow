using MediatR;
using TaskFlow.Application.Features.Users.DTOs;

namespace TaskFlow.Application.Features.Users.Queries.SearchUsers
{
    public sealed record SearchUsersQuery(
        string Search)
        : IRequest<List<UserSummaryResponse>>;
}