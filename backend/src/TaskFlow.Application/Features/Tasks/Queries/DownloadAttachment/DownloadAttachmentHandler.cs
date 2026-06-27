using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Features.Tasks.Dtos;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Features.Tasks.Queries.DownloadAttachment;

public sealed class DownloadAttachmentHandler
    : IRequestHandler<DownloadAttachmentQuery, DownloadAttachmentResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IProjectAuthorizationService _authorization;

    public DownloadAttachmentHandler(
        IApplicationDbContext context,
        IProjectAuthorizationService authorization)
    {
        _context = context;
        _authorization = authorization;
    }

    public async Task<DownloadAttachmentResponse> Handle(
        DownloadAttachmentQuery request,
        CancellationToken cancellationToken)
    {
        TaskAttachment attachment = await _context.TaskAttachments
            .Include(x => x.Task)
            .FirstOrDefaultAsync(
                x => x.Id == request.AttachmentId,
                cancellationToken)
            ?? throw new KeyNotFoundException("Attachment not found.");

        await _authorization.EnsureMemberAsync(
            attachment.Task.ProjectId,
            cancellationToken);

        HttpClient client = new();

        Stream stream = await client.GetStreamAsync(
            attachment.Url,
            cancellationToken);

        return new DownloadAttachmentResponse(
            stream,
            attachment.ContentType,
            attachment.FileName);
    }
}