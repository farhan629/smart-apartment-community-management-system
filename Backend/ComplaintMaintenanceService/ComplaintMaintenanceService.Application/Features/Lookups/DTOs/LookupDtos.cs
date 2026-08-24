namespace ComplaintMaintenanceService.Application.Features.Lookups.DTOs;

public class RefTermLookupDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
}

public class CategoryLookupDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Img { get; set; } = string.Empty;
}
