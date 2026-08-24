namespace IdentityService.Infrastructure.Services;

/// <summary>
/// Represents the Twilio configuration settings.
/// </summary>
public class TwilioSettings
{
    /// <summary>
    /// Gets or sets the Twilio account SID.
    /// </summary>
    public string AccountSid { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Twilio authentication token.
    /// </summary>
    public string AuthToken { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Twilio phone number used to send SMS messages.
    /// </summary>
    public string FromNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the default country code for phone numbers.
    /// </summary>
    public string DefaultCountryCode { get; set; } = "+91";
}
