using MediatR;

namespace TaskFlow.Application.Features.Tasks.Commands.DeleteAttachment;

public sealed record DeleteAttachmentCommand(
    Guid AttachmentId)
    : IRequest;