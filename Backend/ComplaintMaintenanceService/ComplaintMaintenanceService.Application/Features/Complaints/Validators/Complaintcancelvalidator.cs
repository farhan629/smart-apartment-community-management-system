using ComplaintMaintenanceService.Application.Common.Constants;
using ComplaintMaintenanceService.Application.Features.Complaints.Commands;
using FluentValidation;

namespace ComplaintMaintenanceService.Application.Features.Complaints.Validators;

public class ComplaintCancelValidator : AbstractValidator<CancelComplaintCommand>
{
    public ComplaintCancelValidator()
    {
        RuleFor(x => x.ComplaintId)
            .NotEmpty()
            .WithMessage(ComplaintConstants.Messages.ComplaintIdRequired);

        RuleFor(x => x.Request.CancellationReason)
            .NotEmpty()
            .WithMessage(ComplaintConstants.Messages.CancellationReasonRequired)
            .MaximumLength(ComplaintConstants.ValidationLimits.CancellationReasonMaxLength)
            .WithMessage(ComplaintConstants.Messages.CancellationReasonMaxLength);
    }
}
