namespace TaskFlow.Application.Features.Auth.DTOs.Project
{
    public sealed record CreateProjectRequest(
        string Name,
        string Key,
        string Description);
}
