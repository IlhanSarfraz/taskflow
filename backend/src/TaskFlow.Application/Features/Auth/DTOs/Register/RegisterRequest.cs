namespace TaskFlow.Application.Features.Auth.DTOs.Register
{
    public sealed record RegisterRequest(
        string FirstName,
        string LastName,
        string Email,
        string Password);
}
