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
    /// - Development: Allow localhost/127.0.0.1 (any port) with credentials, plus any configured origins
    /// - Staging/Production: Allow only explicitly configured origins (fail closed if not configured)
    ///
    /// Environment variables:
    /// - CORS_ALLOWED_ORIGINS: comma-separated list of allowed origins (e.g. https://app.example.com,http://localhost:5173)
    /// </summary>
    public static IServiceCollection AddCorsPolicy(this IServiceCollection services, IWebHostEnvironment environment)
    {
        var allowedOrigins = EnvironmentHelper.ParseOrigins(Environment.GetEnvironmentVariable("CORS_ALLOWED_ORIGINS"));

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                if (environment.IsDevelopment())
                {
                    ConfigureDevelopmentCors(policy, allowedOrigins);
                    return;
                }

                ConfigureLockedDownCors(policy, allowedOrigins);
            });
        });

        return services;
    }

    private static void ConfigureDevelopmentCors(
        Microsoft.AspNetCore.Cors.Infrastructure.CorsPolicyBuilder policy,
        string[] allowedOrigins)
    {
        policy
            // Allow any localhost/127.0.0.1 origin so Vite can pick any port.
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

                if (uri.Host == "localhost" || uri.Host == "127.0.0.1")
                {
                    return true;
                }

                return allowedOrigins.Contains(origin, StringComparer.OrdinalIgnoreCase);
            })
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    }

    private static void ConfigureLockedDownCors(
        Microsoft.AspNetCore.Cors.Infrastructure.CorsPolicyBuilder policy,
        string[] allowedOrigins)
    {
        if (allowedOrigins.Length == 0)
        {
            // Fail closed if not configured.
            policy.SetIsOriginAllowed(static _ => false);
            return;
        }

        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .DisallowCredentials();
    }
}
