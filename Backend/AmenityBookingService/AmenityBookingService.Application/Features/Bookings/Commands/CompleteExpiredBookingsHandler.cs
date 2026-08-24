using AmenityBookingService.Application.Constants;
using AmenityBookingService.Application.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AmenityBookingService.Application.Features.Bookings.Commands;

public class CompleteExpiredBookingsCommand : IRequest<int>
{
}

public class CompleteExpiredBookingsHandler : IRequestHandler<CompleteExpiredBookingsCommand, int>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IRefTermRepository _refTermRepository;
    private readonly ILogger<CompleteExpiredBookingsHandler> _logger;

    public CompleteExpiredBookingsHandler(
        IBookingRepository bookingRepository,
        IRefTermRepository refTermRepository,
        ILogger<CompleteExpiredBookingsHandler> logger
    )
    {
        _bookingRepository = bookingRepository;
        _refTermRepository = refTermRepository;
        _logger = logger;
    }

    public async Task<int> Handle(
        CompleteExpiredBookingsCommand request,
        CancellationToken cancellationToken
    )
    {
        var completedStatus = await _refTermRepository.GetRefTermByCodeAsync(
            RefTermCodes.Completed,
            cancellationToken
        );

        if (completedStatus == null)
        {
            _logger.LogWarning("COMPLETED status ref term not found");
            return 0;
        }

        var bookingsToComplete = await _bookingRepository.GetExpiredBookingsAsync(cancellationToken);

        foreach (var booking in bookingsToComplete)
        {
            booking.BookingStatusId = completedStatus.Id;
            await _bookingRepository.UpdateBookingAsync(booking, cancellationToken);
            _logger.LogInformation("Booking {BookingId} auto-completed", booking.Id);
        }

        return bookingsToComplete.Count;
    }
}
