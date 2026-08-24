namespace IdentityService.Application.Interfaces.Services;

/// <summary>
/// Defines operations for sending SMS messages.
/// </summary>
public interface ISmsService
{
    /// <summary>
    /// Sends an SMS message.
    /// </summary>
    /// <param name="toPhoneNumber">The recipient's phone number.</param>
    /// <param name="message">The message content.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SendSmsAsync(string toPhoneNumber, string message);
}
