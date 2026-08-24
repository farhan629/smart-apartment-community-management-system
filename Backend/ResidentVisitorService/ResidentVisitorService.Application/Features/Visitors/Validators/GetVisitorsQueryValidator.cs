using ResidentVisitorService.Application.Common.Validators;
using ResidentVisitorService.Application.Features.Visitors.Queries;

namespace ResidentVisitorService.Application.Features.Visitors.Validators;

public class GetVisitorsQueryValidator : PaginationValidator<GetVisitorsQuery>
{
    public GetVisitorsQueryValidator()
    {
        AddPaginationRules(x => x.Page, x => x.Limit);
    }
}
