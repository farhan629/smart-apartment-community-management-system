using Shared.SharedLibrary.DTO.Common;

namespace AmenityBookingService.Application.Features.Bookings.DTO;

/// <summary>
/// Represents the details of an amenity booking.
/// </summary>
public class BookingResponseDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the booking.
    /// </summary>
    public Guid BookingId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the user who made the booking.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the name of the booked amenity.
    /// </summary>
    public string AmenityName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the slot type of the booked amenity.
    /// </summary>
    public string SlotType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the label of the booked slot.
    /// </summary>
    public string SlotLabel { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date of the booked slot.
    /// </summary>
    public DateTime SlotDate { get; set; }

    /// <summary>
    /// Gets or sets the start time of the booked slot.
    /// </summary>
    public TimeSpan StartTime { get; set; }

    /// <summary>
    /// Gets or sets the end time of the booked slot.
    /// </summary>
    public TimeSpan EndTime { get; set; }

    /// <summary>
    /// Gets or sets the number of people included in the booking.
    /// </summary>
    public int PeopleCount { get; set; }

    /// <summary>
    /// Gets or sets the current status of the booking.
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date and time when the booking was created.
    /// </summary>
    public DateTime BookedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the booking was cancelled, if applicable.
    /// </summary>
    public DateTime? CancelledAt { get; set; }
}

/// <summary>
/// Represents a paginated collection of amenity bookings.
/// </summary>
public class BookingListResponseDto
{
    /// <summary>
    /// Gets or sets the collection of booking records.
    /// </summary>
    public List<BookingResponseDto> Data { get; set; } = new();

    /// <summary>
    /// Gets or sets the pagination information for the booking list.
    /// </summary>
    public PaginationDto Pagination { get; set; } = new();
}
