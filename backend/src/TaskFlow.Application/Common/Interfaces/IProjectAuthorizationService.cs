namespace TaskFlow.Application.Common.Interfaces
{
    public interface IProjectAuthorizationService
    {
        Task EnsureMemberAsync(Guid projectId, CancellationToken ct = default);
        Task EnsureAdminAsync(Guid projectId, CancellationToken ct = default);
    }
}
