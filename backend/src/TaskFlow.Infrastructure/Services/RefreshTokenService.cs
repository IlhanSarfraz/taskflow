using System.Security.Cryptography;
using TaskFlow.Application.Common.Interfaces;

namespace TaskFlow.Infrastructure.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        public string Generate()
        {
            byte[] randomNumber = new byte[64];

            using RandomNumberGenerator rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);

            return Convert.ToBase64String(randomNumber);
        }
    }
}
