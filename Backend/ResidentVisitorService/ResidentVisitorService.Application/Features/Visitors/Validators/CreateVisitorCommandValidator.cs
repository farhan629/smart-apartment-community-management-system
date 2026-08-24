using FluentValidation;
using ResidentVisitorService.Application.Features.Visitors.Commands;

namespace ResidentVisitorService.Application.Features.Visitors.Validators;

public class CreateVisitorCommandValidator : AbstractValidator<CreateVisitorCommand>
{
    public CreateVisitorCommandValidator()
    {
        RuleFor(x => x.Request).SetValidator(new CreateVisitorRequestDtoValidator());
    }
}
