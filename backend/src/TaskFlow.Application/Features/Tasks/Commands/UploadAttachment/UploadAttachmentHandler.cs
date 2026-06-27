using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Common.Storage;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Features.Tasks.Commands.UploadAttachment
{
    public sealed class UploadAttachmentHandler
        : IRequestHandler<UploadAttachmentCommand, Guid>
    {
        private readonly IApplicationDbContext _context;
        private readonly IProjectAuthorizationService _authorizationService;
        private readonly IFileStorageService _fileStorageService;
        private readonly ICurrentUserService _currentUser;

        public UploadAttachmentHandler(
            IApplicationDbContext context,
            IProjectAuthorizationService authorizationService,
            IFileStorageService fileStorageService,
            ICurrentUserService currentUser)
        {
            _context = context;
            _authorizationService = authorizationService;
            _fileStorageService = fileStorageService;
            _currentUser = currentUser;
        }

        public async Task<Guid> Handle(
            UploadAttachmentCommand request,
            CancellationToken cancellationToken)
        {
            TaskItem task = await _context.Tasks
                .Include(x => x.Project)
                .FirstOrDefaultAsync(
                    x => x.Id == request.TaskId,
                    cancellationToken)
                ?? throw new KeyNotFoundException("Task not found.");

            await _authorizationService.EnsureMemberAsync(
                task.ProjectId,
                cancellationToken);

            User user = await _context.Users
                .FirstOrDefaultAsync(
                    x => x.Id == _currentUser.UserId,
                    cancellationToken)
                ?? throw new UnauthorizedAccessException();

            FileUploadResult uploadResult =
                await _fileStorageService.UploadAsync(
                    request.File,
                    cancellationToken);

            TaskAttachment attachment = new()
            {
                Id = Guid.NewGuid(),
                TaskId = task.Id,
                FileName = request.File.FileName,
                ContentType = request.File.ContentType,
                FileSize = uploadResult.Bytes,
                Url = uploadResult.Url,
                CloudinaryPublicId = uploadResult.PublicId,
                UploadedBy = user.Id
            };

            _context.TaskAttachments.Add(attachment);

            await _context.SaveChangesAsync(cancellationToken);

            return attachment.Id;
        }
    }
}
