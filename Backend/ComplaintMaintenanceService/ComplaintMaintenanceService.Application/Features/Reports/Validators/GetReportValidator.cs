using ComplaintMaintenanceService.Application.Common.Constants;
using ComplaintMaintenanceService.Application.Features.Reports.Queries;
using FluentValidation;

namespace ComplaintMaintenanceService.Application.Features.Reports.Validators;

public class GetReportValidator : AbstractValidator<GetReportQuery>
{
    public GetReportValidator()
    {
        RuleFor(x => x.ToDate)
            .GreaterThanOrEqualTo(x => x.FromDate)
            .WithMessage(ComplaintConstants.ReportMessages.InvalidDateRange)
            .When(x => x.FromDate.HasValue && x.ToDate.HasValue);

        RuleFor(x => x.FromDate)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage(ComplaintConstants.ReportMessages.FromDateCannotBeFuture)
            .When(x => x.FromDate.HasValue);

        RuleFor(x => x.ToDate)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage(ComplaintConstants.ReportMessages.ToDateCannotBeFuture)
            .When(x => x.ToDate.HasValue);
    }
}
