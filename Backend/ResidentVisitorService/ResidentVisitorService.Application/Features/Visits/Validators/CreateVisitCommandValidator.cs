using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using ResidentVisitorService.Application.Constants;
using ResidentVisitorService.Application.Features.Visitors.Validators;
using ResidentVisitorService.Application.Features.Visits.Commands;

namespace ResidentVisitorService.Application.Features.Visits.Validators;

public class CreateVisitCommandValidator : AbstractValidator<CreateVisitCommand>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreateVisitCommandValidator(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;

        RuleFor(x => x.Request.PurposeTypeId)
            .NotEmpty()
            .WithMessage(ResidentVisitorConstants.Validation.PurposeTypeIdRequired);

        RuleFor(x => x.Request.StartDate)
            .NotEmpty()
            .WithMessage(ResidentVisitorConstants.Validation.StartDateRequired);

        RuleFor(x => x.Request.EndDate)
            .NotEmpty()
            .WithMessage(ResidentVisitorConstants.Validation.EndDateRequired);

        RuleFor(x => x.Request)
            .Must(r => r.VisitorId.HasValue || r.Visitor is not null)
            .WithMessage(ResidentVisitorConstants.Errors.VisitorOrDetailRequired);

        RuleFor(x => x.Request)
            .Must(r => r.EndDate >= r.StartDate)
            .WithMessage(ResidentVisitorConstants.Validation.EndDateBeforeStartDate)
            .When(x => x.Request.StartDate != default && x.Request.EndDate != default);

        RuleFor(x => x.Request)
            .Must(r =>
                !string.IsNullOrWhiteSpace(r.BlockNumber)
                && !string.IsNullOrWhiteSpace(r.FlatNumber)
            )
            .WithMessage(ResidentVisitorConstants.Errors.BlockAndFlatRequired)
            .When(IsSecurityCaller);

        RuleFor(x => x.Request.Visitor!)
            .SetValidator(new CreateVisitorRequestDtoValidator())
            .When(x => x.Request.Visitor is not null);
    }

    private bool IsSecurityCaller(CreateVisitCommand command)
    {
        var role = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value;
        return role == ResidentVisitorConstants.Roles.Security;
    }
}
