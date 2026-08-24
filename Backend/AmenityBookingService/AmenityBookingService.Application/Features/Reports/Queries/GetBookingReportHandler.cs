using AmenityBookingService.Application.Features.Bookings.DTO;
using AmenityBookingService.Application.Features.Reports.DTO;
using AmenityBookingService.Application.Interfaces.Repositories;
using AutoMapper;
using MediatR;
using Shared.SharedLibrary.DTO.Common;

namespace AmenityBookingService.Application.Features.Reports.Queries;

/// <summary>
/// Query for retrieving a booking report with optional filters and pagination.
/// </summary>
public class GetBookingReportQuery : IRequest<ReportResponseDto>
{
    /// <summary>Gets or sets the amenity ID to filter bookings.</summary>
    public Guid? AmenityId { get; set; }

    /// <summary>Gets or sets the slot type to filter bookings.</summary>
    public string? SlotType { get; set; }

    /// <summary>Gets or sets the start date of the reporting period.</summary>
    public DateTime? FromDate { get; set; }

    /// <summary>Gets or sets the end date of the reporting period.</summary>
    public DateTime? ToDate { get; set; }

    /// <summary>Gets or sets the page number for pagination.</summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>Gets or sets the number of records per page.</summary>
    public int PageSize { get; set; } = 10;
}

/// <summary>
/// Handles the retrieval of booking reports.
/// </summary>
public class GetBookingReportHandler : IRequestHandler<GetBookingReportQuery, ReportResponseDto>
{
    private readonly IReportRepository _reportRepository;
    private readonly IAmenityRepository _amenityRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetBookingReportHandler"/> class.
    /// </summary>
    /// <param name="reportRepository">The repository for retrieving report data.</param>
    /// <param name="amenityRepository">The repository for retrieving amenity details.</param>
    /// <param name="bookingRepository">The repository for retrieving booking data.</param>
    /// <param name="mapper">The AutoMapper instance.</param>
    public GetBookingReportHandler(
        IReportRepository reportRepository,
        IAmenityRepository amenityRepository,
        IBookingRepository bookingRepository,
        IMapper mapper
    )
    {
        _reportRepository = reportRepository;
        _amenityRepository = amenityRepository;
        _bookingRepository = bookingRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the booking report query and returns the filtered report.
    /// </summary>
    /// <param name="request">The booking report query.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A booking report containing filters, summary, bookings, and pagination details.</returns>
    public async Task<ReportResponseDto> Handle(
        GetBookingReportQuery request,
        CancellationToken cancellationToken
    )
    {
        string? amenityName = null;

        if (request.AmenityId.HasValue)
        {
            var amenity = await _amenityRepository.GetByIdAsync(
                request.AmenityId.Value,
                cancellationToken
            );
            if (amenity != null)
                amenityName = amenity.Name;
        }

        var reportData = await _reportRepository.GetBookingReportAsync(
            request.AmenityId,
            request.SlotType,
            request.FromDate,
            request.ToDate,
            cancellationToken
        );

        var bookings = await _bookingRepository.GetAllBookingsAsync(
            null,
            request.AmenityId,
            request.SlotType,
            request.FromDate,
            request.ToDate,
            request.PageNumber,
            request.PageSize,
            cancellationToken
        );

        var totalCount = await _bookingRepository.GetAllBookingsCountAsync(
            null,
            request.AmenityId,
            request.SlotType,
            request.FromDate,
            request.ToDate,
            cancellationToken
        );

        var bookingDtos = _mapper.Map<IEnumerable<BookingResponseDto>>(bookings);

        return new ReportResponseDto
        {
            Filters = new ReportFiltersDto
            {
                AmenityId = request.AmenityId,
                AmenityName = amenityName,
                SlotType = request.SlotType,
                FromDate = request.FromDate,
                ToDate = request.ToDate,
            },
            Summary = new ReportSummaryDto
            {
                TotalBookings = reportData.TotalBookings,
                TotalPeople = reportData.TotalPeople,
                ActiveBookings = reportData.ActiveBookings,
                CancelledBookings = reportData.CancelledBookings,
                CompletedBookings = reportData.CompletedBookings,
                UtilizationRate = reportData.UtilizationRate,
            },
            Bookings = bookingDtos.ToList(),
            Pagination = new PaginationDto
            {
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize),
                HasPreviousPage = request.PageNumber > 1,
                HasNextPage =
                    request.PageNumber < (int)Math.Ceiling(totalCount / (double)request.PageSize),
            },
        };
    }
}
