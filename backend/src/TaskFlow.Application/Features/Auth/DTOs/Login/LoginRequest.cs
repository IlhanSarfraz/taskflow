namespace TaskFlow.Application.Features.Auth.DTOs.Login
{
    public sealed record LoginRequest(
        string Email,
        string Password);
}
