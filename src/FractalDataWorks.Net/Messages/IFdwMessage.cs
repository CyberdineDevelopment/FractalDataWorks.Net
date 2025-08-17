namespace FractalDataWorks;

/// <summary>
/// Interface for framework messages that provide structured information about operations.
/// </summary>
public interface IFdwMessage
{
    /// <summary>
    /// Gets the severity level of the message.
    /// </summary>
    /// <value>The severity level indicating the importance and impact of the message.</value>
    MessageSeverity Severity { get; }

    /// <summary>
    /// Gets the message text.
    /// </summary>
    /// <value>The human-readable message text describing the condition or status.</value>
    string Message { get; }

    /// <summary>
    /// Gets the message code or identifier.
    /// </summary>
    /// <value>A unique identifier for this type of message, useful for programmatic handling.</value>
    string? Code { get; }

    /// <summary>
    /// Gets the source component or operation that generated the message.
    /// </summary>
    /// <value>The name or identifier of the source that generated this message.</value>
    string? Source { get; }
}