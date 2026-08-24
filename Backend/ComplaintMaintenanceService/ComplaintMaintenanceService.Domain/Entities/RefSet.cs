using Shared.SharedLibrary.DTO;

namespace ComplaintMaintenanceService.Domain.Entities;

/// <summary>
/// Represents a reference set for the complaint maintenance service.
/// </summary>
public class RefSet : BaseEntity
{
    /// <summary>
    /// Gets or sets the code of the reference set.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the reference set.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the collection of reference terms associated with the reference set.
    /// </summary>
    public virtual ICollection<RefTerm>? RefTerms { get; set; }
}
