using System.Text.Json;
using DivergentFlow.Api.Middleware;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;

namespace DivergentFlow.Api.Tests.Middleware;

public sealed class ValidationExceptionMiddlewareTests
{
    [Fact]
    public async Task Invoke_NoException_CallsNextMiddleware()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var nextCalled = false;
        RequestDelegate next = _ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        };

        var middleware = new ValidationExceptionMiddleware(next);

        // Act
        await middleware.Invoke(context);

        // Assert
        Assert.True(nextCalled);
        Assert.Equal(200, context.Response.StatusCode);
    }

    [Fact]
    public async Task Invoke_ValidationException_Returns400WithErrors()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var failures = new[]
        {
            new ValidationFailure("Text", "Text is required"),
            new ValidationFailure("TypeConfidence", "TypeConfidence must be between 0 and 100")
        };

        RequestDelegate next = _ => throw new ValidationException(failures);

        var middleware = new ValidationExceptionMiddleware(next);

        // Act
        await middleware.Invoke(context);

        // Assert
        Assert.Equal(400, context.Response.StatusCode);
        Assert.StartsWith("application/json", context.Response.ContentType);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(context.Response.Body);
        var responseBody = await reader.ReadToEndAsync();
        var response = JsonSerializer.Deserialize<JsonElement>(responseBody);

        Assert.Equal("Validation failed", response.GetProperty("title").GetString());
        Assert.Equal(400, response.GetProperty("status").GetInt32());
        Assert.True(response.GetProperty("errors").TryGetProperty("Text", out _));
        Assert.True(response.GetProperty("errors").TryGetProperty("TypeConfidence", out _));
    }

    [Fact]
    public async Task Invoke_ValidationException_GroupsErrorsByProperty()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var failures = new[]
        {
            new ValidationFailure("Text", "Text is required"),
            new ValidationFailure("Text", "Text must not be empty"),
            new ValidationFailure("Id", "Id is required")
        };

        RequestDelegate next = _ => throw new ValidationException(failures);

        var middleware = new ValidationExceptionMiddleware(next);

        // Act
        await middleware.Invoke(context);

        // Assert
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var reader = new StreamReader(context.Response.Body);
        var responseBody = await reader.ReadToEndAsync();
        var response = JsonSerializer.Deserialize<JsonElement>(responseBody);

        var errors = response.GetProperty("errors");
        Assert.Equal(2, errors.EnumerateObject().Count()); // Text and Id
        
        var textErrors = errors.GetProperty("Text");
        Assert.Equal(2, textErrors.GetArrayLength());
    }

    [Fact]
    public async Task Invoke_ValidationException_RemovesDuplicateErrorMessages()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var failures = new[]
        {
            new ValidationFailure("Text", "Text is required"),
            new ValidationFailure("Text", "Text is required"), // Duplicate
            new ValidationFailure("Text", "Text must not be empty")
        };

        RequestDelegate next = _ => throw new ValidationException(failures);

        var middleware = new ValidationExceptionMiddleware(next);

        // Act
        await middleware.Invoke(context);

        // Assert
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(context.Response.Body);
        var responseBody = await reader.ReadToEndAsync();
        var response = JsonSerializer.Deserialize<JsonElement>(responseBody);

        var textErrors = response.GetProperty("errors").GetProperty("Text");
        Assert.Equal(2, textErrors.GetArrayLength()); // Should have 2, not 3
    }

    [Fact]
    public async Task Invoke_NonValidationException_Rethrows()
    {
        // Arrange
        var context = new DefaultHttpContext();
        RequestDelegate next = _ => throw new InvalidOperationException("Test exception");

        var middleware = new ValidationExceptionMiddleware(next);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => middleware.Invoke(context));
    }
}
