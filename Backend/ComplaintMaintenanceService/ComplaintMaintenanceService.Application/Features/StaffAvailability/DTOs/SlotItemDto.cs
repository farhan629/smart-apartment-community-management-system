namespace ComplaintMaintenanceService.Application.Features.StaffAvailability.DTOs;

/// <summary>
/// A single time slot entry inside the bulk availability create request.
/// </summary>
public class SlotItemDto
{
    public string Date { get; set; } = string.Empty;
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
}
