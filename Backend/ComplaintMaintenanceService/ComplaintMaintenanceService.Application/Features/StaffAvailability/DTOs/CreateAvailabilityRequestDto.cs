namespace ComplaintMaintenanceService.Application.Features.StaffAvailability.DTOs;

/// <summary> 
/// staffId is supplied as a query parameter, not in this body.
/// </summary>
public class CreateAvailabilityRequestDto
{
    public List<SlotItemDto> Slots { get; set; } = new();
}
