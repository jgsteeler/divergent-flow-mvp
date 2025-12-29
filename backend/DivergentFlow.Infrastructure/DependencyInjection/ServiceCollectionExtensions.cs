using DivergentFlow.Application.Abstractions;
using DivergentFlow.Infrastructure.Repositories;
using DivergentFlow.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DivergentFlow.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<ICaptureRepository, InMemoryCaptureRepository>();
        services.AddSingleton<ITypeInferenceService, BasicTypeInferenceService>();
        return services;
    }
}
