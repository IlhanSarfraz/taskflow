using MediatR;
using TaskFlow.Application.Features.Tasks.Dtos;

namespace TaskFlow.Application.Features.Tasks.Queries.DownloadAttachment;

public sealed record DownloadAttachmentQuery(
    Guid AttachmentId)
    : IRequest<DownloadAttachmentResponse>;