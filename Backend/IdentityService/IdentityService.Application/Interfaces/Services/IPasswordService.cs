namespace IdentityService.Application.Interfaces.Services
{
    /// <summary>
    /// Service interface for password hashing, verification, and salt generation.
    /// </summary>
    public interface IPasswordService
    {
        /// <summary>
        /// Hashes a plain-text password using a secure algorithm.
        /// </summary>
        /// <param name="password">The plain-text password to hash.</param>
        /// <returns>The hashed password representation.</returns>
        string HashPassword(string password);

        /// <summary>
        /// Verifies a plain-text password against a hashed representation.
        /// </summary>
        /// <param name="password">The plain-text password to verify.</param>
        /// <param name="hash">The hashed password to compare against.</param>
        /// <returns>True if the password matches the hash, otherwise false.</returns>
        bool VerifyPassword(string password, string hash);

        /// <summary>
        /// Generates a unique, cryptographically secure salt.
        /// </summary>
        /// <returns>A string representation of the salt.</returns>
        string GenerateSalt();
    }
}