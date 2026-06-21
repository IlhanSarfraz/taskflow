namespace TaskFlow.Application.Features.Tasks.Dtos;

public sealed record TaskAssigneeDto(
    Guid UserId,
    string FullName,
    string Initials);
