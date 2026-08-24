namespace ComplaintMaintenanceService.Application.Features.Assignments.DTOs;

public record ResidentFlatResponseDto(
    Guid FlatId,
    string ResidentName,
    string ResidentEmail,
    string Block,
    string FlatNumber
);
