using ComplaintMaintenanceService.Application.Common.Constants;
using ComplaintMaintenanceService.Application.Features.Complaints.Commands;
using FluentValidation;

namespace ComplaintMaintenanceService.Application.Features.Complaints.Validators;

public class CreateComplaintValidator : AbstractValidator<CreateComplaintCommand>
{
    public CreateComplaintValidator()
    {
        RuleFor(x => x.Request.ComplaintTypeRefId)
            .NotEmpty()
            .WithMessage(ComplaintConstants.Messages.ComplaintTypeRequired);

        RuleFor(x => x.Request.CategoryId)
            .NotEmpty()
            .WithMessage(ComplaintConstants.Messages.CategoryIdRequired);

        RuleFor(x => x.Request.PriorityRefId)
            .NotEmpty()
            .WithMessage(ComplaintConstants.Messages.PriorityRequired);

        RuleFor(x => x.Request.Description)
            .NotEmpty()
            .WithMessage(ComplaintConstants.Messages.DescriptionRequired)
            .MaximumLength(ComplaintConstants.ValidationLimits.DescriptionMaxLength)
            .WithMessage(ComplaintConstants.Messages.DescriptionMaxLength);

        RuleFor(x => x.Request.PreferredDate)
            .NotEmpty()
            .WithMessage(ComplaintConstants.Messages.PreferredDateRequired)
            .Must(d => DateOnly.TryParse(d, out _))
            .WithMessage(ComplaintConstants.Messages.PreferredDateInvalidFormat);

        RuleFor(x => x.Request.PreferredTime)
            .Must(t => t == null || TimeOnly.TryParse(t, out _))
            .WithMessage(ComplaintConstants.Messages.PreferredTimeInvalidFormat);
    }
}
