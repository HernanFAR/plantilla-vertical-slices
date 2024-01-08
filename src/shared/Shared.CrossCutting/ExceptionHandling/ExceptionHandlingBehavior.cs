using Shared.Abstracts.Requests;
using Shared.Abstracts.Responses;
using Shared.CrossCutting.Abstracts;

namespace Shared.CrossCutting.ExceptionHandling;

/// <summary>
/// Base exception handling behavior
/// </summary>
/// <typeparam name="TRequest">The intercepted request to handle</typeparam>
/// <typeparam name="TResult">The expected successful response</typeparam>
public sealed class ExceptionHandlingBehavior<TRequest, TResult> : IPipelineBehavior<TRequest, TResult>
    where TRequest : IBaseRequest<TResult>
{
    /// <inheritdoc/>
    public async ValueTask<Result<TResult>> HandleAsync(TRequest request, RequestHandlerDelegate<TResult> next, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            await ProcessExceptionAsync(ex, request);

            return new BusinessFailure(FailureKind.UnhandledException, 
                "Error interno", 
                "Hubo un error interno en el sistema, por favor intenta más tarde");
        }
    }

    /// <summary>
    /// Processes the exception
    /// </summary>
    /// <remarks>You can add more specific logging, email sending, etc. here</remarks>
    /// <param name="ex">The throw exception</param>
    /// <param name="request">The related request information</param>
    /// <returns>A <see cref="ValueTask"/> representing the processing of the exception</returns>
    internal ValueTask ProcessExceptionAsync(Exception ex, TRequest request)
    {
        return ValueTask.CompletedTask;
    }
    
}
