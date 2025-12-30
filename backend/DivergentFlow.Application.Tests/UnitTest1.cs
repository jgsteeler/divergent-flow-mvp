using DivergentFlow.Application.Abstractions;
using DivergentFlow.Application.Features.TypeInference.Commands;
using DivergentFlow.Application.Features.TypeInference.Handlers;
using DivergentFlow.Application.Features.TypeInference.Queries;
using DivergentFlow.Application.Features.TypeInference.Validation;
using DivergentFlow.Application.Models;
using Xunit;

namespace DivergentFlow.Application.Tests;

public sealed class TypeInferenceApplicationTests
{
    [Fact]
    public void InferTypeQueryValidator_RejectsEmptyText()
    {
        var validator = new InferTypeQueryValidator();
        var result = validator.Validate(new InferTypeQuery(""));

        Assert.False(result.IsValid);
    }

    [Fact]
    public void InferTypeQueryValidator_AcceptsNonEmptyText()
    {
        var validator = new InferTypeQueryValidator();
        var result = validator.Validate(new InferTypeQuery("Buy groceries"));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void ConfirmTypeCommandValidator_RejectsOutOfRangeConfidence()
    {
        var validator = new ConfirmTypeCommandValidator();
        var command = new ConfirmTypeCommand(new TypeConfirmationRequest
        {
            Text = "Buy groceries",
            InferredType = "action",
            InferredConfidence = 150,
            ConfirmedType = "action"
        });

        var result = validator.Validate(command);
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task InferTypeHandler_DelegatesToService()
    {
        var service = new FakeTypeInferenceService();
        var handler = new InferTypeHandler(service);

        var result = await handler.Handle(new InferTypeQuery("anything"), CancellationToken.None);

        Assert.True(service.InferCalled);
        Assert.Equal("action", result.InferredType);
        Assert.Equal(50.0, result.Confidence);
    }

    [Fact]
    public async Task ConfirmTypeHandler_DelegatesToService()
    {
        var service = new FakeTypeInferenceService();
        var handler = new ConfirmTypeHandler(service);

        await handler.Handle(new ConfirmTypeCommand(new TypeConfirmationRequest
        {
            Text = "Buy groceries",
            InferredType = "action",
            InferredConfidence = 50,
            ConfirmedType = "note"
        }), CancellationToken.None);

        Assert.True(service.ConfirmCalled);
    }

    private sealed class FakeTypeInferenceService : ITypeInferenceService
    {
        public bool InferCalled { get; private set; }
        public bool ConfirmCalled { get; private set; }

        public Task<TypeInferenceResult> InferAsync(string text, CancellationToken cancellationToken = default)
        {
            InferCalled = true;
            return Task.FromResult(new TypeInferenceResult
            {
                InferredType = "action",
                Confidence = 50.0
            });
        }

        public Task ConfirmAsync(TypeConfirmationRequest request, CancellationToken cancellationToken = default)
        {
            ConfirmCalled = true;
            return Task.CompletedTask;
        }
    }
}
