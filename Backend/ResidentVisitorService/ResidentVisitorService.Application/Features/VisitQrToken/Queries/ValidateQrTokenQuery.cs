using MediatR;
using Microsoft.Extensions.Logging;
using ResidentVisitorService.Application.Constants;
using ResidentVisitorService.Application.Features.VisitQrToken.DTOs;
using ResidentVisitorService.Application.Interfaces.Repositories;

namespace ResidentVisitorService.Application.Features.VisitQrToken.Queries;

/// <summary>Query to validate a QR token and return the associated visit details for gate display.</summary>
public class ValidateQrTokenQuery : IRequest<QrTokenValidationResponseDto>
{
    public string Token { get; set; } = string.Empty;
}

/// <summary>Handles the <see cref="ValidateQrTokenQuery"/> request.</summary>
public class ValidateQrTokenQueryHandler
    : IRequestHandler<ValidateQrTokenQuery, QrTokenValidationResponseDto>
{
    private readonly IVisitQrTokenRepository _qrTokenRepository;
    private readonly ILogger<ValidateQrTokenQueryHandler> _logger;

    public ValidateQrTokenQueryHandler(
        IVisitQrTokenRepository qrTokenRepository,
        ILogger<ValidateQrTokenQueryHandler> logger
    )
    {
        _qrTokenRepository = qrTokenRepository;
        _logger = logger;
    }

    public async Task<QrTokenValidationResponseDto> Handle(
        ValidateQrTokenQuery request,
        CancellationToken cancellationToken
    )
    {
        _logger.LogInformation("Validating QR token");

        var qrToken =
            await _qrTokenRepository.GetByTokenAsync(request.Token, cancellationToken)
            ?? throw new KeyNotFoundException(
                ResidentVisitorConstants.Errors.QrTokenNotFoundOrExpired
            );

        var visit =
            qrToken.Visit
            ?? throw new KeyNotFoundException(ResidentVisitorConstants.Errors.VisitForQrNotFound);

        _logger.LogInformation(
            "QR token validated for visit {VisitId}, active: {IsActive}",
            visit.Id,
            qrToken.IsActive
        );

        return new QrTokenValidationResponseDto
        {
            VisitId = visit.Id,
            VisitorName = visit.Visitor?.Name ?? string.Empty,
            VisitorPhoneNumber = visit.Visitor?.PhoneNumber ?? string.Empty,
            VisitorType = visit.Visitor?.VisitorType?.DisplayName ?? string.Empty,
            FlatId = visit.FlatId,
            Purpose = visit.PurposeType?.DisplayName ?? string.Empty,
            Status = visit.Status?.Code ?? string.Empty,
            StartDate = DateOnly.FromDateTime(visit.StartDate),
            EndDate = DateOnly.FromDateTime(visit.EndDate),
            IsTokenActive = qrToken.IsActive,
        };
    }
}
