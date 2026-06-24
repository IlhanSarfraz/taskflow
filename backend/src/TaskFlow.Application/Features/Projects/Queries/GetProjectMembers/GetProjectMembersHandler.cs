using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Features.Projects.DTOs;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Features.Projects.Queries.GetProjectMembers;

public sealed class GetProjectMembersHandler
    : IRequestHandler<GetProjectMembersQuery, List<ProjectMemberResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IProjectAuthorizationService _auth;

    public GetProjectMembersHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IProjectAuthorizationService projectAuthorizationService)
    {
        _context = context;
        _currentUser = currentUser;
        _auth = projectAuthorizationService;
    }

    public async Task<List<ProjectMemberResponse>> Handle(
        GetProjectMembersQuery request,
        CancellationToken cancellationToken)
    {
        await _auth
            .EnsureMemberAsync(request.ProjectId, cancellationToken);

        List<ProjectMemberResponse> members = await _context.ProjectMembers
            .AsNoTracking()
            .Where(x => x.ProjectId == request.ProjectId)
            .Select(x => new ProjectMemberResponse(
                x.UserId,
                x.User.FirstName + " " + x.User.LastName,
                x.User.Email,
                x.Role.ToString()))
            .ToListAsync(cancellationToken);

        Project? project = await _context.Projects
            .AsNoTracking()
            .Include(x => x.Owner)
            .FirstOrDefaultAsync(x => x.Id == request.ProjectId, cancellationToken);

        if (project is not null &&
            members.All(m => m.UserId != project.OwnerId))
        {
            members.Insert(0, new ProjectMemberResponse(
                project.OwnerId,
                project.Owner.FirstName + " " + project.Owner.LastName,
                project.Owner.Email,
                "Owner"));
        }

        return members;
    }
}
