using FluentValidation;
using ResidentVisitorService.Application.Constants;
using ResidentVisitorService.Application.Features.Visits.Commands;

namespace ResidentVisitorService.Application.Features.Visits.Validators;

public class RejectVisitCommandValidator : AbstractValidator<RejectVisitCommand>
{
    public RejectVisitCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(ResidentVisitorConstants.Validation.VisitIdRequired);

        RuleFor(x => x.Request.RejectionReason)
            .NotEmpty()
            .WithMessage(ResidentVisitorConstants.Validation.RejectionReasonRequired)
            .MaximumLength(500)
            .WithMessage(ResidentVisitorConstants.Validation.RejectionReasonTooLong);
    }
}
