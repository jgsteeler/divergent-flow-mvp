namespace DivergentFlow.Application.Abstractions;

/// <summary>
/// Provides access to the current user identity for request-scoped operations.
///
/// Note: The system is currently effectively single-user. This abstraction exists to
/// make user-scoping explicit and enable a clean path to real authentication later.
/// </summary>
public interface IUserContext
{
    /// <summary>
    /// Gets the current user identifier.
    /// </summary>
    string UserId { get; }
}
