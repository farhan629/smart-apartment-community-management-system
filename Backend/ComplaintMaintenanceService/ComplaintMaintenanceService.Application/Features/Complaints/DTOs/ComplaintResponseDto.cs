namespace ComplaintMaintenanceService.Application.Features.Complaints.DTOs;

public class ComplaintResponseDto
{
    public Guid ComplaintId { get; set; }
    public Guid ResidentId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string ComplaintType { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public string? CategoryImg { get; set; }
    public string Priority { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? ScheduledDate { get; set; }
    public string? ScheduledTime { get; set; }
    public DateTime CreatedAt { get; set; }
}
