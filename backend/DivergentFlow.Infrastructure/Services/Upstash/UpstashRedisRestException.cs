namespace DivergentFlow.Infrastructure.Services.Upstash;

public sealed class UpstashRedisRestException : Exception
{
    public UpstashRedisRestException(string message) : base(message)
    {
    }

    public UpstashRedisRestException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
