using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ResidentVisitorService.Application.Constants;
using ResidentVisitorService.Application.Features.Visits.DTOs;
using ResidentVisitorService.Application.Interfaces.Repositories;
using ResidentVisitorService.Application.Interfaces.Services;
using Shared.SharedLibrary.Services;

namespace ResidentVisitorService.Application.Features.Visits.Commands;

/// <summary>Command to approve a pending visit request.</summary>
public class ApproveVisitCommand : IRequest<ApproveRejectVisitResponseDto>
{
    public Guid Id { get; set; }
}

/// <summary>Handles the <see cref="ApproveVisitCommand"/> request.</summary>
public class ApproveVisitCommandHandler
    : IRequestHandler<ApproveVisitCommand, ApproveRejectVisitResponseDto>
{
    private readonly IVisitRepository _visitRepository;
    private readonly IRefTermRepository _refTermRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFlatLookupClient _flatLookupClient;
    private readonly INotificationClient _notificationClient;
    private readonly IVisitQrTokenRepository _visitQrTokenRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ApproveVisitCommandHandler> _logger;

    public ApproveVisitCommandHandler(
        IVisitRepository visitRepository,
        IRefTermRepository refTermRepository,
        ICurrentUserService currentUserService,
        IFlatLookupClient flatLookupClient,
        INotificationClient notificationClient,
        IVisitQrTokenRepository visitQrTokenRepository,
        IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration,
        ILogger<ApproveVisitCommandHandler> logger
    )
    {
        _visitRepository = visitRepository;
        _refTermRepository = refTermRepository;
        _currentUserService = currentUserService;
        _flatLookupClient = flatLookupClient;
        _notificationClient = notificationClient;
        _visitQrTokenRepository = visitQrTokenRepository;
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<ApproveRejectVisitResponseDto> Handle(
        ApproveVisitCommand command,
        CancellationToken cancellationToken
    )
    {
        var visit =
            await _visitRepository.GetByIdAsync(command.Id, cancellationToken)
            ?? throw new KeyNotFoundException(
                string.Format(ResidentVisitorConstants.Errors.VisitNotFound, command.Id)
            );

        if (visit.Status?.Code != ResidentVisitorConstants.VisitStatus.PENDING)
        {
            throw new InvalidOperationException(
                string.Format(
                    ResidentVisitorConstants.Errors.OnlyPendingCanBeApproved,
                    visit.Status?.Code
                )
            );
        }

        var approvedStatus =
            await _refTermRepository.GetByCodeAsync(
                ResidentVisitorConstants.RefSetCodes.VISIT_STATUS,
                ResidentVisitorConstants.VisitStatus.APPROVED,
                cancellationToken
            )
            ?? throw new KeyNotFoundException(
                ResidentVisitorConstants.Errors.ApprovedStatusNotConfigured
            );
        visit.StatusId = approvedStatus.Id;

        visit.ApprovedBy = _currentUserService.UserId;
        visit.ApprovedDate = DateTime.UtcNow;

        await _visitRepository.UpdateAsync(visit, cancellationToken);

        _logger.LogInformation(
            "Visit {VisitId} approved by user {UserId}",
            visit.Id,
            _currentUserService.UserId
        );

        var residentInfo = await _flatLookupClient.GetFlatByUserIdAsync(
            visit.HostUserId,
            cancellationToken
        );

        var qrToken = await _visitQrTokenRepository.GetByVisitIdAsync(visit.Id, cancellationToken);
        string? qrCodeUrl = null;
        if (qrToken != null)
        {
            var request = _httpContextAccessor.HttpContext?.Request;
            if (request != null)
            {
                var scheme = request.Scheme;
                var host = request.Host.Value;
                qrCodeUrl = $"{scheme}://{host}/qrcodes/{qrToken.Token}.png";
            }
            else
            {
                var configBaseUrl = _configuration["FileStorage:BaseUrl"] ?? "http://localhost:5064";
                qrCodeUrl = $"{configBaseUrl.TrimEnd('/')}/qrcodes/{qrToken.Token}.png";
            }
        }

        await _notificationClient.NotifyAsync(
            userId: visit.HostUserId,
            notificationType: ResidentVisitorConstants.NotificationTypes.VISITOR_APPROVED,
            title: ResidentVisitorConstants.NotificationTitles.VISITOR_APPROVED,
            message: string.Format(
                ResidentVisitorConstants.NotificationMessages.VISITOR_APPROVED,
                visit.Visitor?.Name
            ),
            visitId: visit.Id,
            recipientEmail: residentInfo.ResidentEmail,
            recipientName: residentInfo.ResidentName,
            qrCodeUrl: qrCodeUrl,
            visitDate: visit.StartDate.ToString("yyyy-MM-dd"),
            cancellationToken: cancellationToken
        );

        return new ApproveRejectVisitResponseDto
        {
            Id = visit.Id,
            Status = ResidentVisitorConstants.VisitStatus.APPROVED,
        };
    }
}

/// <summary>Command to reject a pending visit request.</summary>
public class RejectVisitCommand : IRequest<ApproveRejectVisitResponseDto>
{
    public Guid Id { get; set; }
    public RejectVisitRequestDto Request { get; set; } = null!;
}

/// <summary>Handles the <see cref="RejectVisitCommand"/> request.</summary>
public class RejectVisitCommandHandler
    : IRequestHandler<RejectVisitCommand, ApproveRejectVisitResponseDto>
{
    private readonly IVisitRepository _visitRepository;
    private readonly IRefTermRepository _refTermRepository;
    private readonly IFlatLookupClient _flatLookupClient;
    private readonly INotificationClient _notificationClient;
    private readonly ILogger<RejectVisitCommandHandler> _logger;

    public RejectVisitCommandHandler(
        IVisitRepository visitRepository,
        IRefTermRepository refTermRepository,
        IFlatLookupClient flatLookupClient,
        INotificationClient notificationClient,
        ILogger<RejectVisitCommandHandler> logger
    )
    {
        _visitRepository = visitRepository;
        _refTermRepository = refTermRepository;
        _flatLookupClient = flatLookupClient;
        _notificationClient = notificationClient;
        _logger = logger;
    }

    public async Task<ApproveRejectVisitResponseDto> Handle(
        RejectVisitCommand command,
        CancellationToken cancellationToken
    )
    {
        var visit =
            await _visitRepository.GetByIdAsync(command.Id, cancellationToken)
            ?? throw new KeyNotFoundException(
                string.Format(ResidentVisitorConstants.Errors.VisitNotFound, command.Id)
            );

        if (visit.Status?.Code != ResidentVisitorConstants.VisitStatus.PENDING)
        {
            throw new InvalidOperationException(
                string.Format(
                    ResidentVisitorConstants.Errors.OnlyPendingCanBeRejected,
                    visit.Status?.Code
                )
            );
        }

        var rejectedStatus =
            await _refTermRepository.GetByCodeAsync(
                ResidentVisitorConstants.RefSetCodes.VISIT_STATUS,
                ResidentVisitorConstants.VisitStatus.REJECTED,
                cancellationToken
            )
            ?? throw new KeyNotFoundException(
                ResidentVisitorConstants.Errors.RejectedStatusNotConfigured
            );

        visit.StatusId = rejectedStatus.Id;
        visit.RejectionReason = command.Request.RejectionReason.Trim();

        await _visitRepository.UpdateAsync(visit, cancellationToken);

        _logger.LogInformation("Visit {VisitId} rejected", visit.Id);

        var residentInfo = await _flatLookupClient.GetFlatByUserIdAsync(
            visit.HostUserId,
            cancellationToken
        );

        await _notificationClient.NotifyAsync(
            userId: visit.HostUserId,
            notificationType: ResidentVisitorConstants.NotificationTypes.VISITOR_REJECTED,
            title: ResidentVisitorConstants.NotificationTitles.VISITOR_REJECTED,
            message: string.Format(
                ResidentVisitorConstants.NotificationMessages.VISITOR_REJECTED,
                visit.Visitor?.Name
            ),
            visitId: visit.Id,
            recipientEmail: residentInfo.ResidentEmail,
            recipientName: residentInfo.ResidentName,
            cancellationToken: cancellationToken
        );

        return new ApproveRejectVisitResponseDto
        {
            Id = visit.Id,
            Status = ResidentVisitorConstants.VisitStatus.REJECTED,
        };
    }
}

/// <summary>Command to record a visitor check-in.</summary>
public class CheckInVisitCommand : IRequest<CheckInOutResponseDto>
{
    public Guid Id { get; set; }
}

/// <summary>Handles the <see cref="CheckInVisitCommand"/> request.</summary>
public class CheckInVisitCommandHandler
    : IRequestHandler<CheckInVisitCommand, CheckInOutResponseDto>
{
    private readonly IVisitRepository _visitRepository;
    private readonly IRefTermRepository _refTermRepository;
    private readonly IFlatLookupClient _flatLookupClient;
    private readonly INotificationClient _notificationClient;
    private readonly ILogger<CheckInVisitCommandHandler> _logger;

    public CheckInVisitCommandHandler(
        IVisitRepository visitRepository,
        IRefTermRepository refTermRepository,
        IFlatLookupClient flatLookupClient,
        INotificationClient notificationClient,
        ILogger<CheckInVisitCommandHandler> logger
    )
    {
        _visitRepository = visitRepository;
        _refTermRepository = refTermRepository;
        _flatLookupClient = flatLookupClient;
        _notificationClient = notificationClient;
        _logger = logger;
    }

    public async Task<CheckInOutResponseDto> Handle(
        CheckInVisitCommand command,
        CancellationToken cancellationToken
    )
    {
        var visit =
            await _visitRepository.GetByIdAsync(command.Id, cancellationToken)
            ?? throw new KeyNotFoundException(
                string.Format(ResidentVisitorConstants.Errors.VisitNotFound, command.Id)
            );

        if (visit.Status?.Code != ResidentVisitorConstants.VisitStatus.APPROVED)
        {
            throw new InvalidOperationException(
                string.Format(
                    ResidentVisitorConstants.Errors.OnlyApprovedCanBeCheckedIn,
                    visit.Status?.Code
                )
            );
        }

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

        _logger.LogInformation(
            "Visitor checked in for visit {VisitId} at {CheckInTime}",
            visit.Id,
            visit.CheckInTime
        );

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
            Status = ResidentVisitorConstants.VisitStatus.CHECKED_IN,
            CheckInTime = visit.CheckInTime,
        };
    }
}

