namespace ComplaintMaintenanceService.Application.Features.StaffAvailability.DTOs;

/// <summary>
/// Full availability slot response matching the AvailabilitySlot OpenAPI schema.
/// </summary>
public class AvailabilitySlotResponseDto
{
    public Guid SlotId { get; set; }
    public Guid StaffId { get; set; }
    public string StaffName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public string Date { get; set; } = string.Empty;
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public bool IsBooked { get; set; }
    public bool IsCancelled { get; set; }
    public Guid? ComplaintId { get; set; }
}
