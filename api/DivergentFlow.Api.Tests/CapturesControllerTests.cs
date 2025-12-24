using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using DivergentFlow.Services.Models;
using DivergentFlow.Services.Services;
using Xunit;

namespace DivergentFlow.Api.Tests;

/// <summary>
/// Custom factory for integration tests that uses a fresh in-memory service for each test
/// </summary>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing service registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(ICaptureService));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add a fresh in-memory service for each test
            services.AddSingleton<ICaptureService, InMemoryCaptureService>();
        });
    }
}

/// <summary>
/// Integration tests for the Captures API endpoints
/// </summary>
public class CapturesControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public CapturesControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetAll_ReturnsEmptyList_WhenNoCaptures()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/captures");

        // Assert
        response.EnsureSuccessStatusCode();
        var captures = await response.Content.ReadFromJsonAsync<List<CaptureDto>>();
        Assert.NotNull(captures);
    }

    [Fact]
    public async Task Create_ReturnsCreatedCapture_WithValidRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new CreateCaptureRequest
        {
            Text = "Test capture"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/captures", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var capture = await response.Content.ReadFromJsonAsync<CaptureDto>();
        Assert.NotNull(capture);
        Assert.NotNull(capture.Id);
        Assert.Equal("Test capture", capture.Text);
        Assert.True(capture.CreatedAt > 0);
    }

    [Fact]
    public async Task GetById_ReturnsCapture_WhenExists()
    {
        // Arrange
        var client = _factory.CreateClient();
        var createRequest = new CreateCaptureRequest { Text = "Test capture" };
        var createResponse = await client.PostAsJsonAsync("/api/captures", createRequest);
        var createdCapture = await createResponse.Content.ReadFromJsonAsync<CaptureDto>();

        // Act
        var response = await client.GetAsync($"/api/captures/{createdCapture!.Id}");

        // Assert
        response.EnsureSuccessStatusCode();
        var capture = await response.Content.ReadFromJsonAsync<CaptureDto>();
        Assert.NotNull(capture);
        Assert.Equal(createdCapture.Id, capture.Id);
        Assert.Equal("Test capture", capture.Text);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenDoesNotExist()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/captures/nonexistent-id");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_ReturnsUpdatedCapture_WhenExists()
    {
        // Arrange
        var client = _factory.CreateClient();
        var createRequest = new CreateCaptureRequest { Text = "Original text" };
        var createResponse = await client.PostAsJsonAsync("/api/captures", createRequest);
        var createdCapture = await createResponse.Content.ReadFromJsonAsync<CaptureDto>();

        var updateRequest = new UpdateCaptureRequest { Text = "Updated text" };

        // Act
        var response = await client.PutAsJsonAsync($"/api/captures/{createdCapture!.Id}", updateRequest);

        // Assert
        response.EnsureSuccessStatusCode();
        var updatedCapture = await response.Content.ReadFromJsonAsync<CaptureDto>();
        Assert.NotNull(updatedCapture);
        Assert.Equal(createdCapture.Id, updatedCapture.Id);
        Assert.Equal("Updated text", updatedCapture.Text);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenExists()
    {
        // Arrange
        var client = _factory.CreateClient();
        var createRequest = new CreateCaptureRequest { Text = "To be deleted" };
        var createResponse = await client.PostAsJsonAsync("/api/captures", createRequest);
        var createdCapture = await createResponse.Content.ReadFromJsonAsync<CaptureDto>();

        // Act
        var response = await client.DeleteAsync($"/api/captures/{createdCapture!.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify it's deleted
        var getResponse = await client.GetAsync($"/api/captures/{createdCapture.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenTextIsEmpty()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new CreateCaptureRequest { Text = "" };

        // Act
        var response = await client.PostAsJsonAsync("/api/captures", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
