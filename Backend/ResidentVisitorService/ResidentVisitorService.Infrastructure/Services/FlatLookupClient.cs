using ResidentVisitorService.Application.Interfaces.Services;
using Shared.Grpc;

namespace ResidentVisitorService.Infrastructure.Services;

/// <summary>
/// gRPC client wrapper that calls IdentityService to resolve flat and resident info.
/// </summary>
public class FlatLookupClient : IFlatLookupClient
{
    private readonly FlatLookupGrpcService.FlatLookupGrpcServiceClient _client;

    public FlatLookupClient(FlatLookupGrpcService.FlatLookupGrpcServiceClient client) =>
        _client = client;

    /// <inheritdoc />
    public async Task<FlatInfoDto> GetFlatByBlockAndNumberAsync(
        string block,
        string flatNumber,
        CancellationToken cancellationToken
    )
    {
        var response = await _client.GetFlatByBlockAndNumberAsync(
            new FlatLookupRequest { Block = block, FlatNumber = flatNumber },
            cancellationToken: cancellationToken
        );

        return Map(response);
    }

    /// <inheritdoc />
    public async Task<FlatInfoDto> GetFlatByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken
    )
    {
        var response = await _client.GetFlatByUserIdAsync(
            new FlatByUserIdRequest { UserId = userId.ToString() },
            cancellationToken: cancellationToken
        );

        return Map(response);
    }

    private static FlatInfoDto Map(FlatLookupResponse r) =>
        new()
        {
            FlatId = Guid.Parse(r.FlatId),
            HostUserId = Guid.Parse(r.HostUserId),
            ResidentName = r.ResidentName,
            ResidentEmail = r.ResidentEmail,
        };
}
