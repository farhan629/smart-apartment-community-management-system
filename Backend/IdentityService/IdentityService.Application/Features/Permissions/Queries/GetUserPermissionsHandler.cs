using IdentityService.Application.Interfaces.Repositories;
using MediatR;
using Shared.SharedLibrary.DTO;
using Shared.SharedLibrary.Exceptions;

namespace IdentityService.Application.Features.Permissions.Queries;

/// <summary>
/// Query to get all permissions for a user including role-based and user-specific policies.
/// </summary>
public class GetUserPermissionsQuery : IRequest<UserPermissionsDto>
{
    /// <summary>
    /// Gets or sets the user ID.
    /// </summary>
    public Guid UserId { get; set; }
}

/// <summary>
/// Handler for processing <see cref="GetUserPermissionsQuery"/>.
/// </summary>
public class GetUserPermissionsHandler
    : IRequestHandler<GetUserPermissionsQuery, UserPermissionsDto>
{
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetUserPermissionsHandler"/> class.
    /// </summary>
    public GetUserPermissionsHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    /// <summary>
    /// Handles the retrieval of user permissions with role and policy overrides.
    /// </summary>
    public async Task<UserPermissionsDto> Handle(
        GetUserPermissionsQuery request,
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

        return new UserPermissionsDto
        {
            UserId = request.UserId,
            RoleId = user.RoleId,
            RoleName = user.Role?.DisplayName ?? string.Empty,
            Permissions = finalPermissions.Distinct().ToList(),
        };
    }
}
