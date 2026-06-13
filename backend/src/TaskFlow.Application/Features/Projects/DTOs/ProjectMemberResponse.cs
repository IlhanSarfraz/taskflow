namespace TaskFlow.Application.Features.Projects.DTOs
{
    public sealed record ProjectMemberResponse(
        Guid UserId,
        string FullName,
        string Email,
        string Role);
}