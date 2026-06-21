namespace TaskFlow.Application.Features.Auth.DTOs.Login
{
    public sealed record LoginResponse(
        string AccessToken,
        Guid UserId,
        string Email,
        string FirstName,
        string LastName);
}
