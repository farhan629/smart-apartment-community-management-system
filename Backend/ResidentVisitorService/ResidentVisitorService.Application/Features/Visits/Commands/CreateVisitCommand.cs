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

namespace ResidentVisitorService.Application.Features.Visits.Commands;

/// <summary>Command to register a new visit.</summary>
public class CreateVisitCommand : IRequest<VisitCreateResponseDto>
{
    public CreateVisitRequestDto Request { get; set; } = null!;
}

/// <summary>Handles the <see cref="CreateVisitCommand"/> request.</summary>
public class CreateVisitCommandHandler : IRequestHandler<CreateVisitCommand, VisitCreateResponseDto>
{
    private readonly IVisitRepository _visitRepository;
    private readonly IVisitorRepository _visitorRepository;
    private readonly IRefTermRepository _refTermRepository;
    private readonly IFlatLookupClient _flatLookupClient;
    private readonly ICurrentUserService _currentUserService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly INotificationClient _notificationClient;
    private readonly IVisitQrTokenRepository _visitQrTokenRepository;
    private readonly IQrCodeService _qrCodeService;
    private readonly ILogger<CreateVisitCommandHandler> _logger;

    public CreateVisitCommandHandler(
        IVisitRepository visitRepository,
        IVisitorRepository visitorRepository,
        IRefTermRepository refTermRepository,
        IFlatLookupClient flatLookupClient,
        ICurrentUserService currentUserService,
        IHttpContextAccessor httpContextAccessor,
        INotificationClient notificationClient,
        IVisitQrTokenRepository visitQrTokenRepository,
        IQrCodeService qrCodeService,
        ILogger<CreateVisitCommandHandler> logger
    )
    {
        _visitRepository = visitRepository;
        _visitorRepository = visitorRepository;
        _refTermRepository = refTermRepository;
        _flatLookupClient = flatLookupClient;
        _currentUserService = currentUserService;
        _httpContextAccessor = httpContextAccessor;
        _notificationClient = notificationClient;
        _visitQrTokenRepository = visitQrTokenRepository;
        _qrCodeService = qrCodeService;
        _logger = logger;
    }

