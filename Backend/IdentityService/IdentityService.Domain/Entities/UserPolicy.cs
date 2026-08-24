using Shared.SharedLibrary.DTO;

namespace IdentityService.Domain.Entities;

/// <summary>
/// Represents a custom permission override for a specific user.
/// </summary>
/// <remarks>Overrides RolePolicy when present.</remarks>
public class UserPolicy : BaseEntity
{
    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the permission code.
    /// </summary>
    public string PermissionCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the permission is allowed.
    /// </summary>
    public bool IsAllowed { get; set; }

    /// <summary>
    /// Gets or sets the user associated with the policy.
    /// </summary>
    public virtual User? User { get; set; }
}
