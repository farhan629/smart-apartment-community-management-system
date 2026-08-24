using ComplaintMaintenanceService.Application.Common.Constants;
using ComplaintMaintenanceService.Application.Interfaces.Services;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shared.Grpc;

namespace ComplaintMaintenanceService.Infrastructure.Services;

/// <summary>
/// gRPC client implementation for calling FlatLookupService to retrieve flat/resident info.
/// </summary>
public class FlatLookupGrpcClient : IFlatLookupGrpcClient
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<FlatLookupGrpcClient> _logger;

    public FlatLookupGrpcClient(IConfiguration configuration, ILogger<FlatLookupGrpcClient> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<FlatInfoDto?> GetFlatByUserIdAsync(
        Guid userId,
        CancellationToken ct = default
    )
    {
        var url =
            _configuration[GrpcConfigKeys.FlatLookupServiceUrl]
            ?? throw new InvalidOperationException(GrpcConfigKeys.FlatLookupServiceUrlMissing);

        using var channel = GrpcChannel.ForAddress(url);
        var client = new FlatLookupGrpcService.FlatLookupGrpcServiceClient(channel);

        try
        {
            var response = await client.GetFlatByUserIdAsync(
                new FlatByUserIdRequest { UserId = userId.ToString() },
                cancellationToken: ct
            );

            return new FlatInfoDto(
                Guid.Parse(response.FlatId),
                Guid.Parse(response.HostUserId),
                response.ResidentName,
                response.ResidentEmail,
                response.Block,
                response.FlatNumber
            );
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
        {
            _logger.LogWarning(
                "FlatLookupGrpcClient - no approved flat found for user {UserId}",
                userId
            );
            return null;
        }
    }
}
