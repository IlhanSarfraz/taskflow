using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Features.Boards.Commands.SetDoneColumn;

public sealed class SetDoneColumnHandler
    : IRequestHandler<SetDoneColumnCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IProjectAuthorizationService _authorization;
    private readonly ICurrentUserService _currentUser;
    private readonly IActivityLogger _activityLogger;

    public SetDoneColumnHandler(
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
    SetDoneColumnCommand request,
    CancellationToken cancellationToken)
    {
        Board board = await _context.Boards
            .Include(x => x.Project)                       // ← add
            .FirstOrDefaultAsync(
                x => x.Id == request.BoardId,
                cancellationToken)
            ?? throw new KeyNotFoundException("Board not found.");

        await _authorization.EnsureProjectManagerAsync(
            board.ProjectId,
            cancellationToken);

        List<BoardColumn> columns = await _context.BoardColumns
            .Where(x => x.BoardId == request.BoardId)
            .ToListAsync(cancellationToken);

        BoardColumn? newDoneColumn = columns.FirstOrDefault(x => x.Id == request.ColumnId);

        if (newDoneColumn is null)
            throw new KeyNotFoundException("Column not found.");

        BoardColumn? oldDoneColumn = columns.FirstOrDefault(x => x.IsDoneColumn);

        foreach (BoardColumn column in columns)
        {
            column.IsDoneColumn = column.Id == request.ColumnId;
        }

        // Tasks leaving Done
        if (oldDoneColumn is not null && oldDoneColumn.Id != newDoneColumn.Id)
        {
            List<TaskItem> oldTasks = await _context.Tasks
                .Where(x => x.BoardColumnId == oldDoneColumn.Id)
                .ToListAsync(cancellationToken);

            foreach (TaskItem task in oldTasks)
                task.CompletedAtUtc = null;
        }

        // Tasks entering Done
        List<TaskItem> newTasks = await _context.Tasks
            .Where(x => x.BoardColumnId == newDoneColumn.Id)
            .ToListAsync(cancellationToken);

        foreach (TaskItem task in newTasks)
            task.CompletedAtUtc ??= DateTime.UtcNow;

        await _activityLogger.LogAsync(
            _currentUser.UserId,
            "DoneColumnChanged",
            "Board",
            board.Id,
            $"Changed Done column to \"{newDoneColumn.Name}\".",
            board.ProjectId,
            board.Project.Name,
            board.Id,
            board.Name,
            cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }
}