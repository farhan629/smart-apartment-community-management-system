using AmenityBookingService.Application.Features.Bookings.DTO;
using AmenityBookingService.Application.Interfaces.Repositories;
using AutoMapper;
using MediatR;
using Shared.SharedLibrary.Constants;
using Shared.SharedLibrary.DTO.Common;
using Shared.SharedLibrary.Exceptions;
using Shared.SharedLibrary.Services;

namespace AmenityBookingService.Application.Features.Bookings.Queries;

/// <summary>
/// Represents a query to retrieve the booking history of the authenticated user.
/// </summary>
public class GetBookingHistoryQuery : IRequest<BookingListResponseDto>
{
    /// <summary>
    /// Gets or sets the booking status used to filter the results.
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// Gets or sets the start date used to filter bookings.
    /// </summary>
    public DateTime? FromDate { get; set; }

    /// <summary>
    /// Gets or sets the end date used to filter bookings.
    /// </summary>
    public DateTime? ToDate { get; set; }

    /// <summary>
    /// Gets or sets the page number for paginated results.
    /// </summary>
    public int PageNumber { get; set; } = PaginationConstants.DefaultPageNumber;

    /// <summary>
    /// Gets or sets the number of records to return per page.
    /// </summary>
    public int PageSize { get; set; } = PaginationConstants.DefaultPageSize;
}

/// <summary>
/// Handles <see cref="GetBookingHistoryQuery"/> requests.
/// </summary>
public class GetBookingHistoryHandler
    : IRequestHandler<GetBookingHistoryQuery, BookingListResponseDto>
{
    private readonly IBookingRepository _repository;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetBookingHistoryHandler"/> class.
    /// </summary>
    /// <param name="repository">Repository used to retrieve booking information.</param>
    /// <param name="mapper">Mapper used to convert entities into response DTOs.</param>
    /// <param name="currentUserService">Service that provides information about the authenticated user.</param>
    public GetBookingHistoryHandler(
        IBookingRepository repository,
        IMapper mapper,
        ICurrentUserService currentUserService
    )
    {
        _repository = repository;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// Handles the request to retrieve the authenticated user's booking history.
    /// </summary>
    /// <param name="request">The booking history query.</param>
    /// <param name="cancellationToken">A token used to cancel the asynchronous operation.</param>
    /// <returns>
    /// A paginated collection of bookings that match the specified search criteria.
    /// </returns>
    public async Task<BookingListResponseDto> Handle(
        GetBookingHistoryQuery request,
        CancellationToken cancellationToken
    )
    {
        if (_currentUserService.UserId == Guid.Empty)
            throw new UnauthorizedException(
                AmenityBookingService.Application.Constants.ErrorMessages.UserNotFoundInToken
            );

        var pageNumber =
            request.PageNumber < PaginationConstants.MinPageNumber
                ? PaginationConstants.DefaultPageNumber
                : request.PageNumber;

        var pageSize =
            request.PageSize < PaginationConstants.MinPageSize
                ? PaginationConstants.DefaultPageSize
                : request.PageSize;

        if (pageSize > PaginationConstants.MaxPageSize)
            pageSize = PaginationConstants.MaxPageSize;

        var bookings = await _repository.GetUserBookingsAsync(
            _currentUserService.UserId,
            request.Status,
            request.FromDate,
            request.ToDate,
            pageNumber,
            pageSize,
            cancellationToken
        );

        var totalCount = await _repository.GetUserBookingsCountAsync(
            _currentUserService.UserId,
            request.Status,
            request.FromDate,
            request.ToDate,
            cancellationToken
        );

        var data = _mapper.Map<List<BookingResponseDto>>(bookings);

        var totalPages = totalCount > 0 ? (int)Math.Ceiling((double)totalCount / pageSize) : 0;

        return new BookingListResponseDto
        {
            Data = data,
            Pagination = new PaginationDto
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                HasPreviousPage = pageNumber > PaginationConstants.MinPageNumber,
                HasNextPage = pageNumber < totalPages,
            },
        };
    }
}
