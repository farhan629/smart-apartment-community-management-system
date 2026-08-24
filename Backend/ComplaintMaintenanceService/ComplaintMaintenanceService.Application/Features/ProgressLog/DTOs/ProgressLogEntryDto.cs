namespace ComplaintMaintenanceService.Application.Features.ProgressLog.DTOs;

public class ProgressLogEntryDto
{
    public Guid LogId { get; set; }
    public Guid ComplaintId { get; set; }
    public Guid ChangedBy { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Remarks { get; set; }
    public DateTime ChangedDate { get; set; }
}
