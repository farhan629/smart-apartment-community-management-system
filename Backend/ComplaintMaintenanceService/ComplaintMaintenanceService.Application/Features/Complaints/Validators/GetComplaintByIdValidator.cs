using ComplaintMaintenanceService.Application.Common.Constants;
using ComplaintMaintenanceService.Application.Features.Complaints.Queries;
using FluentValidation;

namespace ComplaintMaintenanceService.Application.Features.Complaints.Validators;

public class GetComplaintByIdValidator : AbstractValidator<GetComplaintByIdQuery>
{
    public GetComplaintByIdValidator()
    {
        RuleFor(x => x.ComplaintId)
            .NotEmpty()
            .WithMessage(ComplaintConstants.Messages.ComplaintIdRequired);
    }
}
