using DivergentFlow.Api.Utilities;

namespace DivergentFlow.Api.Extensions;

/// <summary>
/// Extension methods for configuring CORS policies.
/// </summary>
public static class CorsConfigurationExtensions
{
    /// <summary>
    /// Adds environment-driven CORS policy to the service collection.
    /// 
    /// CORS behavior by environment:
    /// - Development: Allow localhost/127.0.0.1 (any port) with credentials
    /// - Staging: Allow Netlify site + deploy previews/branch deploys (optional) + explicitly configured origins
    /// - Production: Allow only explicitly configured origins
    /// 
    /// Environment variables:
    /// - CORS_PRODUCTION_ORIGINS: semicolon or comma-separated list of allowed origins
    /// - CORS_STAGING_ORIGINS: semicolon or comma-separated list of allowed origins
    /// - CORS_NETLIFY_SITE_NAME: e.g. div-flo-mvp
    /// - CORS_ALLOW_NETLIFY_PREVIEWS: true/false (enables *.netlify.app dynamic preview/branch origins in Staging)
    /// </summary>
    public static IServiceCollection AddCorsPolicy(this IServiceCollection services, IWebHostEnvironment environment)
    {
        var netlifySiteName = Environment.GetEnvironmentVariable("CORS_NETLIFY_SITE_NAME") ?? "div-flo-mvp";
        var allowNetlifyDynamicOrigins = EnvironmentHelper.GetBoolEnv("CORS_ALLOW_NETLIFY_PREVIEWS", false);
        var stagingOrigins = EnvironmentHelper.ParseOrigins(Environment.GetEnvironmentVariable("CORS_STAGING_ORIGINS"));
        var productionOrigins = EnvironmentHelper.ParseOrigins(Environment.GetEnvironmentVariable("CORS_PRODUCTION_ORIGINS"));

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                if (environment.IsDevelopment())
                {
                    ConfigureDevelopmentCors(policy);
                    return;
                }

                if (environment.IsStaging())
                {
                    ConfigureStagingCors(policy, netlifySiteName, allowNetlifyDynamicOrigins, stagingOrigins);
                    return;
                }

                // Production: only explicit origins
                ConfigureProductionCors(policy, productionOrigins);
            });
        });

        return services;
    }

    private static void ConfigureDevelopmentCors(Microsoft.AspNetCore.Cors.Infrastructure.CorsPolicyBuilder policy)
    {
        policy
            // Allow any localhost/127.0.0.1 origin so Vite can pick any port.
            .SetIsOriginAllowed(static origin =>
            {
                if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri))
                {
                    return false;
                }

                return uri.Scheme is "http" or "https" &&
                       (uri.Host == "localhost" || uri.Host == "127.0.0.1");
            })
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    }

    private static void ConfigureStagingCors(
        Microsoft.AspNetCore.Cors.Infrastructure.CorsPolicyBuilder policy,
        string netlifySiteName,
        bool allowNetlifyDynamicOrigins,
        string[] stagingOrigins)
    {
        policy
            .SetIsOriginAllowed(origin =>
            {
                if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri))
                {
                    return false;
                }

                if (uri.Scheme is not ("http" or "https"))
                {
                    return false;
                }

                // Explicitly allowed origins (exact match)
                if (stagingOrigins.Contains(origin, StringComparer.OrdinalIgnoreCase))
                {
                    return true;
                }

                // Netlify staging + deploy previews/branch deploys
                var host = uri.Host;
                var mainNetlifyHost = $"{netlifySiteName}.netlify.app";
                if (string.Equals(host, mainNetlifyHost, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                // Covers both:
                // - Deploy previews: deploy-preview-123--{site}.netlify.app
                // - Branch deploys:   feature-some-branch--{site}.netlify.app
                if (allowNetlifyDynamicOrigins && host.EndsWith($"--{netlifySiteName}.netlify.app", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                return false;
            })
            .AllowAnyMethod()
            .AllowAnyHeader();
    }

    private static void ConfigureProductionCors(
        Microsoft.AspNetCore.Cors.Infrastructure.CorsPolicyBuilder policy,
        string[] productionOrigins)
    {
        if (productionOrigins.Length == 0)
        {
            // Fail closed if not configured.
            policy.SetIsOriginAllowed(static _ => false);
            return;
        }

        policy
            .WithOrigins(productionOrigins)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .DisallowCredentials();
    }
}
