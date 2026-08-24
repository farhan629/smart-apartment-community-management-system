using ComplaintMaintenanceService.Application.Common.Constants;
using ComplaintMaintenanceService.Application.Features.Complaints.Commands;
using FluentValidation;

namespace ComplaintMaintenanceService.Application.Features.Complaints.Validators;

public class ComplaintStatusUpdateValidator : AbstractValidator<UpdateComplaintStatusCommand>
{
    public ComplaintStatusUpdateValidator()
    {
        RuleFor(x => x.ComplaintId)
            .NotEmpty()
            .WithMessage(ComplaintConstants.Messages.ComplaintIdRequired);

        RuleFor(x => x.Request.Status)
            .NotEmpty()
            .WithMessage(ComplaintConstants.Messages.StatusRequired)
            .Must(s =>
                s == ComplaintConstants.StatusCodes.InProgress
                || s == ComplaintConstants.StatusCodes.Resolved
            )
            .WithMessage(ComplaintConstants.Messages.InvalidStatusValue);
    }
}
