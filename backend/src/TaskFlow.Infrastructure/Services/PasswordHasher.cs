using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using TaskFlow.Application.Common.Interfaces;

namespace TaskFlow.Infrastructure.Services
{
    public class PasswordHasher : IPasswordHasher
    {
        public string Hash(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(16);

            string hash = Convert.ToBase64String(
                KeyDerivation.Pbkdf2(
                    password,
                    salt,
                    KeyDerivationPrf.HMACSHA256,
                    10000,
                    32));

            return $"{Convert.ToBase64String(salt)}.{hash}";
        }

        public bool Verify(string password, string hashedPassword)
        {
            string[] parts = hashedPassword.Split('.');
            byte[] salt = Convert.FromBase64String(parts[0]);

            string hash = Convert.ToBase64String(
                KeyDerivation.Pbkdf2(
                    password,
                    salt,
                    KeyDerivationPrf.HMACSHA256,
                    10000,
                    32));

            return hash == parts[1];
        }
    }
}
