namespace DivergentFlow.Infrastructure.Services.Upstash;

public interface IUpstashRedisRestReadClient
{
    Task<string?> GetAsync(string key, CancellationToken cancellationToken = default);

    Task<string[]> SMembersAsync(string key, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string?>> MGetAsync(IReadOnlyList<string> keys, CancellationToken cancellationToken = default);
}

public interface IUpstashRedisRestWriteClient
{
    Task SetAsync(string key, string jsonValue, CancellationToken cancellationToken = default);

    Task<long> DelAsync(string key, CancellationToken cancellationToken = default);

    Task<long> SAddAsync(string key, string member, CancellationToken cancellationToken = default);

    Task<long> SRemAsync(string key, string member, CancellationToken cancellationToken = default);

    Task ExecuteTransactionAsync(
        IReadOnlyList<IReadOnlyList<object?>> commands,
        CancellationToken cancellationToken = default);
}
