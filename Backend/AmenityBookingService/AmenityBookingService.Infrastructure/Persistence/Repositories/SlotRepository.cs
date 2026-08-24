using AmenityBookingService.Application.Interfaces.Repositories;
using AmenityBookingService.Domain.Entities;
using AmenityBookingService.Infrastructure.Persistence.DBContext;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Shared.SharedLibrary.Constants;

namespace AmenityBookingService.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for slot management operations.
/// </summary>
public class SlotRepository : ISlotRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="SlotRepository"/> class.
    /// </summary>
    public SlotRepository(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// Gets a paginated list of active slots for an amenity.
    /// </summary>
    public async Task<IEnumerable<AmenitySlot>> GetSlotsByAmenityIdAsync(
        Guid amenityId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default
    )
    {
        return await _context
            .AmenitySlots.AsNoTracking()
            .Where(s => s.AmenityId == amenityId && s.IsActive)
            .OrderBy(s => s.SlotDate)
            .ThenBy(s => s.StartTime)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets the total count of active slots for an amenity.
    /// </summary>
    public async Task<int> GetSlotsCountByAmenityIdAsync(
        Guid amenityId,
        CancellationToken cancellationToken = default
    )
    {
        return await _context
            .AmenitySlots.AsNoTracking()
            .CountAsync(s => s.AmenityId == amenityId && s.IsActive, cancellationToken);
    }

    /// <summary>
    /// Gets an active slot by ID with Amenity and SlotType included.
    /// </summary>
    public async Task<AmenitySlot?> GetSlotByIdAsync(
        Guid slotId,
        CancellationToken cancellationToken = default
    )
    {
        return await _context
            .AmenitySlots.Where(s => s.Id == slotId && s.IsActive)
            .ProjectTo<AmenitySlot>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Checks if a slot already exists for a specific date and time.
    /// </summary>
    public async Task<bool> SlotExistsAsync(
        Guid amenityId,
        DateTime slotDate,
        TimeSpan startTime,
        CancellationToken cancellationToken = default
    )
    {
        var utcSlotDate = DateTime.SpecifyKind(slotDate, DateTimeKind.Utc);
        return await _context
            .AmenitySlots.AsNoTracking()
            .AnyAsync(
                s =>
                    s.AmenityId == amenityId
                    && s.SlotDate == utcSlotDate
                    && s.StartTime == startTime
                    && s.IsActive,
                cancellationToken
            );
    }

    /// <summary>
    /// Creates a new slot and saves it to the database.
    /// </summary>
    public async Task<AmenitySlot> CreateSlotAsync(
        AmenitySlot slot,
        CancellationToken cancellationToken = default
    )
    {
        await _context.AmenitySlots.AddAsync(slot, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return slot;
    }

    /// <summary>
    /// Updates an existing slot in the database.
    /// </summary>
    public async Task<AmenitySlot> UpdateSlotAsync(
        AmenitySlot slot,
        CancellationToken cancellationToken = default
    )
    {
        _context.AmenitySlots.Update(slot);
        await _context.SaveChangesAsync(cancellationToken);
        return slot;
    }

    /// <summary>
    /// Soft-deletes a slot by setting IsActive to false.
    /// </summary>
    public async Task<bool> DeleteSlotAsync(
        Guid slotId,
        CancellationToken cancellationToken = default
    )
    {
        var slot = await _context.AmenitySlots.FindAsync(
            new object[] { slotId },
            cancellationToken
        );
        if (slot == null)
            return false;

        slot.IsActive = false;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    /// <summary>
    /// Checks if a slot has any bookings.
    /// </summary>
    public async Task<bool> SlotHasBookingsAsync(
        Guid slotId,
        CancellationToken cancellationToken = default
    )
    {
        return await _context
            .AmenityBookings.AsNoTracking()
            .AnyAsync(b => b.AmenitySlotId == slotId && b.IsActive, cancellationToken);
    }

    /// <summary>
    /// Gets the current booking count for a slot based on slot type.
    /// </summary>
    public async Task<int> GetCurrentBookingsCountForSlotAsync(
        Guid slotId,
        CancellationToken cancellationToken = default
    )
    {
        var slotTypeCode = await _context
            .AmenitySlots.AsNoTracking()
            .Where(s => s.Id == slotId)
            .Select(s =>
                s.Amenity != null
                    ? s.Amenity.SlotType != null
                        ? s.Amenity.SlotType.Code
                        : null
                    : null
            )
            .FirstOrDefaultAsync(cancellationToken);

        if (slotTypeCode == null)
            return 0;

        var cancelledStatusId = await _context
            .RefTerms.AsNoTracking()
            .Where(rt => rt.Code == RefTermCodes.Cancelled)
            .Select(rt => (Guid?)rt.Id)
            .FirstOrDefaultAsync(cancellationToken);

        var bookingsQuery = _context
            .AmenityBookings.AsNoTracking()
            .Where(b => b.AmenitySlotId == slotId);

        if (cancelledStatusId.HasValue)
            bookingsQuery = bookingsQuery.Where(b => b.BookingStatusId != cancelledStatusId.Value);

        if (slotTypeCode == RefTermCodes.TimeCount)
        {
            return await bookingsQuery.SumAsync(b => (int?)b.PeopleCount, cancellationToken) ?? 0;
        }

        return await bookingsQuery.CountAsync(cancellationToken);
    }

    /// <summary>
    /// Gets a paginated list of available slots for an amenity, optionally filtered by date.
    /// </summary>
    public async Task<IEnumerable<AmenitySlot>> GetAvailableSlotsAsync(
        Guid amenityId,
        DateTime? date,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default
    )
    {
        var query = _context
            .AmenitySlots.AsNoTracking()
            .Where(s => s.AmenityId == amenityId && s.IsActive);

        if (date.HasValue)
            query = query.Where(s => s.SlotDate.Date == date.Value.Date);

        return await query
            .OrderBy(s => s.SlotDate)
            .ThenBy(s => s.StartTime)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets the total count of available slots for an amenity, optionally filtered by date.
    /// </summary>
    public async Task<int> GetAvailableSlotsCountAsync(
        Guid amenityId,
        DateTime? date,
        CancellationToken cancellationToken = default
    )
    {
        var query = _context
            .AmenitySlots.AsNoTracking()
            .Where(s => s.AmenityId == amenityId && s.IsActive);

        if (date.HasValue)
            query = query.Where(s => s.SlotDate.Date == date.Value.Date);

        return await query.CountAsync(cancellationToken);
    }
}
