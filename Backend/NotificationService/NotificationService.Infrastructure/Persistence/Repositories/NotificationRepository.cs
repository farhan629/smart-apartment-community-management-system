using Microsoft.EntityFrameworkCore;
using NotificationService.Application.Common.Interfaces;
using NotificationService.Domain.Entities;
using NotificationService.Infrastructure.Persistence.DBContext;

namespace NotificationService.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of <see cref="INotificationRepository"/>, providing full CRUD
/// and paged query support for <see cref="Notification"/> entities against <see cref="AppDbContext"/>.
/// </summary>
public class NotificationRepository : INotificationRepository
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Initializes a new instance of <see cref="NotificationRepository"/>.
    /// </summary>
    public NotificationRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Stages a new <see cref="Notification"/> for insertion and returns the same instance
    /// so callers can access store-generated values after <see cref="SaveChangesAsync"/> is called.
    /// </summary>
    public async Task<Notification> AddAsync(Notification notification)
    {
        await _context.Notifications.AddAsync(notification);
        return notification;
    }

    /// <summary>
    /// Retrieves a single active <see cref="Notification"/> by its unique identifier,
    /// or <c>null</c> if no matching active record exists.
    /// </summary>
    public Task<Notification?> GetByIdAsync(Guid id) =>
        _context.Notifications.AsNoTracking().FirstOrDefaultAsync(n => n.Id == id && n.IsActive);

    /// <summary>
    /// Retrieves a paginated, optionally read-state-filtered list of active notifications
    /// for the specified user, ordered by <c>CreatedAt</c> descending.
    /// </summary>
    public async Task<(List<Notification> Items, int TotalCount)> GetByUserAsync(
        Guid userId,
        bool? isRead,
        int page,
        int limit,
        CancellationToken cancellationToken = default
    )
    {
        var query = _context
            .Notifications.AsNoTracking()
            .Where(n => n.UserId == userId && n.IsActive);

        if (isRead.HasValue)
            query = query.Where(n => n.IsRead == isRead.Value);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(n => n.CreatedAt)
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    /// <summary>
    /// Retrieves all active notifications belonging to the specified user as a tracked list.
    /// Intended for bulk mutation operations such as mark-all-as-read or delete-all.
    /// </summary>
    public Task<List<Notification>> GetAllByUserAsync(Guid userId) =>
        _context.Notifications.Where(n => n.UserId == userId && n.IsActive).ToListAsync();

    /// <summary>
    /// Retrieves all active unread notifications belonging to the specified user as a tracked list.
    /// Intended for bulk mutation operations such as mark-all-as-read.
    /// </summary>
    public Task<List<Notification>> GetUnreadByUserAsync(Guid userId) =>
        _context
            .Notifications.Where(n => n.UserId == userId && n.IsActive && !n.IsRead)
            .ToListAsync();

    /// <summary>
    /// Attaches an existing <see cref="Notification"/> to the change tracker in the
    /// <see cref="Microsoft.EntityFrameworkCore.EntityState.Modified"/> state so its changes
    /// are included in the next <see cref="SaveChangesAsync"/> call.
    /// </summary>
    public Task UpdateAsync(Notification notification)
    {
        _context.Notifications.Update(notification);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Persists all pending changes tracked by <see cref="AppDbContext"/> to the database.
    /// </summary>
    public Task SaveChangesAsync() => _context.SaveChangesAsync();

    /// <summary>
    /// Cancels all pending scheduled notifications for the given amenity booking ID.
    /// </summary>
    public async Task<int> CancelByAmenityBookingIdAsync(Guid amenityBookingId)
    {
        var notifications = await _context
            .Notifications.Where(n =>
                n.AmenityBookingId == amenityBookingId && n.IsActive && n.Status == "pending"
            )
            .ToListAsync();

        foreach (var n in notifications)
        {
            n.IsActive = false;
            n.UpdatedAt = DateTime.UtcNow;
        }

        if (notifications.Count > 0)
            await _context.SaveChangesAsync();

        return notifications.Count;
    }

    /// <summary>
    /// Looks up the ID of the active <see cref="NotificationTemplate"/> matching the given
    /// notification type, or <c>null</c> if no active template exists for that type.
    /// </summary>
    public Task<Guid?> GetTemplateIdByTypeAsync(string notificationType) =>
        _context
            .NotificationTemplates.AsNoTracking()
            .Where(t => t.NotificationType == notificationType && t.IsActive)
            .Select(t => (Guid?)t.Id)
            .FirstOrDefaultAsync();
}
