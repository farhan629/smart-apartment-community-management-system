using System.Globalization;
using ComplaintMaintenanceService.Application.Common.Constants;

namespace ComplaintMaintenanceService.API.Helpers;

/// <summary>
/// Shared helper for parsing query-string date values into UTC <see cref="DateTime"/> values.
/// </summary>
public static class DateParsingHelper
{
    /// <summary>
    /// Attempts to parse <paramref name="value"/> using the accepted date formats and
    /// returns it as a UTC-kinded <see cref="DateTime"/>, or <c>null</c> if parsing fails
    /// or the value is empty.
    /// </summary>
    public static DateTime? ParseDateUtc(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var formats = ComplaintConstants.DateFormats.Accepted;

        if (
            DateTime.TryParseExact(
                value,
                formats,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var parsed
            )
        )
        {
            return DateTime.SpecifyKind(parsed, DateTimeKind.Utc);
        }

        return null;
    }
}
