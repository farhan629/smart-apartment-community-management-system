using Shared.SharedLibrary.DTO;

namespace ComplaintMaintenanceService.Domain.Entities;

/// <summary>
/// Represents a category for complaints and staff specialization.
/// </summary>
public class Category : BaseEntity
{
    /// <summary>
    /// Gets or sets the name of the category.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the category.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the image of the category.
    /// </summary>
    public string Img { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the collection of staff members associated with the category.
    /// </summary>
    public virtual ICollection<Staff>? Staff { get; set; }

    /// <summary>
    /// Gets or sets the collection of complaints associated with the category.
    /// </summary>
    public virtual ICollection<Complaint>? Complaints { get; set; }

    /// <summary>
    /// Gets or sets the collection of auto-assignment rules associated with the category.
    /// </summary>
    public virtual ICollection<AutoAssignmentRule>? AutoAssignmentRules { get; set; }
}
