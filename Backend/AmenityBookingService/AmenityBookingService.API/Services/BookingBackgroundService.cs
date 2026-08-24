using AmenityBookingService.Application.Features.Bookings.Commands;
using MediatR;

namespace AmenityBookingService.API.Services;

/// <summary>
/// Background service that periodically processes and completes expired bookings.
/// </summary>
/// <remarks>
/// This service runs as a background task and polls at regular intervals to check for
/// and automatically complete any bookings that have expired.
/// </remarks>
public class BookingBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<BookingBackgroundService> _logger;
    private static readonly TimeSpan PollingInterval = TimeSpan.FromMinutes(1);

    /// <summary>
    /// Initializes a new instance of the <see cref="BookingBackgroundService"/> class.
    /// </summary>
    /// <param name="scopeFactory">The service scope factory used to create scoped dependencies.</param>
    /// <param name="logger">The logger instance for recording service operations and errors.</param>
    public BookingBackgroundService(
        IServiceScopeFactory scopeFactory,
        ILogger<BookingBackgroundService> logger
    )
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    /// <summary>
    /// Executes the background service's main processing loop.
    /// </summary>
    /// <param name="stoppingToken">Cancellation token that signals when the service should stop.</param>
    /// <returns>A task representing the asynchronous execution of the service.</returns>
    /// <remarks>
    /// The service continuously runs until cancellation is requested, processing expired bookings
    /// at regular intervals defined by <see cref="PollingInterval"/>.
    /// </remarks>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("BookingBackgroundService started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessCompletionAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in BookingBackgroundService");
            }

            await Task.Delay(PollingInterval, stoppingToken);
        }
    }

    /// <summary>
    /// Processes and completes all expired bookings.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the current operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <remarks>
    /// This method creates a new scope, resolves the Mediator instance, and sends a
    /// <see cref="CompleteExpiredBookingsCommand"/> to process all expired bookings.
    /// The number of completed bookings is logged for monitoring purposes.
    /// </remarks>
    private async Task ProcessCompletionAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var count = await mediator.Send(new CompleteExpiredBookingsCommand(), cancellationToken);

        if (count > 0)
        {
            _logger.LogInformation("{Count} expired bookings auto-completed", count);
        }
    }
}
