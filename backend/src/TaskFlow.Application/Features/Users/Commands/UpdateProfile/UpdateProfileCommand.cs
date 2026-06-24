using MediatR;

namespace TaskFlow.Application.Features.Users.Commands.UpdateProfile;

public sealed record UpdateProfileCommand(
    string FirstName,
    string LastName,
    string Email) : IRequest;