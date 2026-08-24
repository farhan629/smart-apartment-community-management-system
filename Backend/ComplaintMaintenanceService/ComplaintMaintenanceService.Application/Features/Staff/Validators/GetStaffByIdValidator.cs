using ComplaintMaintenanceService.Application.Common.Constants;
using ComplaintMaintenanceService.Application.Features.Staff.Queries;
using FluentValidation;

namespace ComplaintMaintenanceService.Application.Features.Staff.Validators;

public class GetStaffByIdValidator : AbstractValidator<GetStaffByIdQuery>
{
    public GetStaffByIdValidator()
    {
        RuleFor(x => x.StaffId).NotEmpty().WithMessage(StaffConstants.Validation.StaffIdRequired);
    }
}
