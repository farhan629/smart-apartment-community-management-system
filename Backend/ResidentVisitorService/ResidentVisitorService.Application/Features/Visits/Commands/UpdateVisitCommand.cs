using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ResidentVisitorService.Application.Constants;
using ResidentVisitorService.Application.Features.Visits.DTOs;
using ResidentVisitorService.Application.Interfaces.Repositories;

namespace ResidentVisitorService.Application.Features.Visits.Commands;

public class UpdateVisitCommand : IRequest<VisitResponseDto>
{
    public Guid Id { get; set; }
    public UpdateVisitRequestDto Request { get; set; } = null!;
}

public class UpdateVisitCommandHandler : IRequestHandler<UpdateVisitCommand, VisitResponseDto>
{
    private readonly IVisitRepository _visitRepository;
    private readonly IRefTermRepository _refTermRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateVisitCommandHandler> _logger;

    public UpdateVisitCommandHandler(
        IVisitRepository visitRepository,
        IRefTermRepository refTermRepository,
        IMapper mapper,
        ILogger<UpdateVisitCommandHandler> logger
    )
    {
        _visitRepository = visitRepository;
        _refTermRepository = refTermRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<VisitResponseDto> Handle(
        UpdateVisitCommand command,
        CancellationToken cancellationToken
    )
    {
        var visit =
            await _visitRepository.GetByIdAsync(command.Id, cancellationToken)
            ?? throw new KeyNotFoundException(
                string.Format(ResidentVisitorConstants.Errors.VisitNotFound, command.Id)
            );

        if (visit.Status?.Code != ResidentVisitorConstants.VisitStatus.PENDING)
            throw new InvalidOperationException(
                ResidentVisitorConstants.Errors.OnlyPendingCanBeUpdated
            );

        var request = command.Request;

        if (request.PurposeTypeId.HasValue && request.PurposeTypeId.Value != visit.PurposeTypeId)
        {
            _ =
                await _refTermRepository.GetByIdAsync(
                    request.PurposeTypeId.Value,
                    cancellationToken
                )
                ?? throw new KeyNotFoundException(
                    string.Format(
                        ResidentVisitorConstants.Errors.PurposeTypeNotFound,
                        request.PurposeTypeId
                    )
                );
            visit.PurposeTypeId = request.PurposeTypeId.Value;
        }

        if (request.StartDate.HasValue)
            visit.StartDate = request.StartDate.Value.ToDateTime(
                TimeOnly.MinValue,
                DateTimeKind.Utc
            );

        if (request.EndDate.HasValue)
            visit.EndDate = request.EndDate.Value.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);

        if (visit.EndDate < visit.StartDate)
            throw new InvalidOperationException(
                ResidentVisitorConstants.Errors.EndDateBeforeStartDate
            );

        await _visitRepository.UpdateAsync(visit, cancellationToken);

        _logger.LogInformation("Updated visit {VisitId}", visit.Id);

        return _mapper.Map<VisitResponseDto>(visit);
    }
}
