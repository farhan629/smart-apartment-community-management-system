using Shared.SharedLibrary.DTO;

namespace ResidentVisitorService.Domain.Entities;

/// <summary>
/// Represents a visit to a resident flat by a visitor.
/// </summary>
public class Visit : BaseEntity
{
    /// <summary>
    /// Gets or sets the visitor identifier.
    /// </summary>
    public Guid VisitorId { get; set; }

    /// <summary>
    /// Gets or sets the host user identifier.
    /// </summary>
    /// <remarks>Cross-service FK to IdentityService User</remarks>
    public Guid HostUserId { get; set; }

    /// <summary>
    /// Gets or sets the flat identifier.
    /// </summary>
    public Guid FlatId { get; set; }

    /// <summary>
    /// Gets or sets the purpose type identifier.
    /// </summary>
    /// <remarks>RefTerm FK</remarks>
    public Guid PurposeTypeId { get; set; }

    /// <summary>
    /// Gets or sets the status identifier.
    /// </summary>
    /// <remarks>RefTerm FK</remarks>
    public Guid StatusId { get; set; }

    /// <summary>
    /// Gets or sets the start date and time of the visit.
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Gets or sets the end date and time of the visit.
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Gets or sets the check-in time of the visit.
    /// </summary>
    public DateTime? CheckInTime { get; set; }

    /// <summary>
    /// Gets or sets the check-out time of the visit.
    /// </summary>
    public DateTime? CheckOutTime { get; set; }

    /// <summary>
    /// Gets or sets the user identifier who approved the visit.
    /// </summary>
    /// <remarks>Cross-service FK to IdentityService User</remarks>
    public Guid? ApprovedBy { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the visit was approved.
    /// </summary>
    public DateTime? ApprovedDate { get; set; }

    /// <summary>
    /// Gets or sets the reason for rejection.
    /// </summary>
    public string? RejectionReason { get; set; }

    /// <summary>
    /// Gets or sets the visitor associated with the visit.
    /// </summary>
    public virtual Visitor? Visitor { get; set; }

    /// <summary>
    /// Gets or sets the purpose type reference term.
    /// </summary>
    public virtual RefTerm? PurposeType { get; set; }

    /// <summary>
    /// Gets or sets the status reference term.
    /// </summary>
    public virtual RefTerm? Status { get; set; }

    /// <summary>
    /// Gets or sets the QR token associated with the visit.
    /// </summary>
    public virtual VisitQrToken? VisitQrToken { get; set; }
}
