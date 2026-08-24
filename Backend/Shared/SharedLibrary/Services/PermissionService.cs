using System.Net.Http.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Shared.SharedLibrary.DTO;

namespace Shared.SharedLibrary.Services;

/// <summary>
/// Service for managing user permissions with caching.
/// </summary>
public class PermissionService : IPermissionService
{
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;
    private readonly ILogger<PermissionService> _logger;
    private const string CACHE_KEY_PREFIX = "permissions_";
    private const int CACHE_DURATION_MINUTES = 5;

    /// <summary>
    /// Initializes a new instance of the <see cref="PermissionService"/> class.
    /// </summary>
    public PermissionService(
        HttpClient httpClient,
        IMemoryCache cache,
        ILogger<PermissionService> logger
    )
    {
        _httpClient = httpClient;
        _cache = cache;
        _logger = logger;
    }

    /// <summary>
    /// Gets all permissions for a user with caching.
    /// </summary>
    public async Task<List<string>> GetUserPermissionsAsync(Guid userId)
    {
        var cacheKey = $"{CACHE_KEY_PREFIX}{userId}";

        if (_cache.TryGetValue(cacheKey, out List<string>? permissions))
        {
            _logger.LogInformation("Cache HIT for user: {UserId}", userId);
            return permissions ?? new List<string>();
        }

        _logger.LogInformation(
            "Cache MISS for user: {UserId}, fetching from Identity Service",
            userId
        );

        // Call Identity Service
        var response = await _httpClient.GetFromJsonAsync<UserPermissionsDto>("api/permission/me");

        permissions = response?.Permissions ?? new List<string>();

        _cache.Set(cacheKey, permissions, TimeSpan.FromMinutes(CACHE_DURATION_MINUTES));

        return permissions;
    }

    /// <summary>
    /// Checks if a user has a specific permission.
    /// </summary>
    public async Task<bool> HasPermissionAsync(Guid userId, string permissionCode)
    {
        var permissions = await GetUserPermissionsAsync(userId);
        return permissions.Contains(permissionCode);
    }
}
