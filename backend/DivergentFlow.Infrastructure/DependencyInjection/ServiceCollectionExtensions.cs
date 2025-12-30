using DivergentFlow.Application.Abstractions;
using DivergentFlow.Infrastructure.Repositories;
using DivergentFlow.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DivergentFlow.Infrastructure.DependencyInjection;

/// <summary>
/// Provides extension methods for registering infrastructure-layer services with the dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers infrastructure-layer services, including repository and type inference implementations.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to which infrastructure services are added.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance to allow for method chaining.</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<ICaptureRepository, InMemoryCaptureRepository>();
        services.AddSingleton<ITypeInferenceService, BasicTypeInferenceService>();
        return services;
    }
}
