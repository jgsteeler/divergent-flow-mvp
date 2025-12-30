namespace DivergentFlow.Infrastructure.Services.Upstash;

internal static class UpstashRedisRestUri
{
    internal static Uri BuildBaseUri(string raw)
    {
        // Upstash recommends using the HTTPS REST URL from the console.
        // Some existing setups pass host:port or http(s)://host[:port].
        // Normalize into an absolute HTTPS/HTTP URI.

        if (Uri.TryCreate(raw, UriKind.Absolute, out var absolute))
        {
            // "host:port" can be parsed as an absolute URI with scheme "host" and no host.
            // Example: "example.com:443" => scheme=example.com, host="".
            // Treat this as host[:port] style, not a real URI.
            var hasSchemeSeparator = raw.Contains("://", StringComparison.Ordinal);
            if (!hasSchemeSeparator && string.IsNullOrWhiteSpace(absolute.Host))
            {
                return BuildBaseUri("https://" + raw);
            }

            if (absolute.Scheme is not ("http" or "https"))
            {
                // If someone passes redis://... treat it as https://... for REST purposes.
                if (absolute.Scheme is "redis" or "rediss")
                {
                    var builder = new UriBuilder("https", absolute.Host, absolute.Port);
                    return builder.Uri;
                }

                throw new ArgumentException($"Unsupported Upstash REST URL scheme: {absolute.Scheme}");
            }

            return EnsureTrailingSlash(absolute);
        }

        // host[:port] style
        var trimmed = raw.Trim();
        if (trimmed.StartsWith("redis://", StringComparison.OrdinalIgnoreCase))
        {
            trimmed = trimmed["redis://".Length..];
        }

        if (trimmed.StartsWith("rediss://", StringComparison.OrdinalIgnoreCase))
        {
            trimmed = trimmed["rediss://".Length..];
        }

        var candidate = $"https://{trimmed}";
        if (!Uri.TryCreate(candidate, UriKind.Absolute, out var https))
        {
            throw new ArgumentException("Invalid Upstash REST URL");
        }

        return EnsureTrailingSlash(https);
    }

    private static Uri EnsureTrailingSlash(Uri uri)
    {
        if (uri.AbsoluteUri.EndsWith("/", StringComparison.Ordinal))
        {
            return uri;
        }

        return new Uri(uri.AbsoluteUri + "/");
    }
}
