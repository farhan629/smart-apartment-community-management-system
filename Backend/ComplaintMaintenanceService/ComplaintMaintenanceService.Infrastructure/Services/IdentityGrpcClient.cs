using ComplaintMaintenanceService.Application.Interfaces.Services;
using Grpc.Net.Client;
using IdentityService.Infrastructure.Protos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ComplaintMaintenanceService.Infrastructure.Services;

/// <summary>
/// gRPC client implementation for calling IdentityService to retrieve user info.
/// </summary>
public class IdentityGrpcClient : IIdentityGrpcClient
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<IdentityGrpcClient> _logger;

    public IdentityGrpcClient(IConfiguration configuration, ILogger<IdentityGrpcClient> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<UserInfoDto?> GetUserByIdAsync(Guid userId, CancellationToken ct = default)
    {
        var url =
            _configuration[CmsGrpcConfigKeys.IdentityServiceUrl]
            ?? throw new InvalidOperationException(CmsGrpcConfigKeys.IdentityServiceUrlMissing);

        using var channel = GrpcChannel.ForAddress(url);
        var client = new IdentityGrpc.IdentityGrpcClient(channel);

        var response = await client.GetUserByIdAsync(
            new GetUserByIdRequest { UserId = userId.ToString() },
            cancellationToken: ct
        );

        if (!response.Found)
        {
            _logger.LogWarning("IdentityGrpcClient - user not found: {UserId}", userId);
            return null;
        }

        return new UserInfoDto(
            Guid.Parse(response.UserId),
            response.Email,
            response.Name,
            response.RoleCode
        );
    }

    public async Task<List<UserInfoDto>> GetUsersByRoleAsync(
        string roleCode,
        CancellationToken ct = default
    )
    {
        var url =
            _configuration[CmsGrpcConfigKeys.IdentityServiceUrl]
            ?? throw new InvalidOperationException(CmsGrpcConfigKeys.IdentityServiceUrlMissing);

        using var channel = GrpcChannel.ForAddress(url);
        var client = new IdentityGrpc.IdentityGrpcClient(channel);

        var response = await client.GetUsersByRoleAsync(
            new GetUsersByRoleRequest { RoleCode = roleCode },
            cancellationToken: ct
        );

        return response
            .Users.Select(u => new UserInfoDto(Guid.Parse(u.UserId), u.Email, u.Name, roleCode))
            .ToList();
    }
}

/// <summary>
/// Config key constants for CMS gRPC clients.
/// </summary>
public static class CmsGrpcConfigKeys
{
    public const string IdentityServiceUrl = "GrpcSettings:IdentityServiceUrl";
    public const string NotificationServiceUrl = "GrpcSettings:NotificationServiceUrl";
    public const string IdentityServiceUrlMissing =
        "GrpcSettings:IdentityServiceUrl is not configured.";
    public const string NotificationServiceUrlMissing =
        "GrpcSettings:NotificationServiceUrl is not configured.";
}
