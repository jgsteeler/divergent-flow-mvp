using DivergentFlow.Domain.Entities;
using DivergentFlow.Infrastructure.Repositories;
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
    public async Task InMemoryCaptureRepository_CreateThenGetById_ReturnsClone()
    {
        var repo = new InMemoryCaptureRepository();
        var created = await repo.CreateAsync(new Capture
        {
            Id = "id-1",
            Text = "hello",
            CreatedAt = 123
        });

        created.Text = "mutated";

        var fetched = await repo.GetByIdAsync("id-1");

        Assert.NotNull(fetched);
        Assert.Equal("hello", fetched!.Text);
    }

    [Fact]
    public async Task InMemoryCaptureRepository_UpdateMissing_ReturnsNull()
    {
        var repo = new InMemoryCaptureRepository();
        var updated = await repo.UpdateAsync("missing", new Capture { Id = "id", Text = "x", CreatedAt = 1 });
        Assert.Null(updated);
    }
}
