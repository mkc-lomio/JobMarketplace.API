using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Common.Behaviors
{
    /// <summary>
    /// MediatR pipeline behavior — runs all FluentValidation validators before the handler executes.
    /// If any rule fails → throws ValidationException → caught by middleware → 400 Bad Request.
    /// Handlers never receive invalid data. Add a new validator class and it's auto-discovered.
    /// Pipeline: Request → [ValidationBehavior] → LoggingBehavior → Handler
    /// </summary>
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators; // All validators for this request type (injected by DI)

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            if (!_validators.Any()) return await next(); // No validators? Skip to handler.

            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            var failures = validationResults
                .Where(r => r.Errors.Count > 0)
                .SelectMany(r => r.Errors)
                .ToList();

            if (failures.Count > 0)
                throw new Exceptions.ValidationException(failures);

            return await next();
        }
    }
}