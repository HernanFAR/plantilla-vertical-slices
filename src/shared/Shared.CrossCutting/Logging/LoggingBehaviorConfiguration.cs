using System.Text.Json;

namespace Shared.CrossCutting.Logging;

/// <summary>
/// Configuration for the <see cref="LoggingBehavior{TRequest,TResult}"/>
/// </summary>
public class LoggingBehaviorConfiguration
{
    /// <summary>
    /// Describer to use for the logging messages
    /// </summary>
    public ILoggingDescriber Describer { get; set; } = new DefaultLoggingDescriber();

    /// <summary>
    /// Options to use for the serialization of the logging pipeline
    /// </summary>
    public JsonSerializerOptions? JsonOptions { get; set; }

}
