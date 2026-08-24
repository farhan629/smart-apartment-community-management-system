namespace ComplaintMaintenanceService.Application.Features.BackgroundJobs.DTOs;

public class RunEscalationCheckResultDto
{
    public int EscalatedCount { get; set; }
    public List<Guid> EscalatedComplaintIds { get; set; } = new();
}
