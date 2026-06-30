namespace TaskFlow.Application.Common.Interfaces;

public interface IActivityLogger
{
    Task LogAsync(
        Guid userId,
        string action,
        string entityType,
        Guid entityId,
        string description,
        Guid? projectId,
        string? projectName,
        Guid? boardId,
        string? boardName,
        CancellationToken cancellationToken = default);
}