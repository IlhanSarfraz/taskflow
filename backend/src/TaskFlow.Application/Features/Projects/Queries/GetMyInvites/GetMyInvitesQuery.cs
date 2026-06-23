using MediatR;
using TaskFlow.Application.Features.Projects.DTOs;

namespace TaskFlow.Application.Features.Projects.Queries.GetMyInvites
{
    public sealed record GetMyInvitesQuery : IRequest<List<InviteResponse>>;
}