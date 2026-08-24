using NotificationService.Domain.Entities;

namespace NotificationService.Application.Common.Interfaces;

/// <summary>
/// Defines the persistence contract for <see cref="Notification"/> entities,
/// covering creation, retrieval, filtering, and update operations.
/// </summary>
public interface INotificationRepository
{
    /// <summary>
    /// Persists a new <see cref="Notification"/> and returns the saved entity with any
    /// store-generated values (e.g. <c>Id</c>, audit timestamps) populated.
    /// </summary>
    Task<Notification> AddAsync(Notification notification);

    /// <summary>
    /// Retrieves a single <see cref="Notification"/> by its unique identifier,
    /// or <c>null</c> if no matching record exists.
    /// </summary>
    Task<Notification?> GetByIdAsync(Guid id);

    /// <summary>
    /// Retrieves a paginated, optionally filtered list of notifications belonging to a specific user,
    /// along with the total record count for pagination metadata.
    /// </summary>
    Task<(List<Notification> Items, int TotalCount)> GetByUserAsync(
        Guid userId,
        bool? isRead,
        int page,
        int limit,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves all notifications belonging to a specific user without pagination.
    /// Intended for bulk operations such as mark-all-as-read or delete-all.
    /// </summary>
    Task<List<Notification>> GetAllByUserAsync(Guid userId);

    /// <summary>
    /// Retrieves all unread notifications belonging to a specific user.
    /// </summary>
    Task<List<Notification>> GetUnreadByUserAsync(Guid userId);

    /// <summary>
    /// Applies changes to an existing <see cref="Notification"/> in the data store.
    /// </summary>
    Task UpdateAsync(Notification notification);

    /// <summary>
    /// Persists all pending changes tracked by the underlying unit of work to the data store.
    /// </summary>
    Task SaveChangesAsync();

    /// <summary>
    /// Cancels all pending scheduled notifications associated with the given amenity booking ID
    /// by setting their <c>IsActive</c> to <c>false</c>.
    /// </summary>
    /// <returns>The number of notifications that were cancelled.</returns>
    Task<int> CancelByAmenityBookingIdAsync(Guid amenityBookingId);

    /// <summary>
    /// Retrieves the <see cref="NotificationTemplate"/> ID for the given notification type,
    /// or <c>null</c> if no matching active template exists.
    /// </summary>
    Task<Guid?> GetTemplateIdByTypeAsync(string notificationType);
}