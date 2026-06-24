using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Features.Users.Commands.UpdateProfile;

public sealed class UpdateProfileHandler
    : IRequestHandler<UpdateProfileCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IActivityLogger _activityLogger;

    public UpdateProfileHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IActivityLogger activityLogger)
    {
        _context = context;
        _currentUser = currentUser;
        _activityLogger = activityLogger;
    }

    public async Task Handle(
        UpdateProfileCommand request,
        CancellationToken cancellationToken)
    {
        User user = await _context.Users
            .FirstOrDefaultAsync(
                x => x.Id == _currentUser.UserId,
                cancellationToken)
            ?? throw new KeyNotFoundException("User not found.");

        bool emailTaken = await _context.Users
            .AnyAsync(
                x => x.Email == request.Email &&
                     x.Id != _currentUser.UserId,
                cancellationToken);

        if (emailTaken)
            throw new InvalidOperationException("Email is already in use.");

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.Email = request.Email;

        await _activityLogger.LogAsync(
            _currentUser.UserId,
            "ProfileUpdated",
            "User",
            user.Id,
            "Updated profile information");

        await _context.SaveChangesAsync(cancellationToken);
    }
}