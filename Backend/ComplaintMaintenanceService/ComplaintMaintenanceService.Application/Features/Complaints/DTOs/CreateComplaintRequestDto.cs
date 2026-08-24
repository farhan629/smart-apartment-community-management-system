namespace ComplaintMaintenanceService.Application.Features.Complaints.DTOs;

public class CreateComplaintRequestDto
{
    public Guid ComplaintTypeRefId { get; set; }
    public Guid CategoryId { get; set; }
    public Guid PriorityRefId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string PreferredDate { get; set; } = string.Empty;
    public string? PreferredTime { get; set; }
}
