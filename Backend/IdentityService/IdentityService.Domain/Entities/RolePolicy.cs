using Shared.SharedLibrary.DTO;

namespace IdentityService.Domain.Entities;

/// <summary>
/// Represents a permission attached to a role that all users with that role inherit.
/// </summary>
public class RolePolicy : BaseEntity
{
    /// <summary>
    /// Gets or sets the role identifier.
    /// </summary>
    public Guid RoleId { get; set; }

    /// <summary>
    /// Gets or sets the permission code.
    /// </summary>
    public string PermissionCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the permission.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the permission is allowed.
    /// </summary>
    public bool IsAllowed { get; set; }

    /// <summary>
    /// Gets or sets the role associated with the policy.
    /// </summary>
    public virtual RefTerm? Role { get; set; }
}
