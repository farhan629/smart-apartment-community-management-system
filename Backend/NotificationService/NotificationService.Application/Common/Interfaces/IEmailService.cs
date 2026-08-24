namespace NotificationService.Application.Common.Interfaces;

public interface IEmailService
{
    /// <summary>
    /// Sends an HTML email. Throws on failure — caller decides whether to swallow.
    /// </summary>
    Task SendEmailAsync(string recipientEmail, string recipientName, string subject, string body);
}