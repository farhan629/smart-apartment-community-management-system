using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ResidentVisitorService.Application.Constants;
using ResidentVisitorService.Application.Features.VisitQrToken.DTOs;
using ResidentVisitorService.Application.Interfaces.Repositories;
using ResidentVisitorService.Domain.Entities;

namespace ResidentVisitorService.Application.Features.VisitQrToken.Commands;

/// <summary>Command to generate a QR pass for an approved visit.</summary>
public class GenerateQrTokenCommand : IRequest<VisitQrTokenResponseDto>
{
    public Guid VisitId { get; set; }
}

/// <summary>Handles the <see cref="GenerateQrTokenCommand"/> request.</summary>
public class GenerateQrTokenCommandHandler
    : IRequestHandler<GenerateQrTokenCommand, VisitQrTokenResponseDto>
{
    private readonly IVisitRepository _visitRepository;
    private readonly IVisitQrTokenRepository _qrTokenRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GenerateQrTokenCommandHandler> _logger;

    public GenerateQrTokenCommandHandler(
        IVisitRepository visitRepository,
        IVisitQrTokenRepository qrTokenRepository,
        IMapper mapper,
        ILogger<GenerateQrTokenCommandHandler> logger
    )
    {
        _visitRepository = visitRepository;
        _qrTokenRepository = qrTokenRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<VisitQrTokenResponseDto> Handle(
        GenerateQrTokenCommand command,
        CancellationToken cancellationToken
    )
    {
        var visit =
            await _visitRepository.GetByIdAsync(command.VisitId, cancellationToken)
            ?? throw new KeyNotFoundException(
                string.Format(ResidentVisitorConstants.Errors.VisitNotFound, command.VisitId)
            );

        if (visit.Status?.Code != ResidentVisitorConstants.VisitStatus.APPROVED)
        {
            throw new InvalidOperationException(
                ResidentVisitorConstants.Errors.QrOnlyForApprovedVisits
            );
        }

        var existing = await _qrTokenRepository.GetByVisitIdAsync(
            command.VisitId,
            cancellationToken
        );
        if (existing is not null && existing.IsActive)
        {
            throw new InvalidOperationException(
                ResidentVisitorConstants.Errors.ActiveQrTokenAlreadyExists
            );
        }

        var qrToken = new Domain.Entities.VisitQrToken
        {
            Id = Guid.NewGuid(),
            VisitId = command.VisitId,
            Token = Guid.NewGuid().ToString(ResidentVisitorConstants.QrToken.TokenFormat),
            IsActive = true,
        };

        var created = await _qrTokenRepository.AddAsync(qrToken, cancellationToken);

        _logger.LogInformation(
            "Generated QR token {TokenId} for visit {VisitId}",
            created.Id,
            command.VisitId
        );

        return _mapper.Map<VisitQrTokenResponseDto>(created);
    }
}
