using DivergentFlow.Domain.Entities;
using DivergentFlow.Infrastructure.Services;
using Xunit;

namespace DivergentFlow.Infrastructure.Tests;

public sealed class InfrastructureUnitTests
{
    [Fact]
    public async Task BasicTypeInferenceService_ReturnsActionWithFixedConfidence()
    {
        var service = new BasicTypeInferenceService();
        var result = await service.InferAsync("anything");

        Assert.Equal("action", result.InferredType);
        Assert.Equal(50.0, result.Confidence);
    }

    [Fact]
    public async Task BasicTypeInferenceService_ThrowsOnEmptyText()
    {
        var service = new BasicTypeInferenceService();
        await Assert.ThrowsAsync<ArgumentException>(() => service.InferAsync(""));
    }

    [Fact]
    public void RedisCaptureStorage_CaptureKey_IsStable()
    {
        var key = DivergentFlow.Infrastructure.Repositories.RedisCaptureStorage.CaptureKey("id-1");
        Assert.Equal("capture:id-1", key);
    }

    [Fact]
    public void RedisCaptureStorage_SerializeRoundTrip_Works()
    {
        var original = new Capture
        {
            Id = "id-1",
            Text = "hello",
            CreatedAt = 123,
            InferredType = "note",
            TypeConfidence = 0.5
        };

        var json = DivergentFlow.Infrastructure.Repositories.RedisCaptureStorage.Serialize(original);
        var roundTripped = DivergentFlow.Infrastructure.Repositories.RedisCaptureStorage.Deserialize(json);

        Assert.NotNull(roundTripped);
        Assert.Equal(original.Id, roundTripped!.Id);
        Assert.Equal(original.Text, roundTripped.Text);
        Assert.Equal(original.CreatedAt, roundTripped.CreatedAt);
        Assert.Equal(original.InferredType, roundTripped.InferredType);
        Assert.Equal(original.TypeConfidence, roundTripped.TypeConfidence);
    }
}
