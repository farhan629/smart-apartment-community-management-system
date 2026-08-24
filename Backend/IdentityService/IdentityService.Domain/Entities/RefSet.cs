using Shared.SharedLibrary.DTO;

namespace IdentityService.Domain.Entities;

/// <summary>
/// Represents a reference set that groups related reference terms.
/// </summary>
public class RefSet : BaseEntity
{
    /// <summary>
    /// Gets or sets the unique code for the reference set.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the human-readable description of the reference set.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the collection of reference terms associated with the reference set.
    /// </summary>
    public virtual ICollection<RefTerm>? RefTerms { get; set; }
}
