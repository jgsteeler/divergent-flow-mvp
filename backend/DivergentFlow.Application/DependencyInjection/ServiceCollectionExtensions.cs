using System.Reflection;
using DivergentFlow.Application.Abstractions;
using DivergentFlow.Application.Behaviors;
using DivergentFlow.Application.Configuration;
using DivergentFlow.Application.Services;
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

        // Register type inference workflow services
        services.AddSingleton<ITypeInferenceWorkflowTrigger, SimpleTypeInferenceWorkflowTrigger>();
        services.AddHostedService<BackgroundTypeInferenceService>();

        // Register inference queue and processor
        services.AddSingleton<IInferenceQueue, InProcessInferenceQueue>();
        services.AddHostedService<InferenceQueueProcessorService>();

        // Register configuration options
        services.AddOptions<InferenceOptions>()
            .BindConfiguration(InferenceOptions.SectionName)
            .ValidateOnStart();

        services.AddOptions<Configuration.MongoDbSettings>()
            .BindConfiguration(Configuration.MongoDbSettings.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<FeatureFlagOptions>()
            .BindConfiguration(FeatureFlagOptions.SectionName)
            .ValidateOnStart();

        return services;
    }
}
