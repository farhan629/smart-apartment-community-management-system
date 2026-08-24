using AmenityBookingService.Domain.Entities;

namespace AmenityBookingService.Application.Interfaces.Repositories;

/// <summary>
/// Repository for report generation operations.
/// </summary>
public interface IReportRepository
{
    /// <summary>Generates booking report with optional filters.</summary>
    /// <returns>Returns booking metrics including totals, counts, and utilization rate.</returns>
    Task<(
        int TotalBookings,
        int TotalPeople,
        int ActiveBookings,
        int CancelledBookings,
        int CompletedBookings,
        double UtilizationRate
    )> GetBookingReportAsync(
        Guid? amenityId,
        string? slotType,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken cancellationToken = default
    );
}
