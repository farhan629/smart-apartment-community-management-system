using Grpc.Core;
using IdentityService.Application.Interfaces.Repositories;
using IdentityService.Infrastructure.Protos;
using Microsoft.Extensions.Logging;

namespace IdentityService.API.Grpc;

/// <summary>
/// gRPC server implementation exposing user lookup to other microservices.
/// </summary>
///
public class IdentityGrpcService : IdentityGrpc.IdentityGrpcBase
{
    private readonly IUserRepository _userRepository;

    private readonly ILogger<IdentityGrpcService> _logger;

    public IdentityGrpcService(IUserRepository userRepository, ILogger<IdentityGrpcService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    /// <summary>
    /// Returns email and name for a given userId.
    /// </summary>
    public override async Task<GetUserByIdResponse> GetUserById(
        GetUserByIdRequest request,
        ServerCallContext context
    )
    {
        if (!Guid.TryParse(request.UserId, out var userId))
        {
            _logger.LogWarning("GetUserById called with invalid userId: {UserId}", request.UserId);
            return new GetUserByIdResponse { Found = false };
        }

        var user = await _userRepository.GetUserWithRoleAsync(userId);
        if (user is null)
        {
            _logger.LogWarning("GetUserById - user not found: {UserId}", userId);
            return new GetUserByIdResponse { Found = false };
        }

        return new GetUserByIdResponse
        {
            Found = true,
            UserId = user.Id.ToString(),
            Email = user.Email ?? string.Empty,
            Name = user.Name ?? string.Empty,
            RoleCode = user.Role?.Code ?? string.Empty,
        };
    }

    /// <summary>
    /// Returns all active users with the given role code.
    /// </summary>
    public override async Task<GetUsersByRoleResponse> GetUsersByRole(
        GetUsersByRoleRequest request,
        ServerCallContext context
    )
    {
        if (string.IsNullOrWhiteSpace(request.RoleCode))
        {
            _logger.LogWarning("GetUsersByRole called with empty roleCode");
            return new GetUsersByRoleResponse();
        }

        var (_, users) = await _userRepository.GetAllUsersAsync(1, 1000);

        var filtered = users
            .Where(u => u.IsActive && u.Role?.Code == request.RoleCode)
            .Select(u => new UserSummary
            {
                UserId = u.Id.ToString(),
                Email = u.Email ?? string.Empty,
                Name = u.Name ?? string.Empty,
            });

        var response = new GetUsersByRoleResponse();
        response.Users.AddRange(filtered);

        _logger.LogInformation(
            "GetUsersByRole - found {Count} users with role {RoleCode}",
            response.Users.Count,
            request.RoleCode
        );

        return response;
    }
}
