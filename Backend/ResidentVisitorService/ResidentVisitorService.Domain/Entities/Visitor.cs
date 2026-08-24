using Shared.SharedLibrary.DTO;

namespace ResidentVisitorService.Domain.Entities;

/// <summary>
/// Represents a visitor coming to the community.
/// </summary>
public class Visitor : BaseEntity
{
    /// <summary>
    /// Gets or sets the visitor type identifier.
    /// </summary>
    /// <remarks>RefTerm FK</remarks>
    public Guid VisitorTypeId { get; set; }

    /// <summary>
    /// Gets or sets the name of the visitor.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the phone number of the visitor.
    /// </summary>
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the email of the visitor.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets the photo URL of the visitor.
    /// </summary>
    public string? PhotoUrl { get; set; }

    /// <summary>
    /// Gets or sets the visitor type reference term.
    /// </summary>
    public virtual RefTerm? VisitorType { get; set; }

    /// <summary>
    /// Gets or sets the collection of visits associated with the visitor.
    /// </summary>
    public virtual ICollection<Visit>? Visits { get; set; }
}
