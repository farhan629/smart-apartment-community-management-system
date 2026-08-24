using Shared.SharedLibrary.DTO;

namespace IdentityService.Domain.Entities;

/// <summary>
/// Represents a flat or unit in the community.
/// </summary>
public class Flat : BaseEntity
{
    /// <summary>
    /// Gets or sets the number of the flat.
    /// </summary>
    public string Number { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the block of the flat.
    /// </summary>
    public string Block { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the floor number of the flat.
    /// </summary>
    public int Floor { get; set; }

    /// <summary>
    /// Gets or sets the collection of flat occupancies associated with the flat.
    /// </summary>
    public virtual ICollection<FlatOccupancy>? FlatOccupancies { get; set; }
}
