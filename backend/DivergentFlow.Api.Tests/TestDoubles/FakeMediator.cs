using MediatR;

namespace DivergentFlow.Api.Tests.TestDoubles;

public sealed class FakeMediator : IMediator
{
    private readonly Func<object, CancellationToken, Task<object?>> _send;

    public FakeMediator(Func<object, CancellationToken, Task<object?>> send)
    {
        _send = send;
    }

    public object? LastRequest { get; private set; }

    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        LastRequest = request;
        var result = await _send(request, cancellationToken);
        if (result is null)
        {
            return default!;
        }

        return (TResponse)result;
    }

    public async Task<object?> Send(object request, CancellationToken cancellationToken = default)
    {
        LastRequest = request;
        return await _send(request, cancellationToken);
    }

    public Task Publish(object notification, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification
    {
        return Task.CompletedTask;
    }

    public IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("Streaming is not supported by this FakeMediator.");
    }

    public IAsyncEnumerable<object?> CreateStream(object request, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("Streaming is not supported by this FakeMediator.");
    }
}
