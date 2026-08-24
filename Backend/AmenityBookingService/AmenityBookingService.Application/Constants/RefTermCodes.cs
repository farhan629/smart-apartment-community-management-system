namespace AmenityBookingService.Application.Constants;

/// <summary>
/// Contains constant string values for reference term codes used across the application.
/// </summary>
public static class RefTermCodes
{
    /// <summary>Reference term code for TIME slot type.</summary>
    public const string Time = "TIME";

    /// <summary>Reference term code for TIME_COUNT slot type.</summary>
    public const string TimeCount = "TIME_COUNT";

    /// <summary>Reference term code for CANCELLED booking status.</summary>
    public const string Cancelled = "CANCELLED";

    /// <summary>Reference term code for BOOKED booking status.</summary>
    public const string Booked = "BOOKED";

    /// <summary>Reference term code for COMPLETED booking status.</summary>
    public const string Completed = "COMPLETED";

    /// <summary>Maximum image file size in bytes (10 MB).</summary>
    public const int MaxImageSizeInBytes = 10 * 1024 * 1024;
}
