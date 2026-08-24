using AmenityBookingService.Application.Interfaces.Repositories;
using AmenityBookingService.Domain.Entities;
using AmenityBookingService.Infrastructure.Persistence.DBContext;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace AmenityBookingService.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for amenity CRUD operations.
/// </summary>
public class AmenityRepository : IAmenityRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="AmenityRepository"/> class.
    /// </summary>
    public AmenityRepository(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// Gets an active amenity by ID with SlotType and Status included.
    /// </summary>
    public async Task<Amenity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context
            .Amenities.Where(a => a.Id == id && a.IsActive)
            .ProjectTo<Amenity>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Gets a paginated list of active amenities with optional name and slot type filters.
    /// </summary>
    public async Task<IEnumerable<Amenity>> GetAllAsync(
        int pageNumber,
        int pageSize,
        string? searchName,
        string? slotType,
        CancellationToken cancellationToken = default
    )
    {
        var query = _context.Amenities.Where(a => a.IsActive);

        if (!string.IsNullOrEmpty(searchName))
            query = query.Where(a => a.Name.Contains(searchName));

        if (!string.IsNullOrEmpty(slotType))
            query = query.Where(a => a.SlotType != null && a.SlotType.Code == slotType);

        return await query
            .OrderBy(a => a.Name)
            .ProjectTo<Amenity>(_mapper.ConfigurationProvider)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets the total count of active amenities matching the filters.
    /// </summary>
    public async Task<int> GetTotalCountAsync(
        string? searchName,
        string? slotType,
        CancellationToken cancellationToken = default
    )
    {
        var query = _context.Amenities.AsNoTracking().Where(a => a.IsActive);

        if (!string.IsNullOrEmpty(searchName))
            query = query.Where(a => a.Name.Contains(searchName));

        if (!string.IsNullOrEmpty(slotType))
            query = query.Where(a => a.SlotType != null && a.SlotType.Code == slotType);

        return await query.CountAsync(cancellationToken);
    }

    /// <summary>
    /// Checks if an active amenity with the given name exists.
    /// </summary>
    public async Task<bool> ExistsByNameAsync(
        string name,
        CancellationToken cancellationToken = default
    )
    {
        return await _context
            .Amenities.AsNoTracking()
            .AnyAsync(a => a.Name.ToLower() == name.ToLower() && a.IsActive, cancellationToken);
    }

    /// <summary>
    /// Checks if an active amenity with the given name exists excluding a specific ID.
    /// </summary>
    public async Task<bool> ExistsByNameAsync(
        string name,
        Guid excludeId,
        CancellationToken cancellationToken = default
    )
    {
        return await _context
            .Amenities.AsNoTracking()
            .AnyAsync(
                a => a.Name.ToLower() == name.ToLower() && a.Id != excludeId && a.IsActive,
                cancellationToken
            );
    }

    /// <summary>
    /// Creates a new amenity and saves it to the database.
    /// </summary>
    public async Task<Amenity> CreateAsync(
        Amenity amenity,
        CancellationToken cancellationToken = default
    )
    {
        await _context.Amenities.AddAsync(amenity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return amenity;
    }

    /// <summary>
    /// Updates an existing amenity in the database.
    /// </summary>
    public async Task<Amenity> UpdateAsync(
        Amenity amenity,
        CancellationToken cancellationToken = default
    )
    {
        _context.Amenities.Update(amenity);
        await _context.SaveChangesAsync(cancellationToken);
        return amenity;
    }

    /// <summary>
    /// Soft-deletes an amenity by setting IsActive to false.
    /// </summary>
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var amenity = await _context.Amenities.FindAsync(new object[] { id }, cancellationToken);
        if (amenity == null)
            return false;

        amenity.IsActive = false;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    /// <summary>
    /// Checks if an amenity has any active slots or booking history.
    /// </summary>
    public async Task<bool> HasSlotsOrBookingsAsync(
        Guid amenityId,
        CancellationToken cancellationToken = default
    )
    {
        var hasSlots = await _context
            .AmenitySlots.AsNoTracking()
            .AnyAsync(s => s.AmenityId == amenityId && s.IsActive, cancellationToken);

        if (hasSlots)
            return true;

        var hasBookings = await _context
            .AmenityBookings.AsNoTracking()
            .AnyAsync(
                b => b.AmenitySlot != null && b.AmenitySlot.AmenityId == amenityId,
                cancellationToken
            );

        return hasBookings;
    }

    /// <summary>
    /// Gets an amenity with its SlotType navigation property loaded.
    /// </summary>
    public async Task<Amenity?> GetAmenityWithSlotTypeAsync(
        Guid amenityId,
        CancellationToken cancellationToken = default
    )
    {
        return await _context
            .Amenities.Where(a => a.Id == amenityId && a.IsActive)
            .ProjectTo<Amenity>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
