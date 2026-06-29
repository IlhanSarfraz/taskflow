using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Features.Boards.Commands.UnsetDoneColumn;

public sealed class UnsetDoneColumnHandler
    : IRequestHandler<UnsetDoneColumnCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IProjectAuthorizationService _authorization;
    private readonly ICurrentUserService _currentUser;
    private readonly IActivityLogger _activityLogger;

    public UnsetDoneColumnHandler(
        IApplicationDbContext context,
        IProjectAuthorizationService authorization,
        ICurrentUserService currentUser,
        IActivityLogger activityLogger)
    {
        _context = context;
        _authorization = authorization;
        _currentUser = currentUser;
        _activityLogger = activityLogger;
    }

    public async Task Handle(
        UnsetDoneColumnCommand request,
        CancellationToken cancellationToken)
    {
        Board board = await _context.Boards
            .FirstOrDefaultAsync(
                x => x.Id == request.BoardId,
                cancellationToken)
            ?? throw new KeyNotFoundException("Board not found.");

        await _authorization.EnsureProjectManagerAsync(
            board.ProjectId,
            cancellationToken);

        BoardColumn? doneColumn = await _context.BoardColumns
            .FirstOrDefaultAsync(
                x => x.BoardId == request.BoardId &&
                     x.IsDoneColumn,
                cancellationToken);

        if (doneColumn is null)
            return;

        doneColumn.IsDoneColumn = false;

        List<TaskItem> completedTasks = await _context.Tasks
            .Where(x => x.BoardColumnId == doneColumn.Id)
            .ToListAsync(cancellationToken);

        foreach (TaskItem task in completedTasks)
        {
            task.CompletedAtUtc = null;
        }

        await _activityLogger.LogAsync(
            _currentUser.UserId,
            "DoneColumnUnset",
            "Board",
            board.Id,
            $"Unset \"{doneColumn.Name}\" as the Done column.",
            cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }
}