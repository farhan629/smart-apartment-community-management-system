using AmenityBookingService.Application.Constants;
using AmenityBookingService.Application.Interfaces.Repositories;
using MediatR;
using Shared.SharedLibrary.DTO.Common;
using Shared.SharedLibrary.Exceptions;
using Shared.SharedLibrary.Services;

namespace AmenityBookingService.Application.Features.Slots.Commands;

/// <summary>
/// Command to soft-delete a slot by marking it as inactive.
/// </summary>
public class DeleteSlotCommand : IRequest<MessageResponseDto>
{
    /// <summary>
    /// Gets the unique identifier of the slot to delete.
    /// </summary>
    public Guid SlotId { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteSlotCommand"/> class.
    /// </summary>
    /// <param name="slotId">The unique identifier of the slot to delete.</param>
    public DeleteSlotCommand(Guid slotId)
    {
        SlotId = slotId;
    }
}

/// <summary>
/// Handler for processing <see cref="DeleteSlotCommand"/> to soft-delete a slot.
/// </summary>
/// <remarks>
/// Business Rules:
/// - Slot must exist
/// - Slot cannot have existing bookings
/// - User must be authenticated
/// - Performs soft-delete (marks as inactive) rather than hard delete
/// </remarks>
public class DeleteSlotHandler : IRequestHandler<DeleteSlotCommand, MessageResponseDto>
{
    private readonly ISlotRepository _repository;
    private readonly ICurrentUserService _currentUserService;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteSlotHandler"/> class.
    /// </summary>
    /// <param name="repository">The slot repository for data access.</param>
    /// <param name="currentUserService">The current user service for authentication context.</param>
    public DeleteSlotHandler(ISlotRepository repository, ICurrentUserService currentUserService)
    {
        _repository = repository;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// Handles the soft-deletion of a slot with validation and business rules enforcement.
    /// </summary>
    /// <param name="request">The delete slot command containing the slot ID.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>A message response indicating successful deletion.</returns>
    /// <exception cref="UnauthorizedException">Thrown when user is not authenticated.</exception>
    /// <exception cref="NotFoundException">Thrown when the slot does not exist.</exception>
    /// <exception cref="ConflictException">Thrown when the slot has existing bookings.</exception>
    /// <exception cref="BadRequestException">Thrown when the delete operation fails.</exception>
    public async Task<MessageResponseDto> Handle(
        DeleteSlotCommand request,
        CancellationToken cancellationToken
    )
    {
        if (_currentUserService.UserId == Guid.Empty)
            throw new UnauthorizedException(ErrorMessages.UserNotFoundInToken);

        var slot = await _repository.GetSlotByIdAsync(request.SlotId, cancellationToken);
        if (slot == null)
            throw new NotFoundException(
                string.Format(ErrorMessages.SlotTypeNotFound, request.SlotId)
            );

        var hasBookings = await _repository.SlotHasBookingsAsync(request.SlotId, cancellationToken);
        if (hasBookings)
            throw new ConflictException(ErrorMessages.SlotHasBookings);

        var deleted = await _repository.DeleteSlotAsync(request.SlotId, cancellationToken);
        if (!deleted)
            throw new BadRequestException(ErrorMessages.SlotDeleteFailed);

        return new MessageResponseDto { Message = SuccessMessages.SlotDeleteMessage };
    }
}
