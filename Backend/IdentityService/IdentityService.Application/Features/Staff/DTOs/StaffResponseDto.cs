namespace IdentityService.Application.Features.Staff.DTOs;

/// <summary>
/// Response DTO returned after successful staff creation.
/// </summary>
public record StaffResponseDto(
    Guid StaffId,
    Guid UserId,
    string Name,
    string Email,
    string Phone,
    Guid CategoryId,
    string CategoryName,
    string? Description,
    string? Details
);
