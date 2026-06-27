using MediatR;
using Microsoft.AspNetCore.Http;

namespace TaskFlow.Application.Features.Tasks.Commands.UploadAttachment
{
    public sealed record UploadAttachmentCommand(
        Guid TaskId,
        IFormFile File)
        : IRequest<Guid>;
}
