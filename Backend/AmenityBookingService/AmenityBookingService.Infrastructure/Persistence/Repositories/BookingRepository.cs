using AmenityBookingService.Application.Interfaces.Repositories;
using AmenityBookingService.Domain.Entities;
using AmenityBookingService.Infrastructure.Persistence.DBContext;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Shared.SharedLibrary.Constants;

namespace AmenityBookingService.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository for booking management operations
/// </summary>
public class BookingRepository : IBookingRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public BookingRepository(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<AmenityBooking>> GetUserBookingsAsync(
        Guid userId,
        string? status,
        DateTime? fromDate,
        DateTime? toDate,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default
    )
    {
        var query = _context.AmenityBookings.Where(b => b.UserId == userId);

        if (!string.IsNullOrEmpty(status))
            query = query.Where(b => b.BookingStatus != null && b.BookingStatus.Code == status);

        if (fromDate.HasValue)
        {
            var fromDateUtc = DateTime.SpecifyKind(fromDate.Value, DateTimeKind.Utc);
            query = query.Where(b =>
                b.AmenitySlot != null && b.AmenitySlot.SlotDate >= fromDateUtc
            );
        }

        if (toDate.HasValue)
        {
            var toDateUtc = DateTime.SpecifyKind(toDate.Value, DateTimeKind.Utc);
            query = query.Where(b => b.AmenitySlot != null && b.AmenitySlot.SlotDate <= toDateUtc);
        }

        return await query
            .ProjectTo<AmenityBooking>(_mapper.ConfigurationProvider)
            .OrderByDescending(b => b.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetUserBookingsCountAsync(
        Guid userId,
        string? status,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken cancellationToken = default
    )
    {
        var query = _context.AmenityBookings.Where(b => b.UserId == userId);

        if (!string.IsNullOrEmpty(status))
            query = query.Where(b => b.BookingStatus != null && b.BookingStatus.Code == status);

        if (fromDate.HasValue)
        {
            var fromDateUtc = DateTime.SpecifyKind(fromDate.Value, DateTimeKind.Utc);
            query = query.Where(b =>
                b.AmenitySlot != null && b.AmenitySlot.SlotDate >= fromDateUtc
            );
        }

        if (toDate.HasValue)
        {
            var toDateUtc = DateTime.SpecifyKind(toDate.Value, DateTimeKind.Utc);
            query = query.Where(b => b.AmenitySlot != null && b.AmenitySlot.SlotDate <= toDateUtc);
        }

        return await query.CountAsync(cancellationToken);
    }

    public async Task<AmenityBooking?> GetBookingByIdAsync(
        Guid bookingId,
        CancellationToken cancellationToken = default
    )
    {
        return await _context
            .AmenityBookings.Include(b => b.AmenitySlot)
                .ThenInclude(s => s.Amenity)
            .FirstOrDefaultAsync(b => b.Id == bookingId, cancellationToken);
    }

    public async Task<AmenityBooking> CreateBookingAsync(
        AmenityBooking booking,
        CancellationToken cancellationToken = default
    )
    {
        await _context.AmenityBookings.AddAsync(booking, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return booking;
    }

    public async Task<AmenityBooking> UpdateBookingAsync(
        AmenityBooking booking,
        CancellationToken cancellationToken = default
    )
    {
        _context.AmenityBookings.Update(booking);
        await _context.SaveChangesAsync(cancellationToken);
        return booking;
    }

    public async Task<bool> BookingExistsForSlotAsync(
        Guid slotId,
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        return await _context.AmenityBookings.AnyAsync(
            b => b.AmenitySlotId == slotId && b.UserId == userId && b.IsActive,
            cancellationToken
        );
    }

    public async Task<bool> BookingExistsForSlotAnyUserAsync(
        Guid slotId,
        CancellationToken cancellationToken = default
    )
    {
        return await _context.AmenityBookings.AnyAsync(
            b => b.AmenitySlotId == slotId && b.IsActive,
            cancellationToken
        );
    }

    public async Task<AmenityBooking?> GetBookingBySlotAndUserAsync(
        Guid slotId,
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        return await _context
            .AmenityBookings.Where(b => b.AmenitySlotId == slotId && b.UserId == userId)
            .ProjectTo<AmenityBooking>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<AmenityBooking?> GetInactiveBookingBySlotAndUserAsync(
        Guid slotId,
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        return await _context
            .AmenityBookings.Where(b =>
                b.AmenitySlotId == slotId && b.UserId == userId && !b.IsActive
            )
            .ProjectTo<AmenityBooking>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<AmenityBooking>> GetExpiredBookingsAsync(
        CancellationToken cancellationToken = default
    )
    {
        var now = DateTime.UtcNow;

        return await _context
            .AmenityBookings.Include(b => b.AmenitySlot)
            .Where(b =>
                b.BookingStatus != null
                && b.BookingStatus.Code == RefTermCodes.Booked
                && b.AmenitySlot != null
                && (
                    b.AmenitySlot.SlotDate < now.Date
                    || (
                        b.AmenitySlot.SlotDate == now.Date && b.AmenitySlot.EndTime <= now.TimeOfDay
                    )
                )
            )
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<AmenityBooking>> GetAllBookingsAsync(
        string? status,
        Guid? amenityId,
        string? slotType,
        DateTime? fromDate,
        DateTime? toDate,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default
    )
    {
        var query = _context.AmenityBookings.AsQueryable();

        if (!string.IsNullOrEmpty(status))
            query = query.Where(b => b.BookingStatus != null && b.BookingStatus.Code == status);

        if (amenityId.HasValue)
            query = query.Where(b =>
                b.AmenitySlot != null && b.AmenitySlot.AmenityId == amenityId.Value
            );

        if (!string.IsNullOrEmpty(slotType))
            query = query.Where(b =>
                b.AmenitySlot != null
                && b.AmenitySlot.Amenity != null
                && b.AmenitySlot.Amenity.SlotType != null
                && b.AmenitySlot.Amenity.SlotType.Code == slotType
            );

        if (fromDate.HasValue)
        {
            var fromDateUtc = DateTime.SpecifyKind(fromDate.Value, DateTimeKind.Utc);
            query = query.Where(b =>
                b.AmenitySlot != null && b.AmenitySlot.SlotDate >= fromDateUtc
            );
        }

        if (toDate.HasValue)
        {
            var toDateUtc = DateTime.SpecifyKind(toDate.Value, DateTimeKind.Utc);
            query = query.Where(b => b.AmenitySlot != null && b.AmenitySlot.SlotDate <= toDateUtc);
        }

        return await query
            .ProjectTo<AmenityBooking>(_mapper.ConfigurationProvider)
            .OrderByDescending(b => b.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetAllBookingsCountAsync(
        string? status,
        Guid? amenityId,
        string? slotType,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken cancellationToken = default
    )
    {
        var query = _context.AmenityBookings.AsQueryable();

        if (!string.IsNullOrEmpty(status))
            query = query.Where(b => b.BookingStatus != null && b.BookingStatus.Code == status);

        if (amenityId.HasValue)
            query = query.Where(b =>
                b.AmenitySlot != null && b.AmenitySlot.AmenityId == amenityId.Value
            );

        if (!string.IsNullOrEmpty(slotType))
            query = query.Where(b =>
                b.AmenitySlot != null
                && b.AmenitySlot.Amenity != null
                && b.AmenitySlot.Amenity.SlotType != null
                && b.AmenitySlot.Amenity.SlotType.Code == slotType
            );

        if (fromDate.HasValue)
        {
            var fromDateUtc = DateTime.SpecifyKind(fromDate.Value, DateTimeKind.Utc);
            query = query.Where(b =>
                b.AmenitySlot != null && b.AmenitySlot.SlotDate >= fromDateUtc
            );
        }

        if (toDate.HasValue)
        {
            var toDateUtc = DateTime.SpecifyKind(toDate.Value, DateTimeKind.Utc);
            query = query.Where(b => b.AmenitySlot != null && b.AmenitySlot.SlotDate <= toDateUtc);
        }

        return await query.CountAsync(cancellationToken);
    }
}
