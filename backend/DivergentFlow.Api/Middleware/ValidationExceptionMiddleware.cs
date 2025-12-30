using FluentValidation;

namespace DivergentFlow.Api.Middleware;

/// <summary>
/// Middleware that intercepts <see cref="ValidationException"/> instances and converts them
/// into standardized HTTP 400 (Bad Request) responses with a structured validation error payload.
/// </summary>
/// <remarks>
/// Groups validation failures by property name and returns a JSON response of the form:
/// <c>{ "title": "Validation failed", "status": 400, "errors": { "Property": ["Message1", "Message2"] } }</c>.
/// This ensures API consumers receive consistent and machine-readable validation error information.
/// </remarks>
public sealed class ValidationExceptionMiddleware
{
    private readonly RequestDelegate _next;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationExceptionMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware delegate in the ASP.NET Core request pipeline.</param>
    public ValidationExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// Processes the current HTTP request and converts any thrown <see cref="ValidationException"/>
    /// into an HTTP 400 response containing grouped validation errors in JSON format.
    /// </summary>
    /// <param name="context">The current <see cref="HttpContext"/> for the request.</param>
    /// <returns>A task that represents the asynchronous middleware operation.</returns>
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            if (context.Response.HasStarted)
            {
                throw;
            }

            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";

            var errors = ex.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).Distinct().ToArray()
                );

            await context.Response.WriteAsJsonAsync(new
            {
                title = "Validation failed",
                status = StatusCodes.Status400BadRequest,
                errors
            });
        }
    }
}
