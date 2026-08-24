using IdentityService.Application.Interfaces.Services;
using Shared.SharedLibrary;
using Shared.SharedLibrary.Constants;

namespace IdentityService.Infrastructure.Services
{
    /// <summary>
    /// Service implementation for secure password hashing and verification using BCrypt.
    /// </summary>
    public class PasswordService : IPasswordService
    {
        private const int WorkFactor = PasswordConstants.WorkFactor;

        public string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException(
                    ExceptionMessages.PasswordCannotBeEmpty,
                    nameof(password)
                );

            return BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);
        }

        public bool VerifyPassword(string password, string hash)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hash))
                return false;

            return BCrypt.Net.BCrypt.Verify(password, hash);
        }

        public string GenerateSalt()
        {
            return BCrypt.Net.BCrypt.GenerateSalt(WorkFactor);
        }
    }
}