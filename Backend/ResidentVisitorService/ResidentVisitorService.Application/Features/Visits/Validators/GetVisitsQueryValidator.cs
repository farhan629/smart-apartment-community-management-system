using ResidentVisitorService.Application.Common.Validators;
using ResidentVisitorService.Application.Features.Visits.Queries;

namespace ResidentVisitorService.Application.Features.Visits.Validators;

public class GetVisitsQueryValidator : PaginationValidator<GetVisitsQuery>
{
    public GetVisitsQueryValidator()
    {
        AddPaginationRules(x => x.Page, x => x.Limit);
    }
}
