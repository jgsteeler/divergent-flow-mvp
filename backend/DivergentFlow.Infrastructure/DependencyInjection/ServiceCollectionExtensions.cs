using DivergentFlow.Application.Abstractions;
using DivergentFlow.Application.Configuration;
using DivergentFlow.Infrastructure.Repositories;
using DivergentFlow.Infrastructure.Services;
using DivergentFlow.Infrastructure.Services.Upstash;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Net.Http.Headers;

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
        // Register MongoDB
        services.AddSingleton<IMongoClient>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
            return new MongoClient(settings.ConnectionString);
        });

        services.AddSingleton<IMongoDatabase>(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
            return client.GetDatabase(settings.DatabaseName);
        });

        // Register MongoDB repositories
        services.AddScoped<IItemRepository, MongoItemRepository>();
        services.AddScoped<ICollectionRepository, MongoCollectionRepository>();

        // If this is an Upstash endpoint, prefer REST API.
        // - Avoids TCP/TLS/SNI pitfalls in some hosting environments
        // - Enables use of Upstash REST tokens (standard + readonly)
        services.AddSingleton<UpstashRedisRestOptionsProvider>();

        services.AddHttpClient("UpstashRedisWrite", (sp, client) =>
        {
            var options = sp.GetRequiredService<UpstashRedisRestOptionsProvider>().Options;
            if (options is null)
            {
                return;
            }

            client.BaseAddress = options.BaseUri;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", options.WriteToken);
        });

        services.AddHttpClient("UpstashRedisRead", (sp, client) =>
        {
            var options = sp.GetRequiredService<UpstashRedisRestOptionsProvider>().Options;
            if (options is null)
            {
                return;
            }

            client.BaseAddress = options.BaseUri;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                options.ReadToken ?? options.WriteToken);
        });

        services.AddScoped<IUpstashRedisRestReadClient>(sp =>
        {
            var factory = sp.GetRequiredService<IHttpClientFactory>();
            return new UpstashRedisRestClient(factory.CreateClient("UpstashRedisRead"));
        });

        services.AddScoped<IUpstashRedisRestWriteClient>(sp =>
        {
            var factory = sp.GetRequiredService<IHttpClientFactory>();
            return new UpstashRedisRestClient(factory.CreateClient("UpstashRedisWrite"));
        });

        // Captures API is preserved, but persistence is unified:
        // a "capture" is stored as an Item document with Type == "capture".
        services.AddScoped<ICaptureRepository, MongoCaptureRepository>();

        // Projection sync is REST-only (Upstash Redis REST) or no-op if not configured.
        services.AddScoped<IProjectionWriter>(sp =>
        {
            var options = sp.GetRequiredService<UpstashRedisRestOptionsProvider>().Options;
            return options is null
                ? ActivatorUtilities.CreateInstance<NoOpProjectionWriter>(sp)
                : ActivatorUtilities.CreateInstance<RedisProjectionWriter>(sp);
        });

        services.AddSingleton<ITypeInferenceService, BasicTypeInferenceService>();
        
        // Register feature flags service
        services.AddSingleton<IFeatureFlags, FeatureFlagsService>();
        
        return services;
    }

    private sealed class NoOpProjectionWriter : IProjectionWriter
    {
        private readonly ILogger<NoOpProjectionWriter> _logger;

        public NoOpProjectionWriter(ILogger<NoOpProjectionWriter> logger)
        {
            _logger = logger;
            _logger.LogWarning("NoOpProjectionWriter is being used - projection writes will be skipped");
        }

        public Task SyncItemAsync(Domain.Entities.Item item, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Skipping projection write for item {ItemId}", item.Id);
            return Task.CompletedTask;
        }

        public Task SyncCollectionAsync(Domain.Entities.Collection collection, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Skipping projection write for collection {CollectionId}", collection.Id);
            return Task.CompletedTask;
        }
    }
}
