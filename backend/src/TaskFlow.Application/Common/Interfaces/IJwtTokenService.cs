using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Common.Interfaces;

public interface IJwtTokenService
{
    string GeneratedToken(User user);
}