namespace TaskFlow.Application.Features.Users.DTOs
{
    public sealed record UserSummaryResponse(
        Guid Id,
        string FullName,
        string Email);
}