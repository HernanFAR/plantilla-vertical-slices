namespace Shared.CrossCutting.Logging;

/// <summary>
/// Interface to describe the logging messages
/// </summary>
public interface ILoggingDescriber
{
    /// <summary>
    /// Message to log when the request is starting
    /// </summary>
    string Initial { get; }

    /// <summary>
    /// Message to log when the request is finishing with success
    /// </summary>
    string Success { get; }

    /// <summary>
    /// Message to log when the request is finishing with failure
    /// </summary>
    string Failure { get; }
}

/// <inheritdoc/>
public sealed class DefaultLoggingDescriber : ILoggingDescriber
{
    /// <inheritdoc/>
    public string Initial => "Log hour: {0} | Starting handling of {1}, with the following properties: {2}.";

    /// <inheritdoc/>
    public string Success => "Log hour: {0} | Finishing handling of {1}, response obtained correctly: {2}.";

    /// <inheritdoc/>
    public string Failure => "Log hour: {0} | Finishing handling of {1}, response obtained with errors: {2}.";

}
