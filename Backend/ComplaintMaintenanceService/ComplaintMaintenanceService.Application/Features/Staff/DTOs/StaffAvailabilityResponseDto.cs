namespace ComplaintMaintenanceService.Application.Features.Staff.DTOs;

/// <summary>
/// Response DTO for a staff availability slot returned by the Staff Availability endpoints.
/// </summary>
public class StaffAvailabilityResponseDto
{
    public Guid SlotId { get; set; }
    public Guid StaffId { get; set; }
    public Guid? ComplaintId { get; set; }
    public string AvailableDate { get; set; } = string.Empty;
    public string SlotStartTime { get; set; } = string.Empty;
    public string SlotEndTime { get; set; } = string.Empty;
    public bool IsBooked { get; set; }
    public bool IsCancelled { get; set; }
    public bool IsActive { get; set; }
}
