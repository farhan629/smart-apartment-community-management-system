using Shared.SharedLibrary.DTO.Common;

namespace NotificationService.Application.Notifications.DTOs;

/// <summary>
/// Response payload for a paginated list of notifications.
/// </summary>
public class GetNotificationsResponse
{
    /// <summary>The notifications for the current page.</summary>
    public List<NotificationDto> Items { get; set; } = [];

    /// <summary>Pagination metadata (page, page size, total count).</summary>
    public PaginationDto Pagination { get; set; } = new();
}
