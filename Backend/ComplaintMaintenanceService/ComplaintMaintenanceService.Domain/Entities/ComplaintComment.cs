using Shared.SharedLibrary.DTO;

namespace ComplaintMaintenanceService.Domain.Entities;

/// <summary>
/// Represents comments and resident ratings on a complaint.
/// </summary>
public class ComplaintComment : BaseEntity
{
    /// <summary>
    /// Gets or sets the complaint identifier.
    /// </summary>
    public Guid ComplaintId { get; set; }

    /// <summary>
    /// Gets or sets the user identifier who commented.
    /// </summary>
    /// <remarks>Cross-service FK to IdentityService User</remarks>
    public Guid CommentedBy { get; set; }

    /// <summary>
    /// Gets or sets the comment text.
    /// </summary>
    public string CommentText { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the rating given by the resident to the staff.
    /// </summary>
    /// <remarks>Rating scale of 1-5. Null until rated.</remarks>
    public int? StaffRating { get; set; }

    /// <summary>
    /// Gets or sets the complaint associated with the comment.
    /// </summary>
    public virtual Complaint? Complaint { get; set; }
}
