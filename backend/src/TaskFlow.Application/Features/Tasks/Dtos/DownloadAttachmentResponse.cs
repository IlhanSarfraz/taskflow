namespace TaskFlow.Application.Features.Tasks.Dtos;

public sealed record DownloadAttachmentResponse(
    Stream Stream,
    string ContentType,
    string FileName);