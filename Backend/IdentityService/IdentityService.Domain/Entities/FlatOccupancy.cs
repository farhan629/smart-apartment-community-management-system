using Shared.SharedLibrary.DTO;

namespace IdentityService.Domain.Entities;

/// <summary>
/// Records which user occupies which flat and in what capacity.
/// </summary>
public class FlatOccupancy : BaseEntity
{
    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    /// <remarks>Cross-service FK to IdentityService User</remarks>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the flat identifier.
    /// </summary>
    public Guid FlatId { get; set; }

    /// <summary>
    /// Gets or sets the resident type identifier.
    /// </summary>
    /// <remarks>RefTerm FK</remarks>
    public Guid ResidentTypeId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the occupancy request has been approved by admin.
    /// </summary>
    public bool IsApproved { get; set; }

    /// <summary>
    /// Gets or sets the flat associated with the occupancy.
    /// </summary>
    public virtual Flat? Flat { get; set; }

    /// <summary>
    /// Gets or sets the user associated with the occupancy.
    /// </summary>
    public virtual User? User { get; set; }

    /// <summary>
    /// Gets or sets the resident type reference term.
    /// </summary>
    public virtual RefTerm? ResidentType { get; set; }
}
