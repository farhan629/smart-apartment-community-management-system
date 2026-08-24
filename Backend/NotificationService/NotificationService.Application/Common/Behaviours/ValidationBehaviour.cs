using FluentValidation;
using MediatR;
using Shared.SharedLibrary.Exceptions;

namespace NotificationService.Application.Common.Behaviours;

/// <summary>
/// MediatR pipeline behaviour that runs FluentValidation validators before the request handler.
/// </summary>
public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    /// <summary>
    /// Validates the request and delegates to the next handler if validation passes.
    /// </summary>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);
        var failures = _validators
            .Select(v => v.Validate(context))
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count != 0)
            throw new BadRequestException(string.Join("; ", failures.Select(f => f.ErrorMessage)));

        return await next();
    }
}
