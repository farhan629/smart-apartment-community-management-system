using ComplaintMaintenanceService.Application.Features.Assignments.Queries;
using FluentValidation;
using Shared.SharedLibrary.Constants;

namespace ComplaintMaintenanceService.Application.Features.Assignments.Validators;

public class GetStaffAssignmentHistoryValidator : AbstractValidator<GetStaffAssignmentHistoryQuery>
{
    public GetStaffAssignmentHistoryValidator()
    {
        RuleFor(x => x.CurrentUserId).NotEmpty();

        RuleFor(x => x.Page).GreaterThanOrEqualTo(PaginationConstants.MinPageNumber);

        RuleFor(x => x.Limit)
            .GreaterThanOrEqualTo(PaginationConstants.MinPageSize)
            .LessThanOrEqualTo(PaginationConstants.MaxPageSize);
    }
}
