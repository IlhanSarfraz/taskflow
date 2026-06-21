using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Features.Tasks.Dtos;

namespace TaskFlow.Application.Features.Tasks.Queries.GetMyTasks;

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
            .Where(x =>
                x.AssigneeId == _currentUser.UserId ||
                x.Assignments.Any(a => a.UserId == _currentUser.UserId))
            .Select(x => new TaskDetailsResponse(
                x.Id,
                x.Title,
                x.Description,
                x.Priority,
                x.DueDate,
                x.ProjectId,
                x.Project.Name,
                x.BoardColumnId,
                x.BoardColumn.Name,
                x.BoardColumn.BoardId,
                x.Assignments
                    .OrderBy(a => a.User.FirstName)
                    .Select(a => new TaskAssigneeDto(
                        a.UserId,
                        a.User.FirstName + " " + a.User.LastName,
                        (a.User.FirstName.Substring(0, 1) + a.User.LastName.Substring(0, 1)).ToUpper()))
                    .ToList()))
            .ToListAsync(cancellationToken);
    }
}
