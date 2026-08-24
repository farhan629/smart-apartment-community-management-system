namespace ResidentVisitorService.Application.Features.VisitQrToken.DTOs;

/// <summary>Response DTO for a generated QR token.</summary>
public class VisitQrTokenResponseDto
{
    public Guid Id { get; set; }
    public Guid VisitId { get; set; }
    public string Token { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>Full visit details returned when validating a QR token at the gate.</summary>
public class QrTokenValidationResponseDto
{
    public Guid VisitId { get; set; }
    public string VisitorName { get; set; } = string.Empty;
    public string VisitorPhoneNumber { get; set; } = string.Empty;
    public string VisitorType { get; set; } = string.Empty;
    public Guid FlatId { get; set; }
    public string Purpose { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public bool IsTokenActive { get; set; }
}

/// <summary>DTO for RefTerm lookup items (visitor types, purpose types).</summary>
public class RefTermDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