/// <summary>Command to record a visitor check-out.</summary>
public class CheckOutVisitCommand : IRequest<CheckInOutResponseDto>
{
    public Guid Id { get; set; }
}

/// <summary>Handles the <see cref="CheckOutVisitCommand"/> request.</summary>
public class CheckOutVisitCommandHandler
    : IRequestHandler<CheckOutVisitCommand, CheckInOutResponseDto>
{
    private readonly IVisitRepository _visitRepository;
    private readonly IRefTermRepository _refTermRepository;
    private readonly IFlatLookupClient _flatLookupClient;
    private readonly INotificationClient _notificationClient;
    private readonly ILogger<CheckOutVisitCommandHandler> _logger;

    public CheckOutVisitCommandHandler(
        IVisitRepository visitRepository,
        IRefTermRepository refTermRepository,
        IFlatLookupClient flatLookupClient,
        INotificationClient notificationClient,
        ILogger<CheckOutVisitCommandHandler> logger
    )
    {
        _visitRepository = visitRepository;
        _refTermRepository = refTermRepository;
        _flatLookupClient = flatLookupClient;
        _notificationClient = notificationClient;
        _logger = logger;
    }

    public async Task<CheckInOutResponseDto> Handle(
        CheckOutVisitCommand command,
        CancellationToken cancellationToken
    )
    {
        var visit =
            await _visitRepository.GetByIdAsync(command.Id, cancellationToken)
            ?? throw new KeyNotFoundException(
                string.Format(ResidentVisitorConstants.Errors.VisitNotFound, command.Id)
            );

        if (visit.Status?.Code != ResidentVisitorConstants.VisitStatus.CHECKED_IN)
        {
            throw new InvalidOperationException(
                string.Format(
                    ResidentVisitorConstants.Errors.OnlyCheckedInCanBeCheckedOut,
                    visit.Status?.Code
                )
            );
        }

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

        _logger.LogInformation(
            "Visitor checked out for visit {VisitId} at {CheckOutTime}",
            visit.Id,
            visit.CheckOutTime
        );

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
            Status = ResidentVisitorConstants.VisitStatus.CHECKED_OUT,
            CheckInTime = visit.CheckInTime,
            CheckOutTime = visit.CheckOutTime,
        };
    }
}

