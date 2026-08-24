namespace ComplaintMaintenanceService.Application.Features.Complaints.DTOs;

public class ComplaintStatusUpdateRequestDto
{
    /// <summary>
    /// Must be one of <c>ComplaintConstants.StatusCodes.InProgress</c> or
    /// <c>ComplaintConstants.StatusCodes.Resolved</c>.
    /// </summary>
    public string Status { get; set; } = string.Empty;
}
