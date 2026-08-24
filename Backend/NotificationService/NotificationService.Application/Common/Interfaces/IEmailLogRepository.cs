using NotificationService.Domain.Entities;

namespace NotificationService.Application.Common.Interfaces;

/// <summary>
/// Defines the persistence contract for email template lookups and email delivery log writes
/// within the <c>NotificationService</c>.
/// </summary>
public interface IEmailLogRepository
{
    /// <summary>
    /// Retrieves an <see cref="EmailTemplate"/> by its type identifier, or <c>null</c> if no
    /// matching template exists.
    /// </summary>
    Task<EmailTemplate?> GetTemplateByTypeAsync(string emailType);

    /// <summary>
    /// Stages a new <see cref="EmailLog"/> entry for insertion into the data store.
    /// Call <see cref="SaveChangesAsync"/> to commit the change.
    /// </summary>
    Task AddEmailLogAsync(EmailLog emailLog);

    /// <summary>
    /// Persists all pending changes tracked by the underlying unit of work to the data store.
    /// </summary>
    Task SaveChangesAsync();
}