using Microsoft.EntityFrameworkCore;
using NotificationService.Application.Common.Interfaces;
using NotificationService.Domain.Entities;
using NotificationService.Infrastructure.Persistence.DBContext;

namespace NotificationService.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IEmailLogRepository"/>, providing template lookups
/// and email delivery log persistence against <see cref="AppDbContext"/>.
/// </summary>
public class EmailLogRepository : IEmailLogRepository
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Initializes a new instance of <see cref="EmailLogRepository"/>.
    /// </summary>
    /// <param name="context">The <see cref="AppDbContext"/> used for all database operations.</param>
    public EmailLogRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves the active <see cref="EmailTemplate"/> matching the given type code,
    /// or <c>null</c> if no active template exists for that type.
    /// </summary>
    public Task<EmailTemplate?> GetTemplateByTypeAsync(string emailType) =>
        _context
            .EmailTemplates.AsNoTracking()
            .FirstOrDefaultAsync(t => t.EmailType == emailType && t.IsActive);

    /// <summary>
    /// Stages a new <see cref="EmailLog"/> entry for insertion.
    /// Call <see cref="SaveChangesAsync"/> to commit the entry to the database.
    /// </summary>
    public async Task AddEmailLogAsync(EmailLog emailLog)
    {
        await _context.EmailLogs.AddAsync(emailLog);
    }

    /// <summary>
    /// Persists all pending changes tracked by <see cref="AppDbContext"/> to the database.
    /// </summary>
    public Task SaveChangesAsync() => _context.SaveChangesAsync();
}
