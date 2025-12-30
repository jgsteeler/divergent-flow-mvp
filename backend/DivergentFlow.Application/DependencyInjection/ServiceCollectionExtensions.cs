using System.Reflection;
using DivergentFlow.Application.Behaviors;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace DivergentFlow.Application.DependencyInjection;

/// <summary>
/// Provides extension methods for registering application-layer services with the dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers application-layer services, including MediatR handlers, AutoMapper profiles,
    /// FluentValidation validators, and the validation pipeline behavior, using the current assembly.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to which application services are added.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance to allow for method chaining.</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddMediatR(assembly);
        services.AddAutoMapper(assembly);
        services.AddValidatorsFromAssembly(assembly);

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}
