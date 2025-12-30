using FluentValidation;
using MediatR;

namespace DivergentFlow.Application.Behaviors;

/// <summary>
/// MediatR pipeline behavior that validates incoming requests using FluentValidation before they reach their handlers.
/// </summary>
/// <remarks>
/// <para>
/// This behavior is registered once in the MediatR pipeline and executes for every
/// <typeparamref name="TRequest"/> that has one or more <see cref="IValidator{T}"/>
/// implementations registered in the dependency injection container.
/// </para>
/// <para>
/// For each incoming request, it creates a <see cref="ValidationContext{T}"/> and runs
/// all validators asynchronously. If any validation failures are detected, a
/// <see cref="ValidationException"/> containing all failures is thrown and the request
/// handler is not invoked. If validation succeeds, the request is passed to the next
/// behavior or handler in the pipeline.
/// </para>
/// </remarks>
/// <typeparam name="TRequest">The type of the MediatR request being validated.</typeparam>
/// <typeparam name="TResponse">The type of the response produced by the request handler.</typeparam>
public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="validators">
    /// The collection of <see cref="IValidator{T}"/> instances that will be used to validate
    /// the incoming <typeparamref name="TRequest"/>.
    /// </param>
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    /// <summary>
    /// Handles the validation of the request before passing it to the next behavior or handler.
    /// </summary>
    /// <param name="request">The request to validate.</param>
    /// <param name="next">The next delegate in the pipeline.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The response from the next behavior or handler in the pipeline.</returns>
    /// <exception cref="ValidationException">Thrown when validation fails with one or more errors.</exception>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);
        var results = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = results
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count != 0)
        {
            throw new ValidationException(failures);
        }

        return await next();
    }
}
