namespace AmenityBookingService.Application.Features.Bookings.DTO;

/// <summary>
/// Represents the request payload for creating a new amenity booking.
/// </summary>
public class CreateBookingRequestDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the amenity slot to be booked.
    /// </summary>
    public Guid SlotId { get; set; }

    /// <summary>
    /// Gets or sets the number of people included in the booking.
    /// Defaults to <c>1</c>.
    /// </summary>
    public int PeopleCount { get; set; } = 1;
}
