namespace DivergentFlow.Api.Utilities;

/// <summary>
/// Helper methods for reading and parsing environment variables.
/// </summary>
public static class EnvironmentHelper
{
    /// <summary>
    /// Reads a boolean environment variable with a default fallback value.
    /// </summary>
    /// <param name="key">The environment variable key.</param>
    /// <param name="defaultValue">The default value to return if the variable is not set or cannot be parsed.</param>
    /// <returns>The parsed boolean value or the default.</returns>
    public static bool GetBoolEnv(string key, bool defaultValue)
    {
        var value = Environment.GetEnvironmentVariable(key);
        return bool.TryParse(value, out var parsed) ? parsed : defaultValue;
    }

    /// <summary>
    /// Parses a comma-separated list of allowed origins from an environment variable value.
    /// Entries are normalized to the canonical Origin form: scheme://host[:port].
    /// Invalid or non-http(s) entries are ignored.
    /// </summary>
    /// <param name="raw">The raw environment variable value containing origins.</param>
    /// <returns>An array of normalized origin strings, or an empty array if the input is null or whitespace.</returns>
    public static string[] ParseOrigins(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return Array.Empty<string>();
        }

        return raw
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(static entry => NormalizeOrigin(entry))
            .Where(static origin => origin is not null)
            .Select(static origin => origin!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static string? NormalizeOrigin(string rawOrigin)
    {
        if (!Uri.TryCreate(rawOrigin, UriKind.Absolute, out var uri))
        {
            return null;
        }

        if (uri.Scheme is not ("http" or "https"))
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(uri.Host))
        {
            return null;
        }

        // Normalize to origin (no path/query/fragment). Also strips trailing slash.
        return uri.IsDefaultPort
            ? $"{uri.Scheme}://{uri.Host}"
            : $"{uri.Scheme}://{uri.Host}:{uri.Port}";
    }
}