    public async Task<VisitCreateResponseDto> Handle(
        CreateVisitCommand command,
        CancellationToken cancellationToken
    )
    {
        var request = command.Request;

        Visitor visitor;
        if (request.VisitorId.HasValue)
        {
            visitor =
                await _visitorRepository.GetByIdAsync(request.VisitorId.Value, cancellationToken)
                ?? throw new KeyNotFoundException(
                    string.Format(
                        ResidentVisitorConstants.Errors.VisitorNotFound,
                        request.VisitorId
                    )
                );
        }
        else
        {
            if (
                await _visitorRepository.PhoneNumberExistsAsync(
                    request.Visitor!.PhoneNumber,
                    null,
                    cancellationToken
                )
            )
            {
                visitor =
                    await _visitorRepository.GetByPhoneNumberAsync(
                        request.Visitor.PhoneNumber,
                        cancellationToken
                    )
                    ?? throw new InvalidOperationException(
                        ResidentVisitorConstants.Errors.VisitorLookupFailed
                    );
            }
            else
            {
                _ =
                    await _refTermRepository.GetByIdAsync(
                        request.Visitor.VisitorTypeId,
                        cancellationToken
                    )
                    ?? throw new KeyNotFoundException(
                        string.Format(
                            ResidentVisitorConstants.Errors.VisitorTypeNotFound,
                            request.Visitor.VisitorTypeId
                        )
                    );

                var newVisitor = new Visitor
                {
                    Id = Guid.NewGuid(),
                    Name = request.Visitor.Name.Trim(),
                    PhoneNumber = request.Visitor.PhoneNumber.Trim(),
                    Email = request.Visitor.Email?.Trim(),
                    VisitorTypeId = request.Visitor.VisitorTypeId,
                };

                visitor = await _visitorRepository.AddAsync(newVisitor, cancellationToken);
                _logger.LogInformation(
                    "Inline visitor {VisitorId} created during visit registration",
                    visitor.Id
                );
            }
        }

        var purposeType =
            await _refTermRepository.GetByIdAsync(request.PurposeTypeId, cancellationToken)
            ?? throw new KeyNotFoundException(
                string.Format(
                    ResidentVisitorConstants.Errors.PurposeTypeNotFound,
                    request.PurposeTypeId
                )
            );

        Guid flatId;
        Guid hostUserId;
        FlatInfoDto flatInfo;

        var userRole = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value;

        if (userRole == ResidentVisitorConstants.Roles.Security)
        {
            flatInfo = await _flatLookupClient.GetFlatByBlockAndNumberAsync(
                request.BlockNumber,
                request.FlatNumber,
                cancellationToken
            );

            flatId = flatInfo.FlatId;
            hostUserId = flatInfo.HostUserId;
        }
        else
        {
            flatInfo = await _flatLookupClient.GetFlatByUserIdAsync(
                _currentUserService.UserId,
                cancellationToken
            );

            flatId = flatInfo.FlatId;
            hostUserId = _currentUserService.UserId;
        }

        var isSecurityPost = userRole == ResidentVisitorConstants.Roles.Security;

        var initialStatusCode = isSecurityPost
            ? ResidentVisitorConstants.VisitStatus.PENDING
            : ResidentVisitorConstants.VisitStatus.APPROVED;

        var initialStatus =
            await _refTermRepository.GetByCodeAsync(
                ResidentVisitorConstants.RefSetCodes.VISIT_STATUS,
                initialStatusCode,
                cancellationToken
            )
            ?? throw new KeyNotFoundException(
                isSecurityPost
                    ? ResidentVisitorConstants.Errors.PendingStatusNotConfigured
                    : ResidentVisitorConstants.Errors.ApprovedStatusNotConfigured
            );

        var visit = new Visit
        {
            Id = Guid.NewGuid(),
            VisitorId = visitor.Id,
            HostUserId = hostUserId,
            FlatId = flatId,
            PurposeTypeId = request.PurposeTypeId,
            StatusId = initialStatus.Id,
            StartDate = request.StartDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc),
            EndDate = request.EndDate.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc),
        };

        var created = await _visitRepository.AddAsync(visit, cancellationToken);

        var qrToken = new ResidentVisitorService.Domain.Entities.VisitQrToken
        {
            Id = Guid.NewGuid(),
            VisitId = created.Id,
            Token = Guid.NewGuid().ToString("N"),
        };

        await _visitQrTokenRepository.AddAsync(qrToken, cancellationToken);

        var qrImageUrl = await _qrCodeService.GenerateAndStoreAsync(
            qrToken.Token,
            cancellationToken
        );

        _logger.LogInformation(
            "QR {Token} generated for visit {VisitId}",
            qrToken.Token,
            created.Id
        );

        _logger.LogInformation(
            "Visit {VisitId} created for visitor {VisitorId} by user {HostUserId}",
            created.Id,
            visitor.Id,
            created.HostUserId
        );

        await _notificationClient.NotifyAsync(
            userId: hostUserId,
            notificationType: isSecurityPost
                ? ResidentVisitorConstants.NotificationTypes.VISITOR_AT_GATE
                : ResidentVisitorConstants.NotificationTypes.VISITOR_REGISTERED,
            title: isSecurityPost
                ? ResidentVisitorConstants.NotificationTitles.VISITOR_AT_GATE
                : ResidentVisitorConstants.NotificationTitles.VISITOR_REGISTERED,
            message: string.Format(
                isSecurityPost
                    ? ResidentVisitorConstants.NotificationMessages.VISITOR_AT_GATE
                    : ResidentVisitorConstants.NotificationMessages.VISITOR_REGISTERED,
                visitor.Name
            ),
            visitId: created.Id,
            recipientEmail: isSecurityPost ? flatInfo.ResidentEmail : visitor.Email,
            recipientName: isSecurityPost ? flatInfo.ResidentName : visitor.Name,
            qrCodeUrl: isSecurityPost ? null : qrImageUrl,
            visitDate: isSecurityPost ? null : created.StartDate.ToString("yyyy-MM-dd"),
            cancellationToken: cancellationToken
        );

        return new VisitCreateResponseDto
        {
            Id = created.Id,
            Status = initialStatusCode,
            VisitorName = visitor.Name,
            FlatId = created.FlatId,
            Purpose = purposeType.DisplayName,
            StartDate = DateOnly.FromDateTime(created.StartDate),
            EndDate = DateOnly.FromDateTime(created.EndDate),
        };
    }
}
