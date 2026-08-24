namespace ComplaintMaintenanceService.Application.Features.Assignments.DTOs;

public class AssignComplaintRequestDto
{
    public Guid StaffId { get; set; }
    public DateTime DueDate { get; set; }
}

public class DenyAssignmentRequestDto
{
    public string DenialReason { get; set; } = string.Empty;
}

public class AssignmentResponseDto
{
    public Guid AssignmentId { get; set; }
    public Guid ComplaintId { get; set; }
    public Guid StaffId { get; set; }
    public string StaffName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime AssignedDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? AcceptedDate { get; set; }
    public DateTime? DeniedDate { get; set; }
    public string? DenialReason { get; set; }
    public Guid AssignedBy { get; set; }
}
