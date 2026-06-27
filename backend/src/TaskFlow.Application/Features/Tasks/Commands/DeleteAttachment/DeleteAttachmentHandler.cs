using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Features.Tasks.Commands.DeleteAttachment;

public sealed class DeleteAttachmentHandler
    : IRequestHandler<DeleteAttachmentCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IProjectAuthorizationService _authorization;
    private readonly IFileStorageService _storage;

    public DeleteAttachmentHandler(
        IApplicationDbContext context,
        IProjectAuthorizationService authorization,
        IFileStorageService storage)
    {
        _context = context;
        _authorization = authorization;
        _storage = storage;
    }

    public async Task Handle(
        DeleteAttachmentCommand request,
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

        await _storage.DeleteAsync(
            attachment.CloudinaryPublicId,
            cancellationToken);

        _context.TaskAttachments.Remove(attachment);

        await _context.SaveChangesAsync(cancellationToken);
    }
}