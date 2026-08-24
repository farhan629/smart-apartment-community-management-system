using ComplaintMaintenanceService.Application.Common.Constants;
using ComplaintMaintenanceService.Application.Features.Escalation.DTOs;
using ComplaintMaintenanceService.Application.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.SharedLibrary.Exceptions;

namespace ComplaintMaintenanceService.Application.Features.Escalation.Commands;

public class UpdateEscalationCommand : IRequest<EscalationResponseDto>
{
    public Guid ComplaintId { get; set; }
    public Guid UpdatedBy { get; set; }
    public UpdateEscalationRequestDto Request { get; set; } = default!;
}

public class UpdateEscalationCommandHandler
    : IRequestHandler<UpdateEscalationCommand, EscalationResponseDto>
{
    private readonly IComplaintEscalationRepository _escalationRepo;
    private readonly ILogger<UpdateEscalationCommandHandler> _logger;

    public UpdateEscalationCommandHandler(
        IComplaintEscalationRepository escalationRepo,
        ILogger<UpdateEscalationCommandHandler> logger
    )
    {
        _escalationRepo = escalationRepo;
        _logger = logger;
    }

    public async Task<EscalationResponseDto> Handle(
        UpdateEscalationCommand cmd,
        CancellationToken ct
    )
    {
        var escalation =
            await _escalationRepo.GetByComplaintIdAsync(cmd.ComplaintId, ct)
            ?? throw new NotFoundException(
                ComplaintConstants.EscalationMessages.EscalationNotFound
            );

        var now = DateTime.UtcNow;
        escalation.ResolvedAfterEscalation = cmd.Request.ResolvedAfterEscalation;
        escalation.ResolutionDate = cmd.Request.ResolutionDate;
        escalation.UpdatedAt = now;
        escalation.UpdatedBy = cmd.UpdatedBy;

        await _escalationRepo.UpdateAsync(escalation, ct);

        _logger.LogInformation("Escalation updated for complaint {ComplaintId}", cmd.ComplaintId);

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
