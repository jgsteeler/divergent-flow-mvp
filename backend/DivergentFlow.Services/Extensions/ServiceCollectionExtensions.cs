using DivergentFlow.Services.Repositories;
using DivergentFlow.Services.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DivergentFlow.Services.Extensions;

/// <summary>
/// Extension methods for registering services in the DI container
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all DivergentFlow services with the dependency injection container
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for method chaining</returns>
    public static IServiceCollection UseServices(this IServiceCollection services)
    {
        // Register capture repository
        // Using Redis implementation with Upstash
        services.AddSingleton<ICaptureRepository, RedisCaptureRepository>();

        // Register capture service
        services.AddScoped<ICaptureService, CaptureService>();

        // Register type inference service
        // Using basic implementation for MVP, will be replaced with ML-based inference later
        services.AddSingleton<ITypeService, BasicTypeService>();

        return services;
    }
}
