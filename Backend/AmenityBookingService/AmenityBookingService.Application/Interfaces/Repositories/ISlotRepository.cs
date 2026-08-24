using AmenityBookingService.Domain.Entities;

namespace AmenityBookingService.Application.Interfaces.Repositories;

/// <summary>
/// Repository for slot management operations.
/// </summary>
public interface ISlotRepository
{
    /// <summary>Gets a paginated list of slots for an amenity.</summary>
    Task<IEnumerable<AmenitySlot>> GetSlotsByAmenityIdAsync(
        Guid amenityId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default
    );

    /// <summary>Gets the total count of slots for an amenity.</summary>
    Task<int> GetSlotsCountByAmenityIdAsync(
        Guid amenityId,
        CancellationToken cancellationToken = default
    );

    /// <summary>Gets a slot by its unique identifier.</summary>
    Task<AmenitySlot?> GetSlotByIdAsync(Guid slotId, CancellationToken cancellationToken = default);

    /// <summary>Checks if a slot already exists for a specific date and time.</summary>
    Task<bool> SlotExistsAsync(
        Guid amenityId,
        DateTime slotDate,
        TimeSpan startTime,
        CancellationToken cancellationToken = default
    );

    /// <summary>Creates a new slot and returns it with generated fields populated.</summary>
    Task<AmenitySlot> CreateSlotAsync(
        AmenitySlot slot,
        CancellationToken cancellationToken = default
    );

    /// <summary>Updates an existing slot.</summary>
    Task<AmenitySlot> UpdateSlotAsync(
        AmenitySlot slot,
        CancellationToken cancellationToken = default
    );

    /// <summary>Soft-deletes a slot. Returns true if deleted.</summary>
    Task<bool> DeleteSlotAsync(Guid slotId, CancellationToken cancellationToken = default);

    /// <summary>Checks if a slot has any bookings.</summary>
    Task<bool> SlotHasBookingsAsync(Guid slotId, CancellationToken cancellationToken = default);

    /// <summary>Gets the current booking count for a slot.</summary>
    Task<int> GetCurrentBookingsCountForSlotAsync(
        Guid slotId,
        CancellationToken cancellationToken = default
    );

    /// <summary>Gets a paginated list of available slots for an amenity.</summary>
    Task<IEnumerable<AmenitySlot>> GetAvailableSlotsAsync(
        Guid amenityId,
        DateTime? date,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default
    );

    /// <summary>Gets the total count of available slots for an amenity.</summary>
    Task<int> GetAvailableSlotsCountAsync(
        Guid amenityId,
        DateTime? date,
        CancellationToken cancellationToken = default
    );
}
