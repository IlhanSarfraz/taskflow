using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Features.Tasks.Dtos;

namespace TaskFlow.Application.Features.Tasks.Queries.GetTaskAttachments;

public sealed class GetTaskAttachmentsHandler
    : IRequestHandler<GetTaskAttachmentsQuery, List<TaskAttachmentDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IProjectAuthorizationService _authorization;

    public GetTaskAttachmentsHandler(
        IApplicationDbContext context,
        IProjectAuthorizationService authorization)
    {
        _context = context;
        _authorization = authorization;
    }

    public async Task<List<TaskAttachmentDto>> Handle(
        GetTaskAttachmentsQuery request,
        CancellationToken cancellationToken)
    {
        Domain.Entities.TaskItem task = await _context.Tasks
            .Include(x => x.Project)
            .FirstOrDefaultAsync(
                x => x.Id == request.TaskId,
                cancellationToken)
            ?? throw new KeyNotFoundException("Task not found.");

        await _authorization.EnsureMemberAsync(
            task.ProjectId,
            cancellationToken);

        IQueryable<TaskAttachmentDto> query =
            from a in _context.TaskAttachments
            where a.TaskId == request.TaskId
            join u in _context.Users on a.UploadedBy equals u.Id into uploaderJoin
            from u in uploaderJoin.DefaultIfEmpty()
            orderby a.CreatedAtUtc descending
            select new TaskAttachmentDto
            {
                Id = a.Id,
                FileName = a.FileName,
                ContentType = a.ContentType,
                FileSize = a.FileSize,
                Url = a.Url,
                UploadedByUserId = a.UploadedBy,
                UploadedByName = u != null
                    ? (u.FirstName + " " + u.LastName)
                    : "Unknown user",
                UploadedByEmail = u != null
                    ? (u.Email)
                    : "Unknown user",
                CreatedAtUtc = a.CreatedAtUtc
            };

        return await query.ToListAsync(cancellationToken);
    }
}