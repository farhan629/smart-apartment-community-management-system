using AmenityBookingService.Domain.Entities;

namespace AmenityBookingService.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface for core amenity CRUD operations and lookup queries.
/// </summary>
public interface IAmenityRepository
{
    /// <summary>Gets an active amenity by ID, including SlotType and Status navigation properties.</summary>
    Task<Amenity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Gets a paginated list of active amenities with optional name and slot type filters.</summary>
    Task<IEnumerable<Amenity>> GetAllAsync(
        int pageNumber,
        int pageSize,
        string? searchName,
        string? slotType,
        CancellationToken cancellationToken = default
    );

    /// <summary>Gets the total count of active amenities matching the optional filters.</summary>
    Task<int> GetTotalCountAsync(
        string? searchName,
        string? slotType,
        CancellationToken cancellationToken = default
    );

    /// <summary>Checks whether an active amenity with the given name already exists.</summary>
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>Checks whether an active amenity with the given name exists, excluding a specific ID.</summary>
    Task<bool> ExistsByNameAsync(
        string name,
        Guid excludeId,
        CancellationToken cancellationToken = default
    );

    /// <summary>Creates a new amenity and returns it with generated fields populated.</summary>
    Task<Amenity> CreateAsync(Amenity amenity, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing amenity.</summary>
    Task<Amenity> UpdateAsync(Amenity amenity, CancellationToken cancellationToken = default);

    /// <summary>Soft-deletes an amenity by setting IsActive to false. Returns true if deleted.</summary>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Checks whether an amenity has any active slots or associated booking history.</summary>
    Task<bool> HasSlotsOrBookingsAsync(
        Guid amenityId,
        CancellationToken cancellationToken = default
    );

    /// <summary>Gets an amenity with its SlotType navigation property loaded.</summary>
    Task<Amenity?> GetAmenityWithSlotTypeAsync(
        Guid amenityId,
        CancellationToken cancellationToken = default
    );
}
