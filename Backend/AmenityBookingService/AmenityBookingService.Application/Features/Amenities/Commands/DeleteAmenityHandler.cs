using AmenityBookingService.Application.Constants;
using AmenityBookingService.Application.Interfaces.Repositories;
using MediatR;
using Shared.SharedLibrary.DTO.Common;
using Shared.SharedLibrary.Exceptions;
using Shared.SharedLibrary.Services;

namespace AmenityBookingService.Application.Features.Amenities.Commands;

/// <summary>
/// Command for soft-deleting an amenity by its unique identifier.
/// </summary>
public class DeleteAmenityCommand : IRequest<MessageResponseDto>
{
    /// <summary>
    /// Gets the unique identifier of the amenity to delete.
    /// </summary>
    public Guid AmenityId { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteAmenityCommand"/> class.
    /// </summary>
    /// <param name="amenityId">The unique identifier of the amenity to delete.</param>
    public DeleteAmenityCommand(Guid amenityId)
    {
        AmenityId = amenityId;
    }
}

/// <summary>
/// Handler for processing <see cref="DeleteAmenityCommand"/> to soft-delete an amenity.
/// </summary>
/// <remarks>
/// Business Rules:
/// - Amenity must exist
/// - Amenity cannot have any active slots or booking history
/// - User must be authenticated
/// </remarks>
public class DeleteAmenityHandler : IRequestHandler<DeleteAmenityCommand, MessageResponseDto>
{
    private readonly IAmenityRepository _repository;
    private readonly ICurrentUserService _currentUserService;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteAmenityHandler"/> class.
    /// </summary>
    /// <param name="repository">The amenity repository for data access.</param>
    /// <param name="currentUserService">The current user service for authentication context.</param>
    public DeleteAmenityHandler(
        IAmenityRepository repository,
        ICurrentUserService currentUserService
    )
    {
        _repository = repository;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// Handles the soft-deletion of an amenity with validation and business rules enforcement.
    /// </summary>
    /// <param name="request">The delete amenity command containing the amenity ID.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>A message response indicating successful deletion.</returns>
    /// <exception cref="UnauthorizedException">Thrown when user is not authenticated.</exception>
    /// <exception cref="NotFoundException">Thrown when the amenity does not exist.</exception>
    /// <exception cref="ConflictException">Thrown when the amenity has active slots or bookings.</exception>
    /// <exception cref="BadRequestException">Thrown when the delete operation fails.</exception>
    public async Task<MessageResponseDto> Handle(
        DeleteAmenityCommand request,
        CancellationToken cancellationToken
    )
    {
        if (_currentUserService.UserId == Guid.Empty)
            throw new UnauthorizedException(ErrorMessages.UserNotFoundInToken);

        var amenity = await _repository.GetByIdAsync(request.AmenityId, cancellationToken);
        if (amenity == null)
            throw new NotFoundException(
                string.Format(ErrorMessages.AmenityNotFound, request.AmenityId)
            );

        var hasSlotsOrBookings = await _repository.HasSlotsOrBookingsAsync(
            request.AmenityId,
            cancellationToken
        );
        if (hasSlotsOrBookings)
            throw new ConflictException(ErrorMessages.AmenityHasSlotsOrBookings);

        var deleted = await _repository.DeleteAsync(request.AmenityId, cancellationToken);
        if (!deleted)
            throw new BadRequestException(ErrorMessages.AmenityDeleteFailed);

        return new MessageResponseDto { Message = SuccessMessages.DeletedMessage };
    }
}
