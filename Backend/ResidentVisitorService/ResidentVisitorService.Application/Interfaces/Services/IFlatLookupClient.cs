namespace ResidentVisitorService.Application.Interfaces.Services;

/// <summary>
/// Abstraction over the gRPC call to IdentityService for flat and resident lookups.
/// </summary>
public interface IFlatLookupClient
{
    /// <summary>
    /// Resolves flat ID and host user ID from a block number and flat number.
    /// Used when Security registers a walk-in visit.
    /// </summary>
    Task<FlatInfoDto> GetFlatByBlockAndNumberAsync(
        string block,
        string flatNumber,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Resolves the flat occupied by the given resident user ID.
    /// Used when a Resident registers a planned visit.
    /// </summary>
    Task<FlatInfoDto> GetFlatByUserIdAsync(Guid userId, CancellationToken cancellationToken);
}

/// <summary>
/// Flat and resident information returned from IdentityService.
/// </summary>
public class FlatInfoDto
{
    public Guid FlatId { get; set; }
    public Guid HostUserId { get; set; }
    public string ResidentName { get; set; } = string.Empty;
    public string ResidentEmail { get; set; } = string.Empty;
}
