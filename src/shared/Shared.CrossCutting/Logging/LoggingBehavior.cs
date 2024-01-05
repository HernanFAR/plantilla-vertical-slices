using Microsoft.Extensions.Logging;
using Shared.Abstracts.Requests;
using Shared.Abstracts.Responses;
using Shared.CrossCutting.Abstracts;
using System.Text.Json;

namespace Shared.CrossCutting.Logging;

/// <summary>
/// Base logging behavior
/// </summary>
/// <remarks>Logs at start, successful end and failed end</remarks>
/// <typeparam name="TRequest">The intercepted request to log about</typeparam>
/// <typeparam name="TResult">The expected successful response</typeparam>
/// <remarks>
/// Creates a new instance using the provided <see cref="ILogger{TRequest}"/> and <see cref="LoggingBehaviorConfiguration"/>
/// </remarks>
/// <param name="logger">Logger</param>
/// <param name="behaviorConfiguration">Configuration to use</param>
public class LoggingBehavior<TRequest, TResult>(
    ILogger<TRequest> logger, LoggingBehaviorConfiguration behaviorConfiguration) 
    : IPipelineBehavior<TRequest, TResult>
    where TRequest : IBaseRequest<TResult>
{
    readonly ILogger<TRequest> _logger = logger;
    readonly LoggingBehaviorConfiguration _behaviorConfiguration = behaviorConfiguration;

    /// <inheritdoc/>
    public virtual async ValueTask<Result<TResult>> HandleAsync(TRequest request, 
        RequestHandlerDelegate<TResult> next, CancellationToken cancellationToken = default)
    {
        InitialHandling(request);

        var result = await next();

        Action<Result<TResult>, TRequest> handler = result.IsSuccess 
            ? SuccessHandling : FailureHandling;

        handler(result, request);

        return result;
    }

    /// <summary>
    /// Logs information about the request, at the start of request 
    /// </summary>
    /// <param name="request">Request to log about</param>
    protected internal virtual void InitialHandling(TRequest request)
    {
        _logger.LogInformation(_behaviorConfiguration.Describer.Initial,
            DateTime.Now, typeof(TRequest).FullName, 
            JsonSerializer.Serialize(request, _behaviorConfiguration.JsonOptions));
    }

    /// <summary>
    /// Logs information about the request, only if it was successful
    /// </summary>
    /// <param name="response">Response to log about</param>
    /// <param name="request">Request to log about</param>
    protected internal virtual void SuccessHandling(Result<TResult> response, TRequest request)
    {
        _logger.LogInformation(_behaviorConfiguration.Describer.Success,
            DateTime.Now, typeof(TRequest).FullName, 
            JsonSerializer.Serialize(response.Value, _behaviorConfiguration.JsonOptions));
        
    }

    /// <summary>
    /// Logs information about the request, only if it failed
    /// </summary>
    /// <param name="response">Response to log about</param>
    /// <param name="request">Request to log about</param>
    protected internal virtual void FailureHandling(Result<TResult> response, TRequest request)
    {
        _logger.LogWarning(_behaviorConfiguration.Describer.Failure,
            DateTime.Now, typeof(TRequest).FullName, 
            JsonSerializer.Serialize(response.BusinessFailure, _behaviorConfiguration.JsonOptions));
    }
}
