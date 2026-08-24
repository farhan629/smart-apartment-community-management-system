using AmenityBookingService.Domain.Entities;

namespace AmenityBookingService.Application.Interfaces.Repositories;

/// <summary>
/// Repository for booking management operations.
/// </summary>
public interface IBookingRepository
{
    /// <summary>Gets a paginated list of bookings for a user with optional filters.</summary>
    Task<IEnumerable<AmenityBooking>> GetUserBookingsAsync(
        Guid userId,
        string? status,
        DateTime? fromDate,
        DateTime? toDate,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default
    );

    /// <summary>Gets the total count of user bookings matching the optional filters.</summary>
    Task<int> GetUserBookingsCountAsync(
        Guid userId,
        string? status,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken cancellationToken = default
    );

    /// <summary>Gets a booking by its unique identifier.</summary>
    Task<AmenityBooking?> GetBookingByIdAsync(
        Guid bookingId,
        CancellationToken cancellationToken = default
    );

    /// <summary>Creates a new booking and returns it with generated fields populated.</summary>
    Task<AmenityBooking> CreateBookingAsync(
        AmenityBooking booking,
        CancellationToken cancellationToken = default
    );

    /// <summary>Updates an existing booking.</summary>
    Task<AmenityBooking> UpdateBookingAsync(
        AmenityBooking booking,
        CancellationToken cancellationToken = default
    );

    /// <summary>Checks if a user already has a booking for a specific slot.</summary>
    Task<bool> BookingExistsForSlotAsync(
        Guid slotId,
        Guid userId,
        CancellationToken cancellationToken = default
    );

    /// <summary>Checks if any user has a booking for a specific slot.</summary>
    Task<bool> BookingExistsForSlotAnyUserAsync(
        Guid slotId,
        CancellationToken cancellationToken = default
    );

    /// <summary>Gets a user's existing booking for a specific slot.</summary>
    Task<AmenityBooking?> GetBookingBySlotAndUserAsync(
        Guid slotId,
        Guid userId,
        CancellationToken cancellationToken = default
    );

    /// <summary>Gets a user's inactive (cancelled) booking for a specific slot.</summary>
    Task<AmenityBooking?> GetInactiveBookingBySlotAndUserAsync(
        Guid slotId,
        Guid userId,
        CancellationToken cancellationToken = default
    );

    /// <summary>Gets all expired bookings that are still in BOOKED status.</summary>
    Task<List<AmenityBooking>> GetExpiredBookingsAsync(
        CancellationToken cancellationToken = default
    );

    /// <summary>Gets a paginated list of all bookings with optional filters (admin).</summary>
    Task<IEnumerable<AmenityBooking>> GetAllBookingsAsync(
        string? status,
        Guid? amenityId,
        string? slotType,
        DateTime? fromDate,
        DateTime? toDate,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default
    );

    /// <summary>Gets the total count of all bookings matching the optional filters (admin).</summary>
    Task<int> GetAllBookingsCountAsync(
        string? status,
        Guid? amenityId,
        string? slotType,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken cancellationToken = default
    );
}
