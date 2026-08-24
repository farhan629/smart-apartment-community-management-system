using AmenityBookingService.Application.Interfaces.Repositories;
using AmenityBookingService.Domain.Entities;
using AmenityBookingService.Infrastructure.Persistence.DBContext;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Shared.SharedLibrary.Constants;

namespace AmenityBookingService.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository for report generation operations
/// </summary>
public class ReportRepository : IReportRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public ReportRepository(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<(
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
    )
    {
        var query = _context.AmenityBookings.AsQueryable();

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

        var bookings = await query
            .ProjectTo<AmenityBooking>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        var totalBookings = bookings.Count;
        var totalPeople = bookings.Sum(b => b.PeopleCount);

        var cancelledStatus = await _context.RefTerms.FirstOrDefaultAsync(
            rt => rt.Code == RefTermCodes.Cancelled,
            cancellationToken
        );

        var completedStatus = await _context.RefTerms.FirstOrDefaultAsync(
            rt => rt.Code == RefTermCodes.Completed,
            cancellationToken
        );

        var activeBookings =
            cancelledStatus != null
                ? bookings.Count(b => b.BookingStatusId != cancelledStatus.Id)
                : bookings.Count;

        var cancelledBookings =
            cancelledStatus != null
                ? bookings.Count(b => b.BookingStatusId == cancelledStatus.Id)
                : 0;

        var completedBookings =
            completedStatus != null
                ? bookings.Count(b => b.BookingStatusId == completedStatus.Id)
                : 0;

        var totalSlots = await _context
            .AmenitySlots.Where(s => s.IsActive)
            .CountAsync(cancellationToken);

        var utilizationRate =
            totalSlots > 0 ? Math.Round((double)bookings.Count / totalSlots * 100, 1) : 0;

        return (
            totalBookings,
            totalPeople,
            activeBookings,
            cancelledBookings,
            completedBookings,
            utilizationRate
        );
    }
}
