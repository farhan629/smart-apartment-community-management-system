using IdentityService.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace IdentityService.Infrastructure.Services;

/// <summary>
/// Sends SMS messages using the Twilio service.
/// </summary>
public class SmsService : ISmsService
{
    private readonly TwilioSettings _settings;
    private readonly ILogger<SmsService> _logger;
    private static bool _twilioInitialized;

    /// <summary>
    /// Creates a new instance of the <see cref="SmsService"/> class.
    /// </summary>
    /// <param name="settings">The Twilio configuration.</param>
    /// <param name="logger">The logger instance.</param>
    public SmsService(IOptions<TwilioSettings> settings, ILogger<SmsService> logger)
    {
        _settings = settings.Value;
        _logger = logger;

        if (!_twilioInitialized)
        {
            TwilioClient.Init(_settings.AccountSid, _settings.AuthToken);
            _twilioInitialized = true;
        }
    }

    /// <inheritdoc/>
    public async Task SendSmsAsync(string to, string body)
    {
        if (!to.StartsWith('+'))
            to = _settings.DefaultCountryCode + to.TrimStart('0');

        var fromNumber = _settings.FromNumber;
        if (!fromNumber.StartsWith('+'))
            fromNumber = _settings.DefaultCountryCode + fromNumber.TrimStart('0');

        var msg = await MessageResource.CreateAsync(
            body: body,
            from: new Twilio.Types.PhoneNumber(fromNumber),
            to: new Twilio.Types.PhoneNumber(to)
        );

        _logger.LogInformation(
            "SMS sent to {To} — Sid: {Sid}, Status: {Status}",
            to,
            msg.Sid,
            msg.Status
        );
    }
}
