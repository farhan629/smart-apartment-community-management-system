using IdentityService.Application.Features.Permissions.DTOs;
using IdentityService.Application.Interfaces.Repositories;
using MediatR;
using Shared.SharedLibrary.Exceptions;

namespace IdentityService.Application.Features.Permissions.Queries;

/// <summary>
/// Query to get user permissions with policy overrides
/// </summary>
public class GetUserPermissionsWithPoliciesQuery : IRequest<UserPermissionsResponseDto>
{
    public Guid UserId { get; set; }
}

/// <summary>
/// Handler for GetUserPermissionsWithPoliciesQuery
/// </summary>
public class GetUserPermissionsWithPoliciesHandler
    : IRequestHandler<GetUserPermissionsWithPoliciesQuery, UserPermissionsResponseDto>
{
    private readonly IUserRepository _userRepository;

    public GetUserPermissionsWithPoliciesHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    /// <summary>
    /// Handles the query by combining role-based permissions with user-specific policy overrides
    /// </summary>
    /// <param name="request">Query request containing UserId</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User permissions response DTO</returns>
    public async Task<UserPermissionsResponseDto> Handle(
        GetUserPermissionsWithPoliciesQuery request,
        CancellationToken cancellationToken
    )
    {
        var user = await _userRepository.GetUserWithRoleAsync(request.UserId, cancellationToken);

        if (user == null)
            throw new NotFoundException("User not found");

        var rolePermissions = await _userRepository.GetRolePermissionsAsync(
            user.RoleId,
            cancellationToken
        );

        var userPolicies = await _userRepository.GetUserPoliciesAsync(
            request.UserId,
            cancellationToken
        );

        var finalPermissions = new List<string>();

        foreach (var permission in rolePermissions)
        {
            var overridePolicy = userPolicies.FirstOrDefault(up => up.PermissionCode == permission);

            if (overridePolicy != null)
            {
                if (overridePolicy.IsAllowed)
                    finalPermissions.Add(permission);
            }
            else
            {
                finalPermissions.Add(permission);
            }
        }

        var extraPermissions = userPolicies
            .Where(up => up.IsAllowed && !rolePermissions.Contains(up.PermissionCode))
            .Select(up => up.PermissionCode);

        finalPermissions.AddRange(extraPermissions);

        return new UserPermissionsResponseDto
        {
            UserId = request.UserId,
            Permissions = finalPermissions.Distinct().ToList(),
        };
    }
}
