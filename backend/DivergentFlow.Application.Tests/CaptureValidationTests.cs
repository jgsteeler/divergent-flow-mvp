using DivergentFlow.Application.Features.Captures.Commands;
using DivergentFlow.Application.Features.Captures.Validation;
using Xunit;

namespace DivergentFlow.Application.Tests;

public sealed class CaptureValidationTests
{
    [Fact]
    public void CreateCaptureCommandValidator_Rejects_EmptyText()
    {
        var validator = new CreateCaptureCommandValidator();

        var result = validator.Validate(new CreateCaptureCommand(
            Text: "",
            InferredType: null,
            TypeConfidence: null
        ));

        Assert.False(result.IsValid);
    }

    [Fact]
    public void CreateCaptureCommandValidator_Accepts_NonEmptyText_WithNullConfidence()
    {
        var validator = new CreateCaptureCommandValidator();

        var result = validator.Validate(new CreateCaptureCommand(
            Text: "Buy groceries",
            InferredType: null,
            TypeConfidence: null
        ));

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(100.1)]
    public void CreateCaptureCommandValidator_Rejects_Confidence_OutOfRange(double confidence)
    {
        var validator = new CreateCaptureCommandValidator();

        var result = validator.Validate(new CreateCaptureCommand(
            Text: "Test capture",
            InferredType: "note",
            TypeConfidence: confidence
        ));

        Assert.False(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(50)]
    [InlineData(100)]
    public void CreateCaptureCommandValidator_Accepts_Confidence_InRange(double confidence)
    {
        var validator = new CreateCaptureCommandValidator();

        var result = validator.Validate(new CreateCaptureCommand(
            Text: "Test capture",
            InferredType: "note",
            TypeConfidence: confidence
        ));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void UpdateCaptureCommandValidator_Rejects_EmptyId()
    {
        var validator = new UpdateCaptureCommandValidator();

        var result = validator.Validate(new UpdateCaptureCommand(
            Id: "",
            Text: "Updated",
            InferredType: null,
            TypeConfidence: null
        ));

        Assert.False(result.IsValid);
    }

    [Fact]
    public void UpdateCaptureCommandValidator_Rejects_EmptyText()
    {
        var validator = new UpdateCaptureCommandValidator();

        var result = validator.Validate(new UpdateCaptureCommand(
            Id: "capture-1",
            Text: "",
            InferredType: null,
            TypeConfidence: null
        ));

        Assert.False(result.IsValid);
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(100.1)]
    public void UpdateCaptureCommandValidator_Rejects_Confidence_OutOfRange(double confidence)
    {
        var validator = new UpdateCaptureCommandValidator();

        var result = validator.Validate(new UpdateCaptureCommand(
            Id: "capture-1",
            Text: "Updated",
            InferredType: "action",
            TypeConfidence: confidence
        ));

        Assert.False(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(100)]
    public void UpdateCaptureCommandValidator_Accepts_Confidence_InRange(double confidence)
    {
        var validator = new UpdateCaptureCommandValidator();

        var result = validator.Validate(new UpdateCaptureCommand(
            Id: "capture-1",
            Text: "Updated",
            InferredType: "action",
            TypeConfidence: confidence
        ));

        Assert.True(result.IsValid);
    }
}