/// <summary>Command to cancel a visit (soft delete).</summary>
public class CancelVisitCommand : IRequest
{
    public Guid Id { get; set; }
}

/// <summary>Handles the <see cref="CancelVisitCommand"/> request.</summary>
public class CancelVisitCommandHandler : IRequestHandler<CancelVisitCommand>
{
    private readonly IVisitRepository _visitRepository;
    private readonly ILogger<CancelVisitCommandHandler> _logger;

    public CancelVisitCommandHandler(
        IVisitRepository visitRepository,
        ILogger<CancelVisitCommandHandler> logger
    )
    {
        _visitRepository = visitRepository;
        _logger = logger;
    }

    public async Task Handle(CancelVisitCommand command, CancellationToken cancellationToken)
    {
        var visit =
            await _visitRepository.GetByIdAsync(command.Id, cancellationToken)
            ?? throw new KeyNotFoundException(
                string.Format(ResidentVisitorConstants.Errors.VisitNotFound, command.Id)
            );

        var cancellableStatuses = new[]
        {
            ResidentVisitorConstants.VisitStatus.PENDING,
            ResidentVisitorConstants.VisitStatus.APPROVED,
        };

        if (!cancellableStatuses.Contains(visit.Status?.Code))
        {
            throw new InvalidOperationException(
                string.Format(
                    ResidentVisitorConstants.Errors.VisitCannotBeCancelled,
                    visit.Status?.Code
                )
            );
        }

        await _visitRepository.SoftDeleteAsync(command.Id, cancellationToken);

        _logger.LogInformation("Cancelled visit {VisitId}", command.Id);
    }
}
