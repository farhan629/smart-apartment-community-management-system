namespace ComplaintMaintenanceService.Application.Features.Staff.DTOs;

/// <summary>
/// Lightweight projection of a Staff record returned in paged list responses.
/// </summary>
public class StaffSummaryDto
{
    public Guid StaffId { get; set; }
    public Guid UserId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
