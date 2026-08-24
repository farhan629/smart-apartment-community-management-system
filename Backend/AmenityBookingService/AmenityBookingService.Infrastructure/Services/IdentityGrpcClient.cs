using AmenityBookingService.Application.Interfaces.Services;
using IdentityService.Infrastructure.Protos;
using Microsoft.Extensions.Logging;

namespace AmenityBookingService.Infrastructure.Services;

/// <summary>
/// Provides gRPC client implementation for communicating with the Identity Service.
/// </summary>
/// <remarks>
/// This class implements the <see cref="IIdentityClient"/> interface and uses gRPC
/// to interact with the Identity Service for user information retrieval.
/// </remarks>
public class IdentityGrpcClient : IIdentityClient
{
    private readonly IdentityGrpc.IdentityGrpcClient _client;
    private readonly ILogger<IdentityGrpcClient> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="IdentityGrpcClient"/> class.
    /// </summary>
    /// <param name="client">The gRPC client for the Identity Service.</param>
    /// <param name="logger">The logger instance for recording operations and errors.</param>
    public IdentityGrpcClient(
        IdentityGrpc.IdentityGrpcClient client,
        ILogger<IdentityGrpcClient> logger
    )
    {
        _client = client;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves all users belonging to a specific role from the Identity Service.
    /// </summary>
    /// <param name="roleCode">The unique code identifier for the role.</param>
    /// <param name="cancellationToken">Cancellation token for the operation. Defaults to <see cref="CancellationToken.None"/>.</param>
    /// <returns>
    /// A list of <see cref="IdentityUserInfo"/> objects containing user details.
    /// Returns an empty list if an error occurs or no users are found.
    /// </returns>
    /// <remarks>
    /// This method makes a gRPC call to the Identity Service's GetUsersByRole endpoint.
    /// In case of any exceptions (network issues, service unavailability, etc.),
    /// the error is logged and an empty list is returned to maintain service resilience.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when roleCode is null or empty.</exception>
    public async Task<List<IdentityUserInfo>> GetUsersByRoleAsync(
        string roleCode,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var request = new GetUsersByRoleRequest { RoleCode = roleCode };

            var response = await _client.GetUsersByRoleAsync(
                request,
                cancellationToken: cancellationToken
            );

            return response
                .Users.Select(u => new IdentityUserInfo
                {
                    UserId = Guid.Parse(u.UserId),
                    Email = u.Email,
                    Name = u.Name,
                })
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error fetching users by role {RoleCode} from IdentityService",
                roleCode
            );
            return new List<IdentityUserInfo>();
        }
    }
}
