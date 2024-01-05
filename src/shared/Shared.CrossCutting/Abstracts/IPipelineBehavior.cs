using Shared.Abstracts.Requests;
using Shared.Abstracts.Responses;

namespace Shared.CrossCutting.Abstracts;

/// <summary>
/// A delegate that represents the next action in the pipeline
/// </summary>
/// <typeparam name="TResult">The response of the next action</typeparam>
/// <returns>A <see cref="ValueTask{T}"/> holding a <see cref="Result{Result}"/> of <see cref="Success"/> that represents the result of the next action </returns>
public delegate ValueTask<Result<TResult>> RequestHandlerDelegate<TResult>();

/// <summary>
/// A middleware behavior for a <see cref="IBaseRequest{Result}"/>
/// </summary>
/// <typeparam name="TRequest">The request to intercept</typeparam>
/// <typeparam name="TResult">The expected response</typeparam>
public interface IPipelineBehavior<in TRequest, TResult>
    where TRequest : IBaseRequest<TResult>
{
    /// <summary>
    /// A method that intercepts the pipeline
    /// </summary>
    /// <param name="request">The intercepted request</param>
    /// <param name="next">The next action</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A <see cref="ValueTask{T}"/> holding a <see cref="Result{TResponse}"/> of <see cref="Success"/> that represents the result of the operation </returns>
    ValueTask<Result<TResult>> HandleAsync(TRequest request, RequestHandlerDelegate<TResult> next, CancellationToken cancellationToken);
}
