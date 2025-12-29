using DivergentFlow.Api.Extensions;
using DivergentFlow.Api.Tests.TestDoubles;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace DivergentFlow.Api.Tests;

[CollectionDefinition("CORS", DisableParallelization = true)]
public class CorsCollectionDefinition
{
}

[Collection("CORS")]
public sealed class CorsPolicyTests
{
    private static IDisposable WithEnv(params (string Key, string? Value)[] entries)
    {
        var previous = new Dictionary<string, string?>();
        foreach (var (key, value) in entries)
        {
            previous[key] = Environment.GetEnvironmentVariable(key);
            Environment.SetEnvironmentVariable(key, value);
        }

        return new EnvironmentRestorer(previous);
    }

    private sealed class EnvironmentRestorer : IDisposable
    {
        private readonly Dictionary<string, string?> _originalValues;
        private bool _disposed;

        public EnvironmentRestorer(Dictionary<string, string?> originalValues)
        {
            _originalValues = originalValues;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            foreach (var (key, originalValue) in _originalValues)
            {
                Environment.SetEnvironmentVariable(key, originalValue);
            }

            _disposed = true;
        }
    }

    private static CorsPolicy GetDefaultCorsPolicy(string environmentName)
    {
        var services = new ServiceCollection();
        services.AddCorsPolicy(new FakeWebHostEnvironment { EnvironmentName = environmentName });
        var provider = services.BuildServiceProvider();

        var corsOptions = provider.GetRequiredService<IOptions<CorsOptions>>().Value;
        return corsOptions.GetPolicy(corsOptions.DefaultPolicyName)
            ?? throw new InvalidOperationException("Default CORS policy was not configured");
    }

    [Fact]
    public void Staging_Allows_Configured_Origin()
    {
        using var _ = WithEnv(("CORS_ALLOWED_ORIGINS", "https://staging.example.com,https://test.example.com"));

        var policy = GetDefaultCorsPolicy("Staging");

        Assert.True(policy.IsOriginAllowed("https://staging.example.com"));
        Assert.False(policy.SupportsCredentials);
    }

    [Fact]
    public void Staging_Blocks_Unauthorized_Origin()
    {
        using var _ = WithEnv(("CORS_ALLOWED_ORIGINS", "https://staging.example.com"));

        var policy = GetDefaultCorsPolicy("Staging");

        Assert.False(policy.IsOriginAllowed("https://evil.com"));
    }

    [Fact]
    public void Production_Fails_Closed_When_Not_Configured()
    {
        using var _ = WithEnv(("CORS_ALLOWED_ORIGINS", ""));

        var policy = GetDefaultCorsPolicy("Production");

        Assert.False(policy.IsOriginAllowed("https://any-origin.com"));
        Assert.False(policy.SupportsCredentials);
    }

    [Fact]
    public void Production_Allows_Multiple_Origins_CommaSeparated()
    {
        using var _ = WithEnv(("CORS_ALLOWED_ORIGINS", "https://app.example.com,https://www.example.com"));

        var policy = GetDefaultCorsPolicy("Production");

        Assert.True(policy.IsOriginAllowed("https://app.example.com"));
        Assert.True(policy.IsOriginAllowed("https://www.example.com"));
        Assert.False(policy.SupportsCredentials);
    }

    [Fact]
    public void Production_Normalizes_Origins_TrailingSlash_And_DefaultPorts()
    {
        using var _ = WithEnv(("CORS_ALLOWED_ORIGINS", "https://app.example.com/,https://example.com:443,http://test.example.com:80"));

        var policy = GetDefaultCorsPolicy("Production");

        Assert.True(policy.IsOriginAllowed("https://app.example.com"));
        Assert.True(policy.IsOriginAllowed("https://example.com"));
        Assert.True(policy.IsOriginAllowed("http://test.example.com"));
    }

    [Fact]
    public void Development_Allows_Localhost_And_127_0_0_1_And_Sets_Credentials()
    {
        var policy = GetDefaultCorsPolicy("Development");

        Assert.True(policy.IsOriginAllowed("http://localhost:5173"));
        Assert.True(policy.IsOriginAllowed("https://localhost:5173"));
        Assert.True(policy.IsOriginAllowed("http://127.0.0.1:3000"));
        Assert.True(policy.SupportsCredentials);
    }

    [Fact]
    public void Development_Allows_Configured_Origins_Too()
    {
        using var _ = WithEnv(("CORS_ALLOWED_ORIGINS", "https://app.example.com"));

        var policy = GetDefaultCorsPolicy("Development");

        Assert.True(policy.IsOriginAllowed("https://app.example.com"));
        Assert.True(policy.SupportsCredentials);
    }

    [Fact]
    public void Development_Blocks_NonHttpSchemes_And_Invalid_Origins()
    {
        var policy = GetDefaultCorsPolicy("Development");

        Assert.False(policy.IsOriginAllowed("file://localhost"));
        Assert.False(policy.IsOriginAllowed("not-a-url"));
    }
}
