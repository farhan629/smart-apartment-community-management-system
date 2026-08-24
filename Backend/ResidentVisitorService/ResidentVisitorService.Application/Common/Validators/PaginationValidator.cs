using FluentValidation;
using ResidentVisitorService.Application.Constants;
using Shared.SharedLibrary.Constants;

namespace ResidentVisitorService.Application.Common.Validators;

public abstract class PaginationValidator<T> : AbstractValidator<T>
{
    protected void AddPaginationRules(
        System.Linq.Expressions.Expression<Func<T, int>> pageSelector,
        System.Linq.Expressions.Expression<Func<T, int>> limitSelector
    )
    {
        RuleFor(pageSelector)
            .GreaterThan(0)
            .WithMessage(ResidentVisitorConstants.Validation.PageMustBePositive);

        RuleFor(limitSelector)
            .GreaterThan(0)
            .WithMessage(ResidentVisitorConstants.Validation.LimitMustBePositive)
            .LessThanOrEqualTo(PaginationConstants.MaxPageSize)
            .WithMessage(ResidentVisitorConstants.Validation.LimitExceedsMax);
    }
}
