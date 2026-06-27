namespace TaskFlow.Application.Features.Tasks.Dtos;

public sealed class TaskAttachmentDto
{
    public Guid Id { get; init; }

    public string FileName { get; init; } = default!;

    public string ContentType { get; init; } = default!;

    public long FileSize { get; init; }

    public string Url { get; init; } = default!;

    public Guid UploadedByUserId { get; init; } = default!;

    public string UploadedByName { get; init; } = default!;

    public string UploadedByEmail { get; init; } = default!;

    public DateTime CreatedAtUtc { get; init; }
}