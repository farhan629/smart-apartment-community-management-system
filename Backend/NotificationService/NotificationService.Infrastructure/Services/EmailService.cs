using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NotificationService.Application.Common.Interfaces;
using NotificationService.Application.Constants;

namespace NotificationService.Infrastructure.Services;

/// <summary>
/// SMTP-based implementation of <see cref="IEmailService"/> that sends HTML transactional
/// emails using configuration values from the <c>EmailSettings</c> section.
/// </summary>
public class EmailService : IEmailService
{
    private readonly string _smtpHost;
    private readonly int _smtpPort;
    private readonly string _senderEmail;
    private readonly string _senderName;
    private readonly string _smtpPassword;
    private readonly ILogger<EmailService> _logger;
    private static readonly HttpClient _httpClient = new HttpClient();

    /// <summary>
    /// Initializes a new instance of <see cref="EmailService"/>, reading and validating
    /// all required SMTP configuration from the <c>EmailSettings</c> configuration section.
    /// </summary>
    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _logger = logger;
        var s = configuration.GetSection("EmailSettings");

        _smtpHost =
            s["Host"]
            ?? throw new InvalidOperationException(
                NotificationConstants.Errors.EMAIL_SETTINGS_HOST_MISSING
            );
        _smtpPort = int.Parse(s["Port"] ?? "587");
        _senderEmail =
            s["SenderEmail"]
            ?? throw new InvalidOperationException(
                NotificationConstants.Errors.EMAIL_SETTINGS_SENDER_MISSING
            );
        _senderName = s["SenderName"] ?? "Smart Apartment System";
        _smtpPassword =
            s["Password"]
            ?? throw new InvalidOperationException(
                NotificationConstants.Errors.EMAIL_SETTINGS_PASSWORD_MISSING
            );
    }

    /// <summary>
    /// Sends an HTML email to a single recipient over SMTP with SSL enabled.
    /// Supports embedding images from URLs using CID (Content-ID) references.
    /// </summary>
    public async Task SendEmailAsync(
        string recipientEmail,
        string recipientName,
        string subject,
        string body
    )
    {
        var resources = new List<(MemoryStream Stream, string Cid, string ContentType)>();

        try
        {
            using var client = new SmtpClient(_smtpHost, _smtpPort)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(_senderEmail, _smtpPassword),
            };

            using var message = new MailMessage
            {
                From = new MailAddress(_senderEmail, _senderName),
                Subject = subject,
                IsBodyHtml = true,
            };

            message.To.Add(new MailAddress(recipientEmail, recipientName));

            // Check for image URLs in the HTML body
            var imgRegex = new Regex(@"src=""(http[s]?://[^""]+)""", RegexOptions.IgnoreCase);
            var matches = imgRegex.Matches(body);

            _logger.LogInformation(
                "SendEmailAsync: found {Count} image URL matches in email body",
                matches.Count
            );

            if (matches.Count > 0)
            {
                // Process each image URL and embed it as a linked resource
                foreach (Match match in matches)
                {
                    var imageUrl = match.Groups[1].Value;
                    _logger.LogInformation("SendEmailAsync: processing image URL: {Url}", imageUrl);

                    try
                    {
                        // Download the image
                        var bytes = await _httpClient.GetByteArrayAsync(imageUrl);
                        var cid = "img_" + Guid.NewGuid().ToString("N");

                        // Replace the URL with CID reference in the HTML body
                        body = body.Replace(imageUrl, $"cid:{cid}");

                        _logger.LogInformation(
                            "SendEmailAsync: successfully downloaded image and generated CID: {Cid}",
                            cid
                        );

                        // Determine content type based on file extension
                        var ms = new MemoryStream(bytes);
                        var contentType = MediaTypeNames.Image.Png;

                        if (
                            imageUrl.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
                            || imageUrl.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)
                        )
                        {
                            contentType = MediaTypeNames.Image.Jpeg;
                        }
                        else if (imageUrl.EndsWith(".gif", StringComparison.OrdinalIgnoreCase))
                        {
                            contentType = MediaTypeNames.Image.Gif;
                        }
                        else if (imageUrl.EndsWith(".svg", StringComparison.OrdinalIgnoreCase))
                        {
                            contentType = "image/svg+xml";
                        }

                        resources.Add((ms, cid, contentType));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(
                            ex,
                            "Failed to download and embed image from {Url}. The image will be displayed as an external link.",
                            imageUrl
                        );
                    }
                }

                // Create the HTML view with embedded images
                var htmlView = AlternateView.CreateAlternateViewFromString(
                    body,
                    null,
                    MediaTypeNames.Text.Html
                );

                // Attach all downloaded images as linked resources
                foreach (var res in resources)
                {
                    var linkedResource = new LinkedResource(res.Stream, res.ContentType)
                    {
                        ContentId = res.Cid,
                    };
                    htmlView.LinkedResources.Add(linkedResource);
                }

                message.AlternateViews.Add(htmlView);
            }
            else
            {
                // No images to embed, just set the body directly
                message.Body = body;
            }

            await client.SendMailAsync(message);
            _logger.LogInformation(
                "Email sent to {Email} with subject '{Subject}'",
                recipientEmail,
                subject
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to send email to {Email} with subject '{Subject}'",
                recipientEmail,
                subject
            );
            throw new InvalidOperationException(
                $"{NotificationConstants.Errors.EMAIL_SEND_FAILED} {recipientEmail}",
                ex
            );
        }
        finally
        {
            // Clean up memory streams
            foreach (var res in resources)
            {
                res.Stream.Dispose();
            }
        }
    }
}
