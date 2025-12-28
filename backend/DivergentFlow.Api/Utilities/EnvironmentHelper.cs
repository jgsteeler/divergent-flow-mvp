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
    /// Parses a semicolon or comma-separated list of origins from an environment variable value.
    /// </summary>
    /// <param name="raw">The raw environment variable value containing origins.</param>
    /// <returns>An array of origin strings, or an empty array if the input is null or whitespace.</returns>
    public static string[] ParseOrigins(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return Array.Empty<string>();
        }

        return raw
            .Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToArray();
    }
}
