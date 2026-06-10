using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Features.Tasks.Dtos;

namespace TaskFlow.Application.Features.Tasks.Queries.GetMyTasks
{
    public sealed class GetMyTasksHandler
        : IRequestHandler<GetMyTasksQuery, List<TaskDetailsResponse>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public GetMyTasksHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<List<TaskDetailsResponse>> Handle(
            GetMyTasksQuery request,
            CancellationToken cancellationToken)
        {
            return await _context.Tasks
                .AsNoTracking()
                .Where(x => x.AssigneeId == _currentUser.UserId)
                .Select(x => new TaskDetailsResponse(
                    x.Id,
                    x.Title,
                    x.Description,
                    x.Priority,
                    x.DueDate,
                    x.ProjectId,
                    x.BoardColumnId,
                    x.AssigneeId))
                .ToListAsync(cancellationToken);
        }
    }
}
