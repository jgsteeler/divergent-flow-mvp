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
        var userId = "local";
        var itemId = "test-item-1";

        // Act
        await _queue.EnqueueAsync(userId, itemId);

        // Assert - if we can dequeue, the item was enqueued
        var workItem = await _queue.DequeueAsync();
        Assert.Equal(userId, workItem.UserId);
        Assert.Equal(itemId, workItem.ItemId);
    }

    [Fact]
    public async Task DequeueAsync_ReturnsItemsInFifoOrder()
    {
        // Arrange
        var userId = "local";
        await _queue.EnqueueAsync(userId, "item-1");
        await _queue.EnqueueAsync(userId, "item-2");
        await _queue.EnqueueAsync(userId, "item-3");

        // Act
        var first = await _queue.DequeueAsync();
        var second = await _queue.DequeueAsync();
        var third = await _queue.DequeueAsync();

        // Assert
        Assert.Equal("item-1", first.ItemId);
        Assert.Equal("item-2", second.ItemId);
        Assert.Equal("item-3", third.ItemId);
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
        var userId = "local";
        var tasks = new List<Task>();
        for (int i = 0; i < 10; i++)
        {
            var itemId = $"item-{i}";
            tasks.Add(Task.Run(async () => await _queue.EnqueueAsync(userId, itemId)));
        }

        await Task.WhenAll(tasks);

        // Assert - Dequeue all items
        var dequeuedItems = new List<string>();
        for (int i = 0; i < 10; i++)
        {
            dequeuedItems.Add((await _queue.DequeueAsync()).ItemId);
        }

        Assert.Equal(10, dequeuedItems.Count);
        Assert.Equal(10, dequeuedItems.Distinct().Count()); // All unique
    }
}
