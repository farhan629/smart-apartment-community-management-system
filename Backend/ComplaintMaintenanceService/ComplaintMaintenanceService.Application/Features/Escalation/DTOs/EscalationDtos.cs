namespace ComplaintMaintenanceService.Application.Features.Escalation.DTOs;

public class EscalationResponseDto
{
    public Guid EscalationId { get; set; }
    public Guid ComplaintId { get; set; }
    public Guid EscalatedBy { get; set; }
    public Guid EscalatedTo { get; set; }
    public string EscalationReason { get; set; } = string.Empty;
    public DateTime EscalationDate { get; set; }
    public bool ResolvedAfterEscalation { get; set; }
    public DateTime? ResolutionDate { get; set; }
}

public class UpdateEscalationRequestDto
{
    public bool ResolvedAfterEscalation { get; set; }
    public DateTime? ResolutionDate { get; set; }
}

public class ReEscalateRequestDto
{
    public string EscalationReason { get; set; } = string.Empty;
}

public class ReEscalateResponseDto
{
    public Guid EscalationId { get; set; }
    public Guid ComplaintId { get; set; }
    public string EscalationReason { get; set; } = string.Empty;
    public DateTime EscalationDate { get; set; }
}
