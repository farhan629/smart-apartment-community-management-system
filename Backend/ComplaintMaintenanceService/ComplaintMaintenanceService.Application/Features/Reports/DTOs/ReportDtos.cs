namespace ComplaintMaintenanceService.Application.Features.Reports.DTOs;

public class ReportResponseDto
{
    public int TotalComplaints { get; set; }
    public int OpenComplaints { get; set; }
    public int AssignedComplaints { get; set; }
    public int InProgressComplaints { get; set; }
    public int ResolvedComplaints { get; set; }
    public int CancelledComplaints { get; set; }
    public int EscalatedComplaints { get; set; }
    public int TotalStaff { get; set; }
    public int TotalEscalations { get; set; }
    public List<CategoryReportDto> ByCategory { get; set; } = new();
}

public class CategoryReportDto
{
    public string CategoryName { get; set; } = string.Empty;
    public int ComplaintCount { get; set; }
}
