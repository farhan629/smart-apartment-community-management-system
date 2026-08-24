namespace ResidentVisitorService.Application.Features.Visitors.DTOs;

/// <summary>Request DTO for updating an existing visitor.</summary>
public class UpdateVisitorRequestDto
{
    public string? Name { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }

    public Guid? VisitorTypeId { get; set; }
}
