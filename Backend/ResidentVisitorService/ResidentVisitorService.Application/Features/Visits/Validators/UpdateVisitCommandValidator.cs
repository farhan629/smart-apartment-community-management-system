using FluentValidation;
using ResidentVisitorService.Application.Constants;
using ResidentVisitorService.Application.Features.Visits.Commands;

namespace ResidentVisitorService.Application.Features.Visits.Validators;

public class UpdateVisitCommandValidator : AbstractValidator<UpdateVisitCommand>
{
    public UpdateVisitCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(ResidentVisitorConstants.Validation.VisitIdRequired);

        RuleFor(x => x.Request)
            .Must(r => r.PurposeTypeId.HasValue || r.StartDate.HasValue || r.EndDate.HasValue)
            .WithMessage(ResidentVisitorConstants.Validation.AtLeastOneFieldRequired);

        RuleFor(x => x.Request)
            .Must(r => r.EndDate >= r.StartDate)
            .WithMessage(ResidentVisitorConstants.Validation.EndDateBeforeStartDate)
            .When(x => x.Request.StartDate.HasValue && x.Request.EndDate.HasValue);
    }
}
