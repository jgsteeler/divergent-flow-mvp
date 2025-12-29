using System.Net;
using System.Net.Http.Json;
using DivergentFlow.Application.Models;
using Xunit;

namespace DivergentFlow.Api.Tests;

/// <summary>
/// Integration tests for the Type API endpoints
/// </summary>
public class TypeControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public TypeControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Infer_ReturnsTypeInferenceResult_WithValidRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new TypeInferenceRequest
        {
            Text = "Buy groceries tomorrow"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/type/infer", request);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<TypeInferenceResult>();
        Assert.NotNull(result);
        Assert.Equal("action", result.InferredType);
        Assert.Equal(50.0, result.Confidence);
    }

    [Fact]
    public async Task Infer_ReturnsBadRequest_WhenTextIsEmpty()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new TypeInferenceRequest
        {
            Text = ""
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/type/infer", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Infer_ReturnsBadRequest_WhenTextIsNull()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new { Text = (string?)null };

        // Act
        var response = await client.PostAsJsonAsync("/api/type/infer", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Infer_ReturnsActionType_ForAnyText()
    {
        // Arrange
        var client = _factory.CreateClient();
        var testTexts = new[]
        {
            "Call mom",
            "Remember to review the PRD",
            "Meeting at 3pm",
            "Random note about ideas"
        };

        foreach (var text in testTexts)
        {
            var request = new TypeInferenceRequest { Text = text };

            // Act
            var response = await client.PostAsJsonAsync("/api/type/infer", request);

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<TypeInferenceResult>();
            Assert.NotNull(result);
            Assert.Equal("action", result.InferredType);
            Assert.Equal(50.0, result.Confidence);
        }
    }

    [Fact]
    public async Task Confirm_ReturnsNoContent_WithValidRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new TypeConfirmationRequest
        {
            Text = "Buy groceries",
            InferredType = "action",
            InferredConfidence = 50.0,
            ConfirmedType = "action"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/type/confirm", request);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Confirm_ReturnsNoContent_WhenUserChangesType()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new TypeConfirmationRequest
        {
            Text = "Buy groceries",
            InferredType = "action",
            InferredConfidence = 50.0,
            ConfirmedType = "note" // User changed it
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/type/confirm", request);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Confirm_ReturnsBadRequest_WhenTextIsEmpty()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new TypeConfirmationRequest
        {
            Text = "",
            InferredType = "action",
            InferredConfidence = 50.0,
            ConfirmedType = "action"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/type/confirm", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Confirm_ReturnsBadRequest_WhenInferredTypeIsEmpty()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new TypeConfirmationRequest
        {
            Text = "Buy groceries",
            InferredType = "",
            InferredConfidence = 50.0,
            ConfirmedType = "action"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/type/confirm", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Confirm_ReturnsBadRequest_WhenConfidenceOutOfRange()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new TypeConfirmationRequest
        {
            Text = "Buy groceries",
            InferredType = "action",
            InferredConfidence = 150.0, // Invalid: > 100
            ConfirmedType = "action"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/type/confirm", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
