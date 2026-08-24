namespace NotificationService.Domain.Enums;

/// <summary>
/// Represents the delivery status of an outbound email within the <c>NotificationService</c>.
/// Used to track and audit the lifecycle of each email dispatch attempt recorded in <c>EmailLog</c>.
/// </summary>
public enum EmailStatus
{
    /// <summary>
    /// The email has been queued but not yet dispatched.
    /// This is the initial state assigned when an <c>EmailLog</c> entry is first created.
    /// </summary>
    Pending = 0,

    /// <summary>
    /// The email was successfully delivered by the email service.
    /// </summary>
    Sent = 1,

    /// <summary>
    /// The email dispatch attempt failed.
    /// The associated <c>EmailLog.ErrorMessage</c> will contain the failure reason.
    /// </summary>
    Failed = 2
}