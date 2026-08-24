using Shared.SharedLibrary.DTO;

namespace IdentityService.Domain.Entities;

/// <summary>
/// Stores hashed passwords for a user.
/// </summary>
/// <remarks>Supports history for reuse prevention. Plain text is NEVER stored.</remarks>
public class UserPasswordSecurity : BaseEntity
{
    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the hashed password.
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user associated with the password security.
    /// </summary>
    public virtual User? User { get; set; }
}
