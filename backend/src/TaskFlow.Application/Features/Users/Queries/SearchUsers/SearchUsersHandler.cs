using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Features.Users.DTOs;

namespace TaskFlow.Application.Features.Users.Queries.SearchUsers
{
    public sealed class SearchUsersHandler
        : IRequestHandler<SearchUsersQuery, List<UserSummaryResponse>>
    {
        private readonly IApplicationDbContext _context;

        public SearchUsersHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserSummaryResponse>> Handle(
            SearchUsersQuery request,
            CancellationToken cancellationToken)
        {
            string term = request.Search.Trim().ToLower();

            if (term.Length < 2)
                return new List<UserSummaryResponse>();

            return await _context.Users
                .AsNoTracking()
                .Where(u =>
                    u.Email.ToLower().Contains(term) ||
                    u.FirstName.ToLower().Contains(term) ||
                    u.LastName.ToLower().Contains(term))
                .Take(10)
                .Select(u => new UserSummaryResponse(
                    u.Id,
                    u.FirstName + " " + u.LastName,
                    u.Email))
                .ToListAsync(cancellationToken);
        }
    }
}