using AmenityBookingService.Application.Constants;
using AmenityBookingService.Application.Interfaces.Repositories;
using AmenityBookingService.Application.Interfaces.Services;
using AmenityBookingService.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Configuration;
using Shared.SharedLibrary.DTO.Common;
using Shared.SharedLibrary.Exceptions;
using Shared.SharedLibrary.Services;

namespace AmenityBookingService.Application.Features.Bookings.Commands;

/// <summary>
/// Represents a command to cancel an existing amenity booking.
/// </summary>
public class CancelBookingCommand : IRequest<MessageResponseDto>
{
    /// <summary>
    /// Gets or sets the unique identifier of the booking to cancel.
    /// </summary>
    public Guid BookingId { get; set; }

    /// <summary>
    /// Gets or sets the optional reason for cancellation.
    /// </summary>
    public string? CancellationReason { get; set; }
}

/// <summary>
/// Handles the processing of <see cref="CancelBookingCommand"/> requests.
/// </summary>
public class CancelBookingHandler : IRequestHandler<CancelBookingCommand, MessageResponseDto>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IRefTermRepository _refTermRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotificationClient _notificationClient;
    private readonly IIdentityClient _identityClient;
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="CancelBookingHandler"/> class.
    /// </summary>
    /// <param name="bookingRepository">Repository used to manage booking data.</param>
    /// <param name="refTermRepository">Repository used to retrieve reference terms.</param>
    /// <param name="currentUserService">Service that provides information about the authenticated user.</param>
    /// <param name="notificationClient">Client used to send notifications.</param>
    /// <param name="identityClient">Client used to retrieve user information from the Identity Service.</param>
    /// <param name="configuration">Application configuration.</param>
    public CancelBookingHandler(
        IBookingRepository bookingRepository,
        IRefTermRepository refTermRepository,
        ICurrentUserService currentUserService,
        INotificationClient notificationClient,
        IIdentityClient identityClient,
        IConfiguration configuration
    )
    {
        _bookingRepository = bookingRepository;
        _refTermRepository = refTermRepository;
        _currentUserService = currentUserService;
        _notificationClient = notificationClient;
        _identityClient = identityClient;
        _configuration = configuration;
    }

    /// <summary>
    /// Handles the cancellation of an amenity booking.
    /// </summary>
    /// <param name="request">The booking cancellation request.</param>
    /// <param name="cancellationToken">A token used to cancel the asynchronous operation.</param>
    /// <returns>A response indicating that the booking was cancelled successfully.</returns>
    public async Task<MessageResponseDto> Handle(
        CancelBookingCommand request,
        CancellationToken cancellationToken
    )
    {
        if (_currentUserService.UserId == Guid.Empty)
            throw new UnauthorizedException(ErrorMessages.UserNotFoundInToken);

        var booking = await _bookingRepository.GetBookingByIdAsync(
            request.BookingId,
            cancellationToken
        );

        if (booking == null)
            throw new NotFoundException(
                string.Format(ErrorMessages.BookingNotFound, request.BookingId)
            );

        if (booking.UserId != _currentUserService.UserId)
            throw new ForbiddenException(ErrorMessages.BookingNotOwnedByUser);

        var cancelledStatus = await _refTermRepository.GetRefTermByCodeAsync(
            RefTermCodes.Cancelled,
            cancellationToken
        );

        if (cancelledStatus == null)
            throw new NotFoundException(
                string.Format(ErrorMessages.BookingStatusNotFound, RefTermCodes.Cancelled)
            );

        if (booking.BookingStatusId == cancelledStatus.Id)
            throw new ConflictException(ErrorMessages.BookingAlreadyCancelled);

        if (booking.AmenitySlot != null && booking.AmenitySlot.SlotDate < DateTime.UtcNow.Date)
            throw new BadRequestException(ErrorMessages.BookingCancelledPastDate);

        booking.BookingStatusId = cancelledStatus.Id;
        booking.CancelledAt = DateTime.UtcNow;
        booking.IsActive = false;
        booking.CancellationReason = request.CancellationReason;

        await _bookingRepository.UpdateBookingAsync(booking, cancellationToken);

        // Cancel any pending scheduled notifications (e.g. reminders) for this booking
        await _notificationClient.CancelScheduledNotificationsAsync(booking.Id, cancellationToken);

        await SendCancellationNotificationsAsync(booking, cancellationToken);

        return new MessageResponseDto { Message = SuccessMessages.BookingCancelled };
    }

    /// <summary>
    /// Sends cancellation notifications to the booking owner and all administrators.
    /// </summary>
    /// <param name="booking">The cancelled booking.</param>
    /// <param name="cancellationToken">A token used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous notification operation.</returns>
    private async Task SendCancellationNotificationsAsync(
        AmenityBooking booking,
        CancellationToken cancellationToken
    )
    {
        var amenityName = booking.AmenitySlot?.Amenity?.Name ?? "Amenity";
        var slotLabel = booking.AmenitySlot?.SlotLabel ?? "";
        var slotDate = booking.AmenitySlot?.SlotDate.ToString("yyyy-MM-dd") ?? "";
        var startTime = booking.AmenitySlot?.StartTime.ToString(@"hh\:mm") ?? "";
        var endTime = booking.AmenitySlot?.EndTime.ToString(@"hh\:mm") ?? "";
        var slotInfo = $"{slotLabel} @ {slotDate} {startTime}-{endTime}";

        await _notificationClient.SendNotificationAsync(
            booking.UserId,
            "Booking Cancelled",
            $"Your booking for {amenityName} ({slotInfo}) has been cancelled.",
            "booking_cancelled",
            amenityBookingId: booking.Id,
            cancellationToken: cancellationToken
        );

        var adminRoleCode = _configuration["AdminSettings:AdminRoleCode"];

        if (!string.IsNullOrWhiteSpace(adminRoleCode))
        {
            var admins = await _identityClient.GetUsersByRoleAsync(
                adminRoleCode,
                cancellationToken
            );

            foreach (var admin in admins)
            {
                await _notificationClient.SendNotificationAsync(
                    admin.UserId,
                    "Booking Cancelled",
                    $"User {booking.UserId} cancelled booking for {amenityName} ({slotInfo}).",
                    "admin_booking_cancelled",
                    amenityBookingId: booking.Id,
                    cancellationToken: cancellationToken
                );
            }
        }
    }
}
