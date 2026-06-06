namespace TaskFlow.Application.Features.Auth.DTOs
{
    public sealed record RegisterResponse(
        Guid UserId,
        string Email);
}
