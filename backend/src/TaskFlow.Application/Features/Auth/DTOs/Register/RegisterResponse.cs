namespace TaskFlow.Application.Features.Auth.DTOs.Register
{
    public sealed record RegisterResponse(
        Guid UserId,
        string Email);
}
