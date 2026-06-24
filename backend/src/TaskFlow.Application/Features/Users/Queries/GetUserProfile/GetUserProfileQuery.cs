using MediatR;
using TaskFlow.Application.Features.Users.DTOs;

namespace TaskFlow.Application.Features.Users.Queries.GetUserProfile
{
    public sealed record GetUserProfileQuery : IRequest<UserProfileResponse>;

}
