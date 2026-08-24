using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ResidentVisitorService.Application.Constants;
using ResidentVisitorService.Application.Features.Visits.DTOs;
using ResidentVisitorService.Application.Interfaces.Repositories;
using ResidentVisitorService.Application.Interfaces.Services;
using ResidentVisitorService.Domain.Entities;
using Shared.SharedLibrary.Services;

public class CheckInVisitHardCodeCommand : IRequest<CheckInOutResponseDto>
{
    public Guid Id { get; set; }
}

public class CheckInVisitHardCodeCommandHandler
    : IRequestHandler<CheckInVisitHardCodeCommand, CheckInOutResponseDto>
{
    private readonly IVisitRepository _visitRepository;
    private readonly IRefTermRepository _refTermRepository;
    private readonly IFlatLookupClient _flatLookupClient;
    private readonly INotificationClient _notificationClient;

    public CheckInVisitHardCodeCommandHandler(
        IVisitRepository visitRepository,
        IRefTermRepository refTermRepository,
        IFlatLookupClient flatLookupClient,
        INotificationClient notificationClient
    )
    {
        _visitRepository = visitRepository;
        _refTermRepository = refTermRepository;
        _flatLookupClient = flatLookupClient;
        _notificationClient = notificationClient;
    }

    public async Task<CheckInOutResponseDto> Handle(
        CheckInVisitHardCodeCommand command,
        CancellationToken cancellationToken
    )
    {
        var visit =
            await _visitRepository.GetByIdAsync(command.Id, cancellationToken)
            ?? throw new KeyNotFoundException(
                string.Format(ResidentVisitorConstants.Errors.VisitNotFound, command.Id)
            );

        if (visit.Status?.Code != ResidentVisitorConstants.VisitStatus.APPROVED)
            throw new InvalidOperationException(
                string.Format(
                    ResidentVisitorConstants.Errors.OnlyApprovedCanBeCheckedIn,
                    visit.Status?.Code
                )
            );

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

        return new CheckInOutResponseDto
        {
            Id = visit.Id,
            Status = checkInStatus.Code,
            CheckInTime = visit.CheckInTime,
            CheckOutTime = visit.CheckOutTime,
        };
    }
}

public class CheckOutVisitHardCodeCommand : IRequest<CheckInOutResponseDto>
{
    public Guid Id { get; set; }
}

public class CheckOutVisitHardCodeCommandHandler
    : IRequestHandler<CheckOutVisitHardCodeCommand, CheckInOutResponseDto>
{
    private readonly IVisitRepository _visitRepository;
    private readonly IRefTermRepository _refTermRepository;
    private readonly IFlatLookupClient _flatLookupClient;
    private readonly INotificationClient _notificationClient;

    public CheckOutVisitHardCodeCommandHandler(
        IVisitRepository visitRepository,
        IRefTermRepository refTermRepository,
        IFlatLookupClient flatLookupClient,
        INotificationClient notificationClient
    )
    {
        _visitRepository = visitRepository;
        _refTermRepository = refTermRepository;
        _flatLookupClient = flatLookupClient;
        _notificationClient = notificationClient;
    }

    public async Task<CheckInOutResponseDto> Handle(
        CheckOutVisitHardCodeCommand command,
        CancellationToken cancellationToken
    )
    {
        var visit =
            await _visitRepository.GetByIdAsync(command.Id, cancellationToken)
            ?? throw new KeyNotFoundException(
                string.Format(ResidentVisitorConstants.Errors.VisitNotFound, command.Id)
            );

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

        return new CheckInOutResponseDto
        {
            Id = visit.Id,
            Status = checkOutStatus.Code,
            CheckInTime = visit.CheckInTime,
            CheckOutTime = visit.CheckOutTime,
        };
    }
}