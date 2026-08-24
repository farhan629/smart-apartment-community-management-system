using MediatR;
using ResidentVisitorService.Application.Constants;
using ResidentVisitorService.Application.Interfaces.Repositories;
using ResidentVisitorService.Application.Interfaces.Services;

namespace ResidentVisitorService.Application.Features.Visits.Commands;

public class CheckInVisitByTokenCommand : IRequest<Unit>
{
    public string Token { get; set; } = null!;
}

public class CheckInVisitByTokenCommandHandler : IRequestHandler<CheckInVisitByTokenCommand, Unit>
{
    private readonly IVisitQrTokenRepository _qrTokenRepository;
    private readonly IVisitRepository _visitRepository;
    private readonly IRefTermRepository _refTermRepository;
    private readonly IFlatLookupClient _flatLookupClient;
    private readonly INotificationClient _notificationClient;

    public CheckInVisitByTokenCommandHandler(
        IVisitQrTokenRepository qrTokenRepository,
        IVisitRepository visitRepository,
        IRefTermRepository refTermRepository,
        IFlatLookupClient flatLookupClient,
        INotificationClient notificationClient
    )
    {
        _qrTokenRepository = qrTokenRepository;
        _visitRepository = visitRepository;
        _refTermRepository = refTermRepository;
        _flatLookupClient = flatLookupClient;
        _notificationClient = notificationClient;
    }

    public async Task<Unit> Handle(
        CheckInVisitByTokenCommand command,
        CancellationToken cancellationToken
    )
    {
        var qrToken =
            await _qrTokenRepository.GetByTokenAsync(command.Token, cancellationToken)
            ?? throw new KeyNotFoundException(ResidentVisitorConstants.Errors.QrTokenNotFound);

        var visit =
            await _visitRepository.GetByIdAsync(qrToken.VisitId, cancellationToken)
            ?? throw new KeyNotFoundException(ResidentVisitorConstants.Errors.VisitNotFound);

        if (visit.CheckInTime.HasValue)
            throw new InvalidOperationException(ResidentVisitorConstants.Errors.AlreadyCheckedIn);

        var checkInStatus =
            await _refTermRepository.GetByCodeAsync(
                ResidentVisitorConstants.RefSetCodes.VISIT_STATUS,
                ResidentVisitorConstants.VisitStatus.CHECKED_IN,
                cancellationToken
            )
            ?? throw new KeyNotFoundException(
                ResidentVisitorConstants.Errors.CheckedInStatusNotConfigured
            );

        visit.StatusId = checkInStatus.Id;
        visit.CheckInTime = DateTime.UtcNow;
        await _visitRepository.UpdateAsync(visit, cancellationToken);

        var residentInfo = await _flatLookupClient.GetFlatByUserIdAsync(
            visit.HostUserId,
            cancellationToken
        );
        await _notificationClient.NotifyAsync(
            userId: visit.HostUserId,
            notificationType: ResidentVisitorConstants.NotificationTypes.VISITOR_CHECKED_IN,
            title: ResidentVisitorConstants.NotificationTitles.VISITOR_CHECKED_IN,
            message: string.Format(
                ResidentVisitorConstants.NotificationMessages.VISITOR_CHECKED_IN,
                visit.Visitor?.Name
            ),
            visitId: visit.Id,
            recipientEmail: residentInfo.ResidentEmail,
            recipientName: residentInfo.ResidentName,
            cancellationToken: cancellationToken
        );

        return Unit.Value;
    }
}

public class CheckOutVisitByTokenCommand : IRequest<Unit>
{
    public string Token { get; set; } = null!;
}

public class CheckOutVisitByTokenCommandHandler : IRequestHandler<CheckOutVisitByTokenCommand, Unit>
{
    private readonly IVisitQrTokenRepository _qrTokenRepository;
    private readonly IVisitRepository _visitRepository;
    private readonly IRefTermRepository _refTermRepository;
    private readonly IFlatLookupClient _flatLookupClient;
    private readonly INotificationClient _notificationClient;

    public CheckOutVisitByTokenCommandHandler(
        IVisitQrTokenRepository qrTokenRepository,
        IVisitRepository visitRepository,
        IRefTermRepository refTermRepository,
        IFlatLookupClient flatLookupClient,
        INotificationClient notificationClient
    )
    {
        _qrTokenRepository = qrTokenRepository;
        _visitRepository = visitRepository;
        _refTermRepository = refTermRepository;
        _flatLookupClient = flatLookupClient;
        _notificationClient = notificationClient;
    }

    public async Task<Unit> Handle(
        CheckOutVisitByTokenCommand command,
        CancellationToken cancellationToken
    )
    {
        var qrToken =
            await _qrTokenRepository.GetByTokenAsync(command.Token, cancellationToken)
            ?? throw new KeyNotFoundException(ResidentVisitorConstants.Errors.QrTokenNotFound);

        var visit =
            await _visitRepository.GetByIdAsync(qrToken.VisitId, cancellationToken)
            ?? throw new KeyNotFoundException(ResidentVisitorConstants.Errors.VisitNotFound);

        if (!visit.CheckInTime.HasValue)
            throw new InvalidOperationException(ResidentVisitorConstants.Errors.NotCheckedIn);

        if (visit.CheckOutTime.HasValue)
            throw new InvalidOperationException(ResidentVisitorConstants.Errors.AlreadyCheckedOut);

        var checkOutStatus =
            await _refTermRepository.GetByCodeAsync(
                ResidentVisitorConstants.RefSetCodes.VISIT_STATUS,
                ResidentVisitorConstants.VisitStatus.CHECKED_OUT,
                cancellationToken
            )
            ?? throw new KeyNotFoundException(
                ResidentVisitorConstants.Errors.CheckedOutStatusNotConfigured
            );

        visit.StatusId = checkOutStatus.Id;
        visit.CheckOutTime = DateTime.UtcNow;
        await _visitRepository.UpdateAsync(visit, cancellationToken);

        var residentInfo = await _flatLookupClient.GetFlatByUserIdAsync(
            visit.HostUserId,
            cancellationToken
        );
        await _notificationClient.NotifyAsync(
            userId: visit.HostUserId,
            notificationType: ResidentVisitorConstants.NotificationTypes.VISITOR_CHECKED_OUT,
            title: ResidentVisitorConstants.NotificationTitles.VISITOR_CHECKED_OUT,
            message: string.Format(
                ResidentVisitorConstants.NotificationMessages.VISITOR_CHECKED_OUT,
                visit.Visitor?.Name
            ),
            visitId: visit.Id,
            recipientEmail: residentInfo.ResidentEmail,
            recipientName: residentInfo.ResidentName,
            cancellationToken: cancellationToken
        );

        return Unit.Value;
    }
}
