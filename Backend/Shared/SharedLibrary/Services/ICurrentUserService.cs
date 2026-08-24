namespace Shared.SharedLibrary.Services;

/// <summary>
/// Service for accessing the current authenticated user's information.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Gets the current user ID.
    /// </summary>
    Guid UserId { get; }

    /// <summary>
    /// Gets the current role ID.
    /// </summary>
    Guid RoleId { get; }
}
