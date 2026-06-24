namespace TaskFlow.Application.Features.Users.DTOs
{
    public sealed record UserProfileResponse(
        Guid Id,
        string FirstName,
        string LastName,
        string Email,
        DateTime MemberSinceUtc,
        int TasksAssigned,
        int TasksCompleted,
        int TasksInProgress,
        int ProjectCount);
}
