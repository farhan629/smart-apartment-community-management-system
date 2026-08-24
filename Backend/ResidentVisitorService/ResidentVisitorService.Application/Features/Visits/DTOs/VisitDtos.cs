using ResidentVisitorService.Application.Features.Visitors.DTOs;

namespace ResidentVisitorService.Application.Features.Visits.DTOs;

/// <summary>Response DTO for a visit record.</summary>
public class VisitResponseDto
{
    public Guid Id { get; set; }
    public Guid VisitorId { get; set; }
    public string VisitorName { get; set; } = string.Empty;
    public string VisitorPhoneNumber { get; set; } = string.Empty;
    public string? VisitorEmail { get; set; }
    public string VisitorType { get; set; } = string.Empty;
    public Guid HostUserId { get; set; }
    public Guid FlatId { get; set; }
    public Guid PurposeTypeId { get; set; }
    public string Purpose { get; set; } = string.Empty;
    public Guid StatusId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public DateTime? CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public Guid? ApprovedBy { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public string? RejectionReason { get; set; }
    public VisitQrTokenEmbedDto? QrToken { get; set; }
}

/// <summary>Embedded QR token info within a visit response.</summary>
public class VisitQrTokenEmbedDto
{
    public string Token { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

/// <summary>Slim response DTO returned after creating a visit.</summary>
public class VisitCreateResponseDto
{
    public Guid Id { get; set; }
    public string Status { get; set; } = string.Empty;
    public string VisitorName { get; set; } = string.Empty;
    public Guid FlatId { get; set; }
    public string Purpose { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
}

/// <summary>Slim response after approving or rejecting a visit.</summary>
public class ApproveRejectVisitResponseDto
{
    public Guid Id { get; set; }
    public string Status { get; set; } = string.Empty;
}

/// <summary>Slim response after check-in or check-out.</summary>
public class CheckInOutResponseDto
{
    public Guid Id { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
}

/// <summary>Request DTO for registering a new visit.</summary>
public class CreateVisitRequestDto
{
    /// <summary>Existing visitor ID — provide this OR the inline visitor object.</summary>
    public Guid? VisitorId { get; set; }

    /// <summary>Inline visitor data for creating a visitor on the fly.</summary>
    public CreateVisitorRequestDto? Visitor { get; set; }

    public Guid PurposeTypeId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    /// <summary>
    /// Required when Security registers a walk-in visit.
    /// Residents leave these null — their flat is resolved from their JWT automatically.
    /// </summary>
    public string? BlockNumber { get; set; }
    public string? FlatNumber { get; set; }
}

/// <summary>Request DTO for updating a pending visit.</summary>
public class UpdateVisitRequestDto
{
    public Guid? PurposeTypeId { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
}

/// <summary>Request DTO for rejecting a visit.</summary>
public class RejectVisitRequestDto
{
    public string RejectionReason { get; set; } = string.Empty;
}
