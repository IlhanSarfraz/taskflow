namespace TaskFlow.Application.Features.Auth.DTOs
{
    public sealed record RegisterRequest(
        string FirstName,
        string LastName,
        string Email,
        string Password);
}
