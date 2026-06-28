namespace TaskFlow.Application.Common.Interfaces
{
    public interface IProjectAuthorizationService
    {
        Task EnsureMemberAsync(Guid projectId, CancellationToken ct = default);
        Task EnsureAdminAsync(Guid projectId, CancellationToken ct = default);
        Task EnsureProjectManagerAsync(Guid projectId, CancellationToken cancellationToken = default);
        Task EnsureTaskMemberAsync(Guid taskId, CancellationToken cancellationToken);
        Task<List<Guid>> GetAccessibleProjectIdsAsync(CancellationToken cancellationToken = default);
    }
}
