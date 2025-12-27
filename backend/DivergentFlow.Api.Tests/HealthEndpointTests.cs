using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace DivergentFlow.Api.Tests;

/// <summary>
/// Integration tests for general API endpoints (non-controller)
/// </summary>
public class HealthEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public HealthEndpointTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Health_ReturnsOk_WithHealthyStatus()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/health");

        // Assert
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<HealthResponse>();
        Assert.NotNull(body);
        Assert.Equal("Healthy", body.Status);
    }

    private record HealthResponse(string Status);
}
