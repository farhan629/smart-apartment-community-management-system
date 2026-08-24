using AmenityBookingService.Application.Interfaces.Services;
using IdentityService.Infrastructure.Protos;

namespace AmenityBookingService.API.Services;

/// <summary>
/// Provides a gRPC client implementation for communicating with the
/// Identity Service to retrieve user-related information.
/// </summary>
public class IdentityGrpcClient : IIdentityClient
{
    private readonly IdentityGrpc.IdentityGrpcClient _client;
    private readonly ILogger<IdentityGrpcClient> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="IdentityGrpcClient"/> class.
    /// </summary>
    /// <param name="client">
    /// The gRPC client used to communicate with the Identity Service.
    /// </param>
    /// <param name="logger">
    /// The logger used to record service operations and errors.
    /// </param>
    public IdentityGrpcClient(
        IdentityGrpc.IdentityGrpcClient client,
        ILogger<IdentityGrpcClient> logger
    )
    {
        _client = client;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves all users associated with the specified role from the
    /// Identity Service.
    /// </summary>
    /// <param name="roleId">
    /// The unique identifier of the role whose users are to be retrieved.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A list of users belonging to the specified role. Returns an empty list
    /// if the request fails.
    /// </returns>
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
