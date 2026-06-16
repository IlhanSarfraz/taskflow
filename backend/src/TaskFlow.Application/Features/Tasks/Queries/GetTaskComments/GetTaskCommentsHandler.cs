using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Features.Tasks.Dtos;

namespace TaskFlow.Application.Features.Tasks.Queries.GetTaskComments
{
    public sealed class GetTaskCommentsHandler
        : IRequestHandler<GetTaskCommentsQuery,
            List<CommentResponse>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IProjectAuthorizationService _auth;

        public GetTaskCommentsHandler(
            IApplicationDbContext context,
            IProjectAuthorizationService auth)
        {
            _context = context;
            _auth = auth;
        }

        public async Task<List<CommentResponse>> Handle(
            GetTaskCommentsQuery request,
            CancellationToken cancellationToken)
        {
            await _auth.EnsureTaskMemberAsync(
                request.TaskId,
                cancellationToken);

            return await _context.TaskComments
                .AsNoTracking()
                .Where(x => x.TaskId == request.TaskId)
                .OrderBy(x => x.CreatedAtUtc)
                .Select(x => new CommentResponse(
                    x.Id,
                    x.UserId,
                    x.User.FirstName + " " + x.User.LastName,
                    x.Content,
                    x.CreatedAtUtc))
                .ToListAsync(cancellationToken);
        }
    }
}