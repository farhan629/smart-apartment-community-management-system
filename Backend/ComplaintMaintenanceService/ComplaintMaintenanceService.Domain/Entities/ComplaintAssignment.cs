using Shared.SharedLibrary.DTO;

namespace ComplaintMaintenanceService.Domain.Entities;

/// <summary>
/// Represents an assignment of a complaint to a staff member.
/// </summary>
public class ComplaintAssignment : BaseEntity
{
    /// <summary>
    /// Gets or sets the complaint identifier.
    /// </summary>
    public Guid ComplaintId { get; set; }

    /// <summary>
    /// Gets or sets the staff identifier.
    /// </summary>
    public Guid StaffId { get; set; }

    /// <summary>
    /// Gets or sets the user identifier who made the assignment.
    /// </summary>
    /// <remarks>Cross-service FK to IdentityService User</remarks>
    public Guid AssignedBy { get; set; }

    /// <summary>
    /// Gets or sets the status identifier.
    /// </summary>
    /// <remarks>RefTerm FK</remarks>
    public Guid StatusId { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the assignment was made.
    /// </summary>
    public DateTime AssignedDate { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the assignment was accepted.
    /// </summary>
    public DateTime? AcceptedDate { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the assignment was denied.
    /// </summary>
    public DateTime? DeniedDate { get; set; }

    /// <summary>
    /// Gets or sets the reason for denial.
    /// </summary>
    public string? DenialReason { get; set; }

    /// <summary>
    /// Gets or sets the due date of the assignment.
    /// </summary>
    public DateTime DueDate { get; set; }

    /// <summary>
    /// Gets or sets the complaint associated with the assignment.
    /// </summary>
    public virtual Complaint? Complaint { get; set; }

    /// <summary>
    /// Gets or sets the staff member associated with the assignment.
    /// </summary>
    public virtual Staff? Staff { get; set; }

    /// <summary>
    /// Gets or sets the status reference term.
    /// </summary>
    public virtual RefTerm? Status { get; set; }
}
