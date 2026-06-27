using MediatR;
using TaskFlow.Application.Features.Tasks.Dtos;

namespace TaskFlow.Application.Features.Tasks.Queries.GetTaskAttachments;

public sealed record GetTaskAttachmentsQuery(Guid TaskId)
    : IRequest<List<TaskAttachmentDto>>;