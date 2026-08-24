using System.ComponentModel.DataAnnotations;

namespace ResidentVisitorService.Application.Features.Visitors.DTOs;

/// <summary>Response DTO for a visitor record.</summary>
public class VisitorResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Email { get; set; }
    public Guid VisitorTypeId { get; set; }
    public string? PhotoUrl { get; set; }
    public string VisitorType { get; set; } = string.Empty;
}
