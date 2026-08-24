using ComplaintMaintenanceService.Application.Common.Constants;
using ComplaintMaintenanceService.Application.Features.Escalation.DTOs;
using ComplaintMaintenanceService.Application.Interfaces.Repositories;
using MediatR;
using Shared.SharedLibrary.Exceptions;

namespace ComplaintMaintenanceService.Application.Features.Escalation.Queries;

public class GetEscalationQuery : IRequest<EscalationResponseDto>
{
    public Guid ComplaintId { get; set; }
}

public class GetEscalationQueryHandler : IRequestHandler<GetEscalationQuery, EscalationResponseDto>
{
    private readonly IComplaintEscalationRepository _escalationRepo;

    public GetEscalationQueryHandler(IComplaintEscalationRepository escalationRepo)
    {
        _escalationRepo = escalationRepo;
    }

    public async Task<EscalationResponseDto> Handle(GetEscalationQuery query, CancellationToken ct)
    {
        var escalation =
            await _escalationRepo.GetByComplaintIdAsync(query.ComplaintId, ct)
            ?? throw new NotFoundException(
                ComplaintConstants.EscalationMessages.EscalationNotFound
            );

        return new EscalationResponseDto
        {
            EscalationId = escalation.Id,
            ComplaintId = escalation.ComplaintId,
            EscalatedBy = escalation.EscalatedBy,
            EscalatedTo = escalation.EscalatedTo,
            EscalationReason = escalation.EscalationReason,
            EscalationDate = escalation.EscalationDate,
            ResolvedAfterEscalation = escalation.ResolvedAfterEscalation,
            ResolutionDate = escalation.ResolutionDate,
        };
    }
}
