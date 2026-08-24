namespace ResidentVisitorService.Application.Features.Visitors.DTOs;

/// <summary>Request DTO for creating a new visitor.</summary>
public class CreateVisitorRequestDto
{
    public string Name { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string? Email { get; set; }

    public Guid VisitorTypeId { get; set; }
}
