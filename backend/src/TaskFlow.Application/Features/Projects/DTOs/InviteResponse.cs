namespace TaskFlow.Application.Features.Projects.DTOs
{
    public sealed record InviteResponse(
        Guid Id,
        Guid ProjectId,
        string ProjectName,
        string InvitedByName,
        string Role,
        DateTime CreatedAtUtc);
}