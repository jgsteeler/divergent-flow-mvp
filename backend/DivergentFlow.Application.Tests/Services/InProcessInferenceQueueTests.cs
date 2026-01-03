using DivergentFlow.Application.Abstractions;
using DivergentFlow.Application.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace DivergentFlow.Application.Tests.Services;

public sealed class InProcessInferenceQueueTests
{
    private readonly Mock<ILogger<InProcessInferenceQueue>> _mockLogger;
    private readonly InProcessInferenceQueue _queue;

    public InProcessInferenceQueueTests()
    {
        _mockLogger = new Mock<ILogger<InProcessInferenceQueue>>();
        _queue = new InProcessInferenceQueue(_mockLogger.Object);
    }

    [Fact]
    public async Task EnqueueAsync_EnqueuesItemSuccessfully()
    {
        // Arrange
        var itemId = "test-item-1";

        // Act
        await _queue.EnqueueAsync(itemId);

        // Assert - if we can dequeue, the item was enqueued
        var dequeuedId = await _queue.DequeueAsync();
        Assert.Equal(itemId, dequeuedId);
    }

    [Fact]
    public async Task DequeueAsync_ReturnsItemsInFifoOrder()
    {
        // Arrange
        await _queue.EnqueueAsync("item-1");
        await _queue.EnqueueAsync("item-2");
        await _queue.EnqueueAsync("item-3");

        // Act
        var first = await _queue.DequeueAsync();
        var second = await _queue.DequeueAsync();
        var third = await _queue.DequeueAsync();

        // Assert
        Assert.Equal("item-1", first);
        Assert.Equal("item-2", second);
        Assert.Equal("item-3", third);
    }

    [Fact]
    public async Task DequeueAsync_WithCancellation_ThrowsOperationCanceledException()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert - TaskCanceledException is a subtype of OperationCanceledException
        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => _queue.DequeueAsync(cts.Token).AsTask());
    }

    [Fact]
    public async Task Queue_SupportsMultipleWriters()
    {
        // Arrange & Act - Simulate multiple writers
        var tasks = new List<Task>();
        for (int i = 0; i < 10; i++)
        {
            var itemId = $"item-{i}";
            tasks.Add(Task.Run(async () => await _queue.EnqueueAsync(itemId)));
        }

        await Task.WhenAll(tasks);

        // Assert - Dequeue all items
        var dequeuedItems = new List<string>();
        for (int i = 0; i < 10; i++)
        {
            dequeuedItems.Add(await _queue.DequeueAsync());
        }

        Assert.Equal(10, dequeuedItems.Count);
        Assert.Equal(10, dequeuedItems.Distinct().Count()); // All unique
    }
}
