using AmenityBookingService.Application.Constants;
using AmenityBookingService.Application.Interfaces.Repositories;
using AmenityBookingService.Application.Interfaces.Services;
using AmenityBookingService.Domain.Entities;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shared.SharedLibrary.DTO.Common;
using Shared.SharedLibrary.Exceptions;
using Shared.SharedLibrary.Services;

namespace AmenityBookingService.Application.Features.Bookings.Commands;

/// <summary>
/// Command for creating a new booking for an amenity slot
/// </summary>
public class CreateBookingCommand : IRequest<IdResponseDto>
{
    /// <summary>
    /// Gets or sets the unique identifier of the slot to book
    /// </summary>
    public Guid SlotId { get; set; }

    /// <summary>
    /// Gets or sets the number of people for the booking
    /// </summary>
    public int PeopleCount { get; set; }
}

/// <summary>
/// Handler for processing CreateBookingCommand requests
/// </summary>
public class CreateBookingHandler : IRequestHandler<CreateBookingCommand, IdResponseDto>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly ISlotRepository _slotRepository;
    private readonly IAmenityRepository _amenityRepository;
    private readonly IRefTermRepository _refTermRepository;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotificationClient _notificationClient;
    private readonly IIdentityClient _identityClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<CreateBookingHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateBookingHandler"/> class
    /// </summary>
    /// <param name="bookingRepository">The booking repository</param>
    /// <param name="slotRepository">The slot repository</param>
    /// <param name="amenityRepository">The amenity repository</param>
    /// <param name="refTermRepository">The reference term repository</param>
    /// <param name="mapper">The AutoMapper instance</param>
    /// <param name="currentUserService">The current user service</param>
    /// <param name="notificationClient">The notification client</param>
    /// <param name="identityClient">The identity client</param>
    /// <param name="configuration">The configuration settings</param>
    /// <param name="logger">The logger instance</param>
    public CreateBookingHandler(
        IBookingRepository bookingRepository,
        ISlotRepository slotRepository,
        IAmenityRepository amenityRepository,
        IRefTermRepository refTermRepository,
        IMapper mapper,
        ICurrentUserService currentUserService,
        INotificationClient notificationClient,
        IIdentityClient identityClient,
        IConfiguration configuration,
        ILogger<CreateBookingHandler> logger
    )
    {
        _bookingRepository = bookingRepository;
        _slotRepository = slotRepository;
        _amenityRepository = amenityRepository;
        _refTermRepository = refTermRepository;
        _mapper = mapper;
        _currentUserService = currentUserService;
        _notificationClient = notificationClient;
        _identityClient = identityClient;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Handles the creation of a new booking
    /// </summary>
    /// <param name="request">The create booking command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The ID of the created booking</returns>
    /// <exception cref="UnauthorizedException">Thrown when user is not authenticated</exception>
    /// <exception cref="NotFoundException">Thrown when slot, amenity, or booking status not found</exception>
    /// <exception cref="BadRequestException">Thrown when slot is in the past or invalid slot type</exception>
    /// <exception cref="ConflictException">Thrown when slot is already booked or capacity exceeded</exception>
    public async Task<IdResponseDto> Handle(
        CreateBookingCommand request,
        CancellationToken cancellationToken
    )
    {
        // Validate user authentication
        if (_currentUserService.UserId == Guid.Empty)
            throw new UnauthorizedException(ErrorMessages.UserNotFoundInToken);

        // Retrieve and validate the slot
        var slot = await _slotRepository.GetSlotByIdAsync(request.SlotId, cancellationToken);
        if (slot == null)
            throw new NotFoundException(
                string.Format(ErrorMessages.BookingSlotNotFound, request.SlotId)
            );

        // Check if slot is in the past
        if (slot.SlotDate < DateTime.UtcNow.Date)
            throw new BadRequestException(ErrorMessages.BookingSlotInPast);

        // Retrieve and validate the amenity
        var amenity = await _amenityRepository.GetByIdAsync(slot.AmenityId, cancellationToken);
        if (amenity == null || amenity.SlotType == null)
            throw new NotFoundException(ErrorMessages.BookingSlotTypeNotFound);

        var slotTypeCode = amenity.SlotType.Code;

        // Handle booking logic based on slot type
        if (slotTypeCode == RefTermCodes.Time)
        {
            // Time-based slot: only one person can book and only one booking allowed
            var existingBooking = await _bookingRepository.BookingExistsForSlotAnyUserAsync(
                request.SlotId,
                cancellationToken
            );
            if (existingBooking)
                throw new ConflictException(ErrorMessages.BookingSlotAlreadyBooked);

            if (request.PeopleCount != 1)
                throw new BadRequestException(ErrorMessages.BookingTimeOnlyOnePerson);
        }
        else if (slotTypeCode == RefTermCodes.TimeCount)
        {
            // Time-count based slot: multiple people can book up to max capacity
            var currentBookings = await _slotRepository.GetCurrentBookingsCountForSlotAsync(
                request.SlotId,
                cancellationToken
            );

            var userHasBooking = await _bookingRepository.BookingExistsForSlotAsync(
                request.SlotId,
                _currentUserService.UserId,
                cancellationToken
            );

            if (userHasBooking)
            {
                // Update existing booking
                var existingBooking = await _bookingRepository.GetBookingBySlotAndUserAsync(
                    request.SlotId,
                    _currentUserService.UserId,
                    cancellationToken
                );

                var additionalPeople = request.PeopleCount - existingBooking!.PeopleCount;
                if (additionalPeople > 0 && additionalPeople > slot.MaxCapacity - currentBookings)
                    throw new ConflictException(
                        string.Format(
                            ErrorMessages.BookingNotEnoughCapacity,
                            slot.MaxCapacity - currentBookings,
                            additionalPeople
                        )
                    );

                existingBooking.PeopleCount = request.PeopleCount;
                var updateResult = await _bookingRepository.UpdateBookingAsync(
                    existingBooking,
                    cancellationToken
                );
                return new IdResponseDto { Id = updateResult.Id };
            }

            // Check if there's enough capacity for new booking
            if (currentBookings + request.PeopleCount > slot.MaxCapacity)
                throw new ConflictException(
                    string.Format(
                        ErrorMessages.BookingNotEnoughCapacity,
                        slot.MaxCapacity - currentBookings,
                        request.PeopleCount
                    )
                );
        }
        else
        {
            throw new BadRequestException(
                string.Format(ErrorMessages.UnknownSlotType, slotTypeCode)
            );
        }

        // Get the "Booked" status reference term
        var bookedStatus = await _refTermRepository.GetRefTermByCodeAsync(
            RefTermCodes.Booked,
            cancellationToken
        );
        if (bookedStatus == null)
            throw new NotFoundException(
                string.Format(ErrorMessages.BookingStatusNotFound, RefTermCodes.Booked)
            );

        // Check if there is an inactive (cancelled) booking for this user+slot to reactivate
        var existingInactive = await _bookingRepository.GetInactiveBookingBySlotAndUserAsync(
            request.SlotId,
            _currentUserService.UserId,
            cancellationToken
        );

        AmenityBooking result;
        if (existingInactive != null)
        {
            existingInactive.IsActive = true;
            existingInactive.BookingStatusId = bookedStatus.Id;
            existingInactive.PeopleCount =
                slotTypeCode == RefTermCodes.Time ? 1 : request.PeopleCount;
            existingInactive.CancelledAt = null;
            existingInactive.CancellationReason = null;
            existingInactive.UpdatedAt = DateTime.UtcNow;
            result = await _bookingRepository.UpdateBookingAsync(
                existingInactive,
                cancellationToken
            );
        }
        else
        {
            var booking = _mapper.Map<AmenityBooking>(request);
            booking.UserId = _currentUserService.UserId;
            booking.BookingStatusId = bookedStatus.Id;
            if (slotTypeCode == RefTermCodes.Time)
                booking.PeopleCount = 1;

            result = await _bookingRepository.CreateBookingAsync(booking, cancellationToken);
        }

        // Send notifications
        await SendBookingNotificationsAsync(result, amenity, slot, slotTypeCode, cancellationToken);

        return new IdResponseDto { Id = result.Id };
    }

    /// <summary>
    /// Sends booking confirmation and reminder notifications to the user and admin
    /// </summary>
    /// <param name="booking">The created booking</param>
    /// <param name="amenity">The amenity being booked</param>
    /// <param name="slot">The slot being booked</param>
    /// <param name="slotTypeCode">The type of slot (Time or TimeCount)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    private async Task SendBookingNotificationsAsync(
        AmenityBooking booking,
        Amenity amenity,
        AmenitySlot slot,
        string slotTypeCode,
        CancellationToken cancellationToken
    )
    {
        // Format slot information for notification messages
        var slotDate = slot.SlotDate.ToString("yyyy-MM-dd");
        var startTime = slot.StartTime.ToString(@"hh\:mm");
        var endTime = slot.EndTime.ToString(@"hh\:mm");
        var slotInfo = $"{slot.SlotLabel} @ {slotDate} {startTime}-{endTime}";

        var peopleCountInfo =
            slotTypeCode == RefTermCodes.TimeCount ? $" with {booking.PeopleCount} people" : "";

        // Send booking confirmation to the user
        _logger.LogInformation(
            "[SendBookingNotifications] Sending booking_confirmed for booking {BookingId}",
            booking.Id
        );
        await _notificationClient.SendNotificationAsync(
            booking.UserId,
            "Booking Confirmed",
            $"Your booking for {amenity.Name} ({slotInfo}) has been successfully reserved{peopleCountInfo}.",
            "booking_confirmed",
            amenityBookingId: booking.Id,
            cancellationToken: cancellationToken
        );
        _logger.LogInformation(
            "[SendBookingNotifications] booking_confirmed sent for booking {BookingId}",
            booking.Id
        );

        // Send notification to all admins
        var adminRoleCode = _configuration["AdminSettings:AdminRoleCode"];
        if (!string.IsNullOrWhiteSpace(adminRoleCode))
        {
            _logger.LogInformation(
                "[SendBookingNotifications] Fetching admins with role {AdminRoleCode}",
                adminRoleCode
            );
            var admins = await _identityClient.GetUsersByRoleAsync(
                adminRoleCode,
                cancellationToken
            );
            _logger.LogInformation(
                "[SendBookingNotifications] Found {AdminCount} admins",
                admins.Count
            );

            foreach (var admin in admins)
            {
                _logger.LogInformation(
                    "[SendBookingNotifications] Sending admin_booking_notification to admin {AdminId}",
                    admin.UserId
                );
                await _notificationClient.SendNotificationAsync(
                    admin.UserId,
                    "New Booking",
                    $"User {booking.UserId} booked {amenity.Name} @ {slotInfo}{peopleCountInfo}.",
                    "admin_booking_notification",
                    amenityBookingId: booking.Id,
                    cancellationToken: cancellationToken
                );
            }
        }

        // Schedule a reminder notification 1 hour before the booking
        var reminderTime = DateTime.SpecifyKind(
            slot.SlotDate.Date + slot.StartTime - TimeSpan.FromHours(1),
            DateTimeKind.Utc
        );
        _logger.LogInformation(
            "[SendBookingNotifications] ReminderTime={ReminderTime:O}, UtcNow={UtcNow:O}, IsFuture={IsFuture}",
            reminderTime,
            DateTime.UtcNow,
            reminderTime > DateTime.UtcNow
        );

        if (reminderTime > DateTime.UtcNow)
        {
            _logger.LogInformation(
                "[SendBookingNotifications] Sending booking_reminder with scheduledFor={ScheduledFor:O}",
                reminderTime
            );
            await _notificationClient.SendNotificationAsync(
                booking.UserId,
                "Booking Reminder",
                $"Reminder: Your booking for {amenity.Name} ({slotInfo}) starts in 1 hour.",
                "booking_reminder",
                amenityBookingId: booking.Id,
                scheduledFor: reminderTime,
                cancellationToken: cancellationToken
            );
            _logger.LogInformation("[SendBookingNotifications] booking_reminder call completed");
        }
        else
        {
            _logger.LogWarning(
                "[SendBookingNotifications] SKIPPED booking_reminder because reminderTime is in the past"
            );
        }
    }
}
