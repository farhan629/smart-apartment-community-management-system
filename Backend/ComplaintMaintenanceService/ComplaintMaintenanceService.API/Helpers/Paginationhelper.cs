namespace ComplaintMaintenanceService.API.Helpers;

/// <summary>
/// Shared helper for resolving paging values, falling back to defaults when the
/// caller passes an unset (zero) value.
/// </summary>
public static class PaginationHelper
{
    /// <summary>
    /// Returns <paramref name="page"/> if non-zero, otherwise <paramref name="defaultPage"/>.
    /// </summary>
    public static int ResolvePage(int page, int defaultPage) => page == 0 ? defaultPage : page;

    /// <summary>
    /// Returns <paramref name="limit"/> if non-zero, otherwise <paramref name="defaultLimit"/>.
    /// </summary>
    public static int ResolveLimit(int limit, int defaultLimit) =>
        limit == 0 ? defaultLimit : limit;
}