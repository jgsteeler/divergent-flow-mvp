using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace DivergentFlow.Infrastructure.Services.Upstash;

public sealed class UpstashRedisRestClient : IUpstashRedisRestReadClient, IUpstashRedisRestWriteClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly HttpClient _http;

    public UpstashRedisRestClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<string?> GetAsync(string key, CancellationToken cancellationToken = default)
    {
        var result = await ExecuteAsync(new object[] { "GET", key }, cancellationToken).ConfigureAwait(false);
        if (result is null || result.Value.ValueKind == JsonValueKind.Null)
        {
            return null;
        }

        return result.Value.ValueKind == JsonValueKind.String
            ? result.Value.GetString()
            : result.Value.GetRawText();
    }

    public async Task SetAsync(string key, string jsonValue, CancellationToken cancellationToken = default)
    {
        var result = await ExecuteAsync(new object[] { "SET", key, jsonValue }, cancellationToken).ConfigureAwait(false);
        if (result is null)
        {
            throw new UpstashRedisRestException("Unexpected null result for SET");
        }

        if (result.Value.ValueKind == JsonValueKind.String && string.Equals(result.Value.GetString(), "OK", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        throw new UpstashRedisRestException($"Unexpected SET result: {result.Value.GetRawText()}");
    }

    public async Task<long> DelAsync(string key, CancellationToken cancellationToken = default)
    {
        var result = await ExecuteAsync(new object[] { "DEL", key }, cancellationToken).ConfigureAwait(false);
        return GetInt64OrThrow(result, "DEL");
    }

    public async Task<long> SAddAsync(string key, string member, CancellationToken cancellationToken = default)
    {
        var result = await ExecuteAsync(new object[] { "SADD", key, member }, cancellationToken).ConfigureAwait(false);
        return GetInt64OrThrow(result, "SADD");
    }

    public async Task<long> SRemAsync(string key, string member, CancellationToken cancellationToken = default)
    {
        var result = await ExecuteAsync(new object[] { "SREM", key, member }, cancellationToken).ConfigureAwait(false);
        return GetInt64OrThrow(result, "SREM");
    }

    public async Task<string[]> SMembersAsync(string key, CancellationToken cancellationToken = default)
    {
        var result = await ExecuteAsync(new object[] { "SMEMBERS", key }, cancellationToken).ConfigureAwait(false);
        if (result is null || result.Value.ValueKind == JsonValueKind.Null)
        {
            return Array.Empty<string>();
        }

        if (result.Value.ValueKind != JsonValueKind.Array)
        {
            throw new UpstashRedisRestException($"Unexpected SMEMBERS result: {result.Value.GetRawText()}");
        }

        var values = new List<string>();
        foreach (var element in result.Value.EnumerateArray())
        {
            if (element.ValueKind == JsonValueKind.String)
            {
                values.Add(element.GetString() ?? string.Empty);
            }
        }

        return values.Where(v => !string.IsNullOrWhiteSpace(v)).ToArray();
    }

    public async Task<IReadOnlyList<string?>> MGetAsync(IReadOnlyList<string> keys, CancellationToken cancellationToken = default)
    {
        if (keys.Count == 0)
        {
            return Array.Empty<string?>();
        }

        var command = new object[keys.Count + 1];
        command[0] = "MGET";
        for (var i = 0; i < keys.Count; i++)
        {
            command[i + 1] = keys[i];
        }

        var result = await ExecuteAsync(command, cancellationToken).ConfigureAwait(false);
        if (result is null || result.Value.ValueKind == JsonValueKind.Null)
        {
            return Array.Empty<string?>();
        }

        if (result.Value.ValueKind != JsonValueKind.Array)
        {
            throw new UpstashRedisRestException($"Unexpected MGET result: {result.Value.GetRawText()}");
        }

        var values = new List<string?>(keys.Count);
        foreach (var element in result.Value.EnumerateArray())
        {
            if (element.ValueKind == JsonValueKind.Null)
            {
                values.Add(null);
                continue;
            }

            if (element.ValueKind == JsonValueKind.String)
            {
                values.Add(element.GetString());
                continue;
            }

            values.Add(element.GetRawText());
        }

        return values;
    }

    public async Task ExecuteTransactionAsync(
        IReadOnlyList<IReadOnlyList<object?>> commands,
        CancellationToken cancellationToken = default)
    {
        if (commands.Count == 0)
        {
            return;
        }

        using var request = new HttpRequestMessage(HttpMethod.Post, "multi-exec");
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var payload = JsonSerializer.Serialize(commands, JsonOptions);
        request.Content = new StringContent(payload, Encoding.UTF8, "application/json");

        using var response = await _http.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var body = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            throw new UpstashRedisRestException($"Upstash multi-exec failed ({(int)response.StatusCode}): {body}");
        }

        // Body is an array of { result | error } items.
        using var doc = JsonDocument.Parse(body);
        if (doc.RootElement.ValueKind == JsonValueKind.Object
            && doc.RootElement.TryGetProperty("error", out var topLevelError)
            && topLevelError.ValueKind == JsonValueKind.String)
        {
            throw new UpstashRedisRestException($"Upstash multi-exec error: {topLevelError.GetString()}");
        }

        if (doc.RootElement.ValueKind != JsonValueKind.Array)
        {
            throw new UpstashRedisRestException($"Unexpected multi-exec response: {body}");
        }

        foreach (var element in doc.RootElement.EnumerateArray())
        {
            if (element.ValueKind == JsonValueKind.Object
                && element.TryGetProperty("error", out var error)
                && error.ValueKind == JsonValueKind.String)
            {
                throw new UpstashRedisRestException($"Upstash multi-exec command error: {error.GetString()}");
            }
        }
    }

    internal static UpstashRedisRestResponse ParseResponse(string json)
    {
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        JsonElement? result = null;
        string? error = null;

        if (root.ValueKind == JsonValueKind.Object)
        {
            if (root.TryGetProperty("error", out var errorElement) && errorElement.ValueKind == JsonValueKind.String)
            {
                error = errorElement.GetString();
            }

            if (root.TryGetProperty("result", out var resultElement))
            {
                result = resultElement.Clone();
            }
        }

        return new UpstashRedisRestResponse(result, error);
    }

    private async Task<JsonElement?> ExecuteAsync(object[] command, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, string.Empty);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var payload = JsonSerializer.Serialize(command, JsonOptions);
        request.Content = new StringContent(payload, Encoding.UTF8, "application/json");

        using var response = await _http.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var body = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            throw new UpstashRedisRestException($"Upstash request failed ({(int)response.StatusCode}): {body}");
        }

        var parsed = ParseResponse(body);
        if (!string.IsNullOrWhiteSpace(parsed.Error))
        {
            throw new UpstashRedisRestException($"Upstash error: {parsed.Error}");
        }

        return parsed.Result;
    }

    private static long GetInt64OrThrow(JsonElement? element, string command)
    {
        if (element is null)
        {
            throw new UpstashRedisRestException($"Unexpected null result for {command}");
        }

        if (element.Value.ValueKind == JsonValueKind.Number && element.Value.TryGetInt64(out var value))
        {
            return value;
        }

        throw new UpstashRedisRestException($"Unexpected {command} result: {element.Value.GetRawText()}");
    }
}
