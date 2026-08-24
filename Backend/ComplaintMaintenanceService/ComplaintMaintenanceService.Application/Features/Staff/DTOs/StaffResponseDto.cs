namespace ComplaintMaintenanceService.Application.Features.Staff.DTOs;

/// <summary>
/// Full Staff profile returned by single-record GET and PATCH responses.
/// </summary>
public class StaffResponseDto
{
    public Guid StaffId { get; set; }
    public Guid UserId { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
