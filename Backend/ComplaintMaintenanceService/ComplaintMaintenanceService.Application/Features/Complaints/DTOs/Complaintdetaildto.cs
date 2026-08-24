namespace ComplaintMaintenanceService.Application.Features.Complaints.DTOs;

/// <summary>
/// Full detail of a single complaint — superset of <see cref="ComplaintSummaryDto"/>,
/// returned by GET /complaints/{complaintId} and by the status-update/cancel commands.
/// </summary>
public class ComplaintDetailDto : ComplaintSummaryDto
{
    public Guid? ScheduledSlotId { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancellationReason { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
