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
    private static IDisposable WithEnv(params (string Key, string? Value)[] entries)
    {
        var previous = new Dictionary<string, string?>();
        foreach (var (key, value) in entries)
        {
            previous[key] = Environment.GetEnvironmentVariable(key);
            Environment.SetEnvironmentVariable(key, value);
        }

        return new DisposableAction(() =>
        {
            foreach (var (key, _) in entries)
            {
                Environment.SetEnvironmentVariable(key, previous[key]);
            }
        });
    }

    private sealed class DisposableAction : IDisposable
    {
        private readonly Action _dispose;
        public DisposableAction(Action dispose) => _dispose = dispose;
        public void Dispose() => _dispose();
    }

    [Fact]
    public async Task Staging_Allows_Netlify_DeployPreview_Origin()
    {
        using var _ = WithEnv(
            ("CORS_NETLIFY_SITE_NAME", "div-flo-mvp"),
            ("CORS_ALLOW_NETLIFY_PREVIEWS", "true"),
            ("CORS_STAGING_ORIGINS", "")
        );

        // Arrange
        await using var factory = new StagingWebApplicationFactory();
        var client = factory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, "/health");
        request.Headers.Add("Origin", "https://deploy-preview-123--div-flo-mvp.netlify.app");

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.True(
            response.Headers.TryGetValues("Access-Control-Allow-Origin", out var values) &&
            values.Contains("https://deploy-preview-123--div-flo-mvp.netlify.app"),
            "Expected Access-Control-Allow-Origin to echo the Netlify deploy preview origin in Staging"
        );
    }

    [Fact]
    public async Task Staging_Allows_Netlify_BranchDeploy_Origin()
    {
        using var _ = WithEnv(
            ("CORS_NETLIFY_SITE_NAME", "div-flo-mvp"),
            ("CORS_ALLOW_NETLIFY_PREVIEWS", "true"),
            ("CORS_STAGING_ORIGINS", "")
        );

        // Arrange
        await using var factory = new StagingWebApplicationFactory();
        var client = factory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, "/health");
        request.Headers.Add("Origin", "https://feature-cool-thing--div-flo-mvp.netlify.app");

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.True(
            response.Headers.TryGetValues("Access-Control-Allow-Origin", out var values) &&
            values.Contains("https://feature-cool-thing--div-flo-mvp.netlify.app"),
            "Expected Access-Control-Allow-Origin to echo the Netlify branch deploy origin in Staging"
        );
    }

    [Fact]
    public async Task Staging_Allows_Explicitly_Configured_Origins()
    {
        using var _ = WithEnv(
            ("CORS_STAGING_ORIGINS", "https://staging.example.com,https://test.example.com"),
            ("CORS_NETLIFY_SITE_NAME", "div-flo-mvp"),
            ("CORS_ALLOW_NETLIFY_PREVIEWS", "false")
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
            "Expected Access-Control-Allow-Origin to allow explicitly configured staging origin"
        );
    }

    [Fact]
    public async Task Production_DoesNotAllow_Netlify_DeployPreview_Origin()
    {
        using var _ = WithEnv(
            ("CORS_PRODUCTION_ORIGINS", "https://app.getdivergentflow.com")
        );

        // Arrange
        await using var factory = new ProductionWebApplicationFactory();
        var client = factory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, "/health");
        request.Headers.Add("Origin", "https://deploy-preview-123--div-flo-mvp.netlify.app");

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.False(
            response.Headers.Contains("Access-Control-Allow-Origin"),
            "Did not expect Access-Control-Allow-Origin for Netlify deploy preview origin in Production"
        );
    }

    [Fact]
    public async Task Production_Allows_Custom_Domain_Origin()
    {
        using var _ = WithEnv(
            ("CORS_PRODUCTION_ORIGINS", "https://app.getdivergentflow.com")
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
            "Expected Access-Control-Allow-Origin to allow the custom domain origin in Production"
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
}
