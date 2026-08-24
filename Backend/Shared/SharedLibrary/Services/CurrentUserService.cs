using Microsoft.AspNetCore.Http;
using Shared.SharedLibrary.Extensions;
using Shared.SharedLibrary.Services;

namespace Shared.SharedLibrary.Services;

/// <summary>
/// Service for accessing the current authenticated user's information.
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="CurrentUserService"/> class.
    /// </summary>
    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Gets the current user ID from the HTTP context.
    /// </summary>
    public Guid UserId => _httpContextAccessor.HttpContext?.User.GetCurrentUserId() ?? Guid.Empty;

    /// <summary>
    /// Gets the current role ID from the HTTP context.
    /// </summary>
    public Guid RoleId => _httpContextAccessor.HttpContext?.User.GetCurrentRoleId() ?? Guid.Empty;
}
