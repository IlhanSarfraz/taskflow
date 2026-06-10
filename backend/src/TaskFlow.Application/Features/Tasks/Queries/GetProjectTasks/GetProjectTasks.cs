using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Features.Tasks.Dtos;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Features.Tasks.Queries.GetProjectTasks
{
    public sealed class GetProjectTasksHandler
        : IRequestHandler<GetProjectTasksQuery, List<TaskDetailsResponse>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public GetProjectTasksHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<List<TaskDetailsResponse>> Handle(
            GetProjectTasksQuery request,
            CancellationToken cancellationToken)
        {
            IQueryable<TaskItem> query = _context.Tasks
                .AsNoTracking()
                .Where(x =>
                    x.ProjectId == request.ProjectId &&
                    x.Project.OwnerId == _currentUser.UserId);

            if (request.ColumnId.HasValue)
            {
                query = query.Where(x => x.BoardColumnId == request.ColumnId);
            }

            if (request.Priority.HasValue)
            {
                query = query.Where(x => x.Priority == request.Priority);
            }

            return await query
                .OrderByDescending(x => x.CreatedAtUtc)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
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
