namespace ComplaintMaintenanceService.Application.Interfaces.Services;

/// <summary>
/// Abstraction for calling IdentityService via gRPC to retrieve flat/resident info.
/// </summary>
public interface IFlatLookupGrpcClient
{
    /// <summary>
    /// Gets the flat and resident info for the resident occupying a flat, by user ID.
    /// Returns null if no approved occupancy is found.
    /// </summary>
    Task<FlatInfoDto?> GetFlatByUserIdAsync(Guid userId, CancellationToken ct = default);
}

/// <summary>
/// Lightweight flat/resident info returned from IdentityService gRPC.
/// </summary>
public record FlatInfoDto(
    Guid FlatId,
    Guid HostUserId,
    string ResidentName,
    string ResidentEmail,
    string Block,
    string FlatNumber
);