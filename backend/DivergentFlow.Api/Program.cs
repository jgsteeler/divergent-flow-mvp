using DivergentFlow.Services.Extensions;

static void LoadDotEnvUpwards(string startDirectory, int maxDepth = 4)
{
    var current = new DirectoryInfo(startDirectory);

    for (var depth = 0; depth < maxDepth && current != null; depth++)
    {
        var candidate = Path.Combine(current.FullName, ".env");
        if (File.Exists(candidate))
        {
            foreach (var rawLine in File.ReadAllLines(candidate))
            {
                var line = rawLine.Trim();
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#'))
                {
                    continue;
                }

                if (line.StartsWith("export ", StringComparison.OrdinalIgnoreCase))
                {
                    line = line[7..].TrimStart();
                }

                var equalsIndex = line.IndexOf('=');
                if (equalsIndex <= 0)
                {
                    continue;
                }

                var key = line[..equalsIndex].Trim();
                var value = line[(equalsIndex + 1)..].Trim();

                if (value.Length >= 2 &&
                    ((value.StartsWith('"') && value.EndsWith('"')) || (value.StartsWith('\'') && value.EndsWith('\''))))
                {
                    value = value[1..^1];
                }

                if (string.IsNullOrWhiteSpace(key))
                {
                    continue;
                }

                // Don't override real environment variables (e.g., Fly, CI)
                if (Environment.GetEnvironmentVariable(key) == null)
                {
                    Environment.SetEnvironmentVariable(key, value);
                }
            }

            return;
        }

        current = current.Parent;
    }
}

static bool GetBoolEnv(string key, bool defaultValue)
{
    var value = Environment.GetEnvironmentVariable(key);
    return bool.TryParse(value, out var parsed) ? parsed : defaultValue;
}

static string[] ParseOrigins(string? raw)
{
    if (string.IsNullOrWhiteSpace(raw))
    {
        return Array.Empty<string>();
    }

    return raw
        .Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        .Where(o => !string.IsNullOrWhiteSpace(o))
        .ToArray();
}

var builder = WebApplication.CreateBuilder(args);

// Local dev convenience: load env vars from a .env file (if present).
// Notes:
// - This is intentionally dependency-free.
// - It only sets variables that aren't already set in the process environment.
LoadDotEnvUpwards(builder.Environment.ContentRootPath);

// Add services to the container
builder.Services.AddControllers();

// CORS (env-var driven)
// - Development: allow localhost/127.0.0.1 (any port)
// - Staging: allow Netlify site + deploy previews/branch deploys (optional) + any explicitly configured origins
// - Production: allow only explicitly configured origins
//
// Env vars:
// - CORS_PRODUCTION_ORIGINS: semicolon or comma-separated list of allowed origins
// - CORS_STAGING_ORIGINS: semicolon or comma-separated list of allowed origins
// - CORS_NETLIFY_SITE_NAME: e.g. div-flo-mvp
// - CORS_ALLOW_NETLIFY_PREVIEWS: true/false (enables *.netlify.app dynamic preview/branch origins in Staging)
var netlifySiteName = Environment.GetEnvironmentVariable("CORS_NETLIFY_SITE_NAME") ?? "div-flo-mvp";
var allowNetlifyDynamicOrigins = GetBoolEnv("CORS_ALLOW_NETLIFY_PREVIEWS", false);
var stagingOrigins = ParseOrigins(Environment.GetEnvironmentVariable("CORS_STAGING_ORIGINS"));
var productionOrigins = ParseOrigins(Environment.GetEnvironmentVariable("CORS_PRODUCTION_ORIGINS"));

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        if (builder.Environment.IsDevelopment())
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

            return;
        }

        if (builder.Environment.IsStaging())
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

            return;
        }

        // Production: only explicit origins
        if (productionOrigins.Length == 0)
        {
            // Fail closed if not configured.
            policy.SetIsOriginAllowed(static _ => false);
            return;
        }

        policy
            .WithOrigins(productionOrigins)
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// Register services for dependency injection using extension method
builder.Services.UseServices();

// Add Swagger/OpenAPI documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "Divergent Flow API",
        Version = "v1",
        Description = "ADHD-friendly brain management tool API"
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Divergent Flow API v1");
    });
}

// Enable CORS
app.UseCors();

app.UseAuthorization();

app.MapGet("/health", () => Results.Ok(new { status = "Healthy" }));

app.MapControllers();

app.Run();

// Make Program class accessible to tests
public partial class Program { }
