using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace DivergentFlow.Api.Tests;

[CollectionDefinition("CORS", DisableParallelization = true)]
public class CorsCollectionDefinition
{
}

public class StagingWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.UseEnvironment("Staging");
    }
}

public class ProductionWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.UseEnvironment("Production");
    }
}

public class DevelopmentWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.UseEnvironment("Development");
    }
}

[Collection("CORS")]
public class CorsPolicyTests
{
    /// <summary>
    /// Sets environment variables temporarily and returns a disposable to restore them.
    /// Using statement ensures cleanup happens even if test throws an exception.
    /// The collection-level parallelization is disabled to prevent race conditions.
    /// </summary>
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

    /// <summary>
    /// Disposable helper that restores environment variables to their original state.
    /// Guaranteed to run even if test throws an exception (via using statement).
    /// </summary>
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

    [Fact]
    public async Task Staging_Allows_Configured_Origin()
    {
        using var _ = WithEnv(
            ("CORS_ALLOWED_ORIGINS", "https://staging.example.com,https://test.example.com")
        );

        // Arrange
        await using var factory = new StagingWebApplicationFactory();
        var client = factory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, "/health");
        request.Headers.Add("Origin", "https://staging.example.com");

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.True(
            response.Headers.TryGetValues("Access-Control-Allow-Origin", out var values) &&
            values.Contains("https://staging.example.com"),
            "Expected Access-Control-Allow-Origin to allow configured origin in Staging"
        );
    }

    [Fact]
    public async Task Staging_BlocksUnauthorizedOrigins()
    {
        using var _ = WithEnv(
            ("CORS_ALLOWED_ORIGINS", "https://staging.example.com")
        );

        // Arrange
        await using var factory = new StagingWebApplicationFactory();
        var client = factory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, "/health");
        request.Headers.Add("Origin", "https://evil.com");

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.False(
            response.Headers.Contains("Access-Control-Allow-Origin"),
            "Expected no Access-Control-Allow-Origin for unauthorized origin in Staging"
        );
    }

    [Fact]
    public async Task Production_Allows_Configured_Origin()
    {
        using var _ = WithEnv(
            ("CORS_ALLOWED_ORIGINS", "https://app.getdivergentflow.com")
        );

        // Arrange
        await using var factory = new ProductionWebApplicationFactory();
        var client = factory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, "/health");
        request.Headers.Add("Origin", "https://app.getdivergentflow.com");

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.True(
            response.Headers.TryGetValues("Access-Control-Allow-Origin", out var values) &&
            values.Contains("https://app.getdivergentflow.com"),
            "Expected Access-Control-Allow-Origin to allow configured origin in Production"
        );
    }

    [Fact]
    public async Task Production_BlocksAllOrigins_WhenNoCorsAllowedOriginsSet()
    {
        using var _ = WithEnv(
            ("CORS_ALLOWED_ORIGINS", "")
        );

        // Arrange
        await using var factory = new ProductionWebApplicationFactory();
        var client = factory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, "/health");
        request.Headers.Add("Origin", "https://any-origin.com");

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.False(
            response.Headers.Contains("Access-Control-Allow-Origin"),
            "Expected no Access-Control-Allow-Origin when CORS_ALLOWED_ORIGINS is not set (fail closed)"
        );
    }

    [Fact]
    public async Task Production_AllowsMultipleOrigins_CommaSeparated()
    {
        using var _ = WithEnv(
            ("CORS_ALLOWED_ORIGINS", "https://app.example.com,https://www.example.com")
        );

        // Arrange
        await using var factory = new ProductionWebApplicationFactory();
        var client = factory.CreateClient();

        // Test first origin
        using var request1 = new HttpRequestMessage(HttpMethod.Get, "/health");
        request1.Headers.Add("Origin", "https://app.example.com");
        var response1 = await client.SendAsync(request1);

        // Test second origin
        using var request2 = new HttpRequestMessage(HttpMethod.Get, "/health");
        request2.Headers.Add("Origin", "https://www.example.com");
        var response2 = await client.SendAsync(request2);

        // Assert
        Assert.True(
            response1.Headers.TryGetValues("Access-Control-Allow-Origin", out var values1) &&
            values1.Contains("https://app.example.com"),
            "Expected first origin to be allowed"
        );
        Assert.True(
            response2.Headers.TryGetValues("Access-Control-Allow-Origin", out var values2) &&
            values2.Contains("https://www.example.com"),
            "Expected second origin to be allowed"
        );
    }

    [Fact]
    public async Task Development_Allows_Localhost_Origin()
    {
        // Arrange
        await using var factory = new DevelopmentWebApplicationFactory();
        var client = factory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, "/health");
        request.Headers.Add("Origin", "http://localhost:5173");

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.True(
            response.Headers.TryGetValues("Access-Control-Allow-Origin", out var values) &&
            values.Contains("http://localhost:5173"),
            "Expected Access-Control-Allow-Origin to allow localhost origin in Development"
        );
    }

    [Fact]
    public async Task Development_Allows_LocalhostHttps_Origin()
    {
        // Arrange
        await using var factory = new DevelopmentWebApplicationFactory();
        var client = factory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, "/health");
        request.Headers.Add("Origin", "https://localhost:5173");

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.True(
            response.Headers.TryGetValues("Access-Control-Allow-Origin", out var values) &&
            values.Contains("https://localhost:5173"),
            "Expected Access-Control-Allow-Origin to allow https localhost origin in Development"
        );
    }

    [Fact]
    public async Task Development_Allows_127_0_0_1_Origin()
    {
        // Arrange
        await using var factory = new DevelopmentWebApplicationFactory();
        var client = factory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, "/health");
        request.Headers.Add("Origin", "http://127.0.0.1:3000");

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.True(
            response.Headers.TryGetValues("Access-Control-Allow-Origin", out var values) &&
            values.Contains("http://127.0.0.1:3000"),
            "Expected Access-Control-Allow-Origin to allow 127.0.0.1 origin in Development"
        );
    }

    [Fact]
    public async Task Development_Sets_AllowCredentials()
    {
        // Arrange
        await using var factory = new DevelopmentWebApplicationFactory();
        var client = factory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, "/health");
        request.Headers.Add("Origin", "http://localhost:5173");

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.True(
            response.Headers.TryGetValues("Access-Control-Allow-Credentials", out var values) &&
            values.Contains("true"),
            "Expected Access-Control-Allow-Credentials to be true in Development"
        );
    }

    [Fact]
    public async Task Staging_HandlesOriginsWithWhitespace()
    {
        using var _ = WithEnv(
            ("CORS_ALLOWED_ORIGINS", "  https://staging.example.com  ,  https://test.example.com  ")
        );

        // Arrange
        await using var factory = new StagingWebApplicationFactory();
        var client = factory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, "/health");
        request.Headers.Add("Origin", "https://staging.example.com");

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.True(
            response.Headers.TryGetValues("Access-Control-Allow-Origin", out var values) &&
            values.Contains("https://staging.example.com"),
            "Expected origins with whitespace to be trimmed and allowed"
        );
    }

    [Fact]
    public async Task Staging_NormalizesOrigins_WithTrailingSlash()
    {
        using var _ = WithEnv(
            ("CORS_ALLOWED_ORIGINS", "https://app.example.com/")
        );

        // Arrange
        await using var factory = new StagingWebApplicationFactory();
        var client = factory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, "/health");
        request.Headers.Add("Origin", "https://app.example.com");

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.True(
            response.Headers.TryGetValues("Access-Control-Allow-Origin", out var values) &&
            values.Contains("https://app.example.com"),
            "Expected configured origin with trailing slash to be normalized and allowed"
        );
    }

    [Fact]
    public async Task Production_DoesNotAllow_SemicolonSeparatedList()
    {
        using var _ = WithEnv(
            ("CORS_ALLOWED_ORIGINS", "https://app.example.com;https://www.example.com")
        );

        // Arrange
        await using var factory = new ProductionWebApplicationFactory();
        var client = factory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, "/health");
        request.Headers.Add("Origin", "https://app.example.com");

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.False(
            response.Headers.Contains("Access-Control-Allow-Origin"),
            "Expected semicolon-separated list to be rejected (CORS_ALLOWED_ORIGINS must be comma-separated)"
        );
    }

    [Fact]
    public async Task Staging_BlocksInvalidOrigins_MalformedUrl()
    {
        using var _ = WithEnv(
            ("CORS_ALLOWED_ORIGINS", "https://staging.example.com")
        );

        // Arrange
        await using var factory = new StagingWebApplicationFactory();
        var client = factory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, "/health");
        request.Headers.Add("Origin", "not-a-valid-url");

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.False(
            response.Headers.Contains("Access-Control-Allow-Origin"),
            "Expected malformed URL to be rejected"
        );
    }

    [Fact]
    public async Task Staging_BlocksNonHttpSchemes()
    {
        using var _ = WithEnv(
            ("CORS_ALLOWED_ORIGINS", "https://staging.example.com")
        );

        // Arrange
        await using var factory = new StagingWebApplicationFactory();
        var client = factory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, "/health");
        request.Headers.Add("Origin", "ftp://example.com");

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.False(
            response.Headers.Contains("Access-Control-Allow-Origin"),
            "Expected non-HTTP/HTTPS scheme to be rejected"
        );
    }

    [Fact]
    public async Task Production_NormalizesOrigins_WithDefaultPort()
    {
        using var _ = WithEnv(
            ("CORS_ALLOWED_ORIGINS", "https://app.example.com:443,http://test.example.com:80")
        );
        
        // Arrange
        await using var factory = new ProductionWebApplicationFactory();
        var client = factory.CreateClient();
        
        // Act - Test HTTPS with default port
        using var request1 = new HttpRequestMessage(HttpMethod.Get, "/health");
        request1.Headers.Add("Origin", "https://app.example.com");
        var response1 = await client.SendAsync(request1);
        
        // Act - Test HTTP with default port
        using var request2 = new HttpRequestMessage(HttpMethod.Get, "/health");
        request2.Headers.Add("Origin", "http://test.example.com");
        var response2 = await client.SendAsync(request2);
        
        // Assert
        Assert.True(
            response1.Headers.TryGetValues("Access-Control-Allow-Origin", out var values1) &&
            values1.Contains("https://app.example.com"),
            "Expected :443 to be removed for HTTPS"
        );
        Assert.True(
            response2.Headers.TryGetValues("Access-Control-Allow-Origin", out var values2) &&
            values2.Contains("http://test.example.com"),
            "Expected :80 to be removed for HTTP"
        );
    }

    [Fact]
    public async Task Production_DeduplicatesOrigins()
    {
        using var _ = WithEnv(
            ("CORS_ALLOWED_ORIGINS", "https://app.example.com,https://APP.example.com,https://app.example.com")
        );
        
        // Arrange
        await using var factory = new ProductionWebApplicationFactory();
        var client = factory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, "/health");
        request.Headers.Add("Origin", "https://app.example.com");
        
        // Act
        var response = await client.SendAsync(request);
        
        // Assert
        Assert.True(
            response.Headers.TryGetValues("Access-Control-Allow-Origin", out var values) &&
            values.Contains("https://app.example.com"),
            "Expected duplicate origins to be deduplicated"
        );
    }

    [Fact]
    public async Task Production_NormalizesOrigins_WithPathQueryFragment()
    {
        using var _ = WithEnv(
            ("CORS_ALLOWED_ORIGINS", "https://app.example.com/path?query=1#fragment")
        );
        
        // Arrange
        await using var factory = new ProductionWebApplicationFactory();
        var client = factory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, "/health");
        request.Headers.Add("Origin", "https://app.example.com");
        
        // Act
        var response = await client.SendAsync(request);
        
        // Assert
        Assert.True(
            response.Headers.TryGetValues("Access-Control-Allow-Origin", out var values) &&
            values.Contains("https://app.example.com"),
            "Expected origin with path/query/fragment to be normalized to origin form"
        );
    }
}
