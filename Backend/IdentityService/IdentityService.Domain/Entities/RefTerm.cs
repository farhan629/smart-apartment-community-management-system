using Shared.SharedLibrary.DTO;

namespace IdentityService.Domain.Entities;

/// <summary>
/// Represents a reference term within a reference set.
/// </summary>
public class RefTerm : BaseEntity
{
    /// <summary>
    /// Gets or sets the reference set identifier.
    /// </summary>
    public Guid RefSetId { get; set; }

    /// <summary>
    /// Gets or sets the code of the reference term.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display name of the reference term.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the reference set associated with the reference term.
    /// </summary>
    public virtual RefSet? RefSet { get; set; }

    /// <summary>
    /// Gets or sets the collection of users assigned to this role.
    /// </summary>
    public virtual ICollection<User>? Users { get; set; }

    /// <summary>
    /// Gets or sets the collection of role policies linked to this role.
    /// </summary>
    public virtual ICollection<RolePolicy>? RolePolicies { get; set; }

    /// <summary>
    /// Gets or sets the collection of flat occupancies associated with the reference term.
    /// </summary>
    public virtual ICollection<FlatOccupancy>? FlatOccupancies { get; set; }
}
