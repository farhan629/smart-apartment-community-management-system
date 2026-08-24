using Shared.SharedLibrary.DTO;

namespace ResidentVisitorService.Domain.Entities;

/// <summary>
/// Represents a QR token generated for a visit.
/// </summary>
/// <remarks>One-to-one relationship with Visit.</remarks>
public class VisitQrToken : BaseEntity
{
    /// <summary>
    /// Gets or sets the visit identifier.
    /// </summary>
    public Guid VisitId { get; set; }

    /// <summary>
    /// Gets or sets the unique opaque token string embedded in the QR code.
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the visit associated with the QR token.
    /// </summary>
    public virtual Visit? Visit { get; set; }
}
