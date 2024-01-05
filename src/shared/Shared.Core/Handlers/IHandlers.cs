using Shared.Abstracts.Requests;
using Shared.Abstracts.Responses;
using Shared.Core.Requests;

namespace Shared.Core.Handlers;

/// <summary>
/// Defines a handler for a <see cref="IBaseRequest{Result}"/>
/// </summary>
/// <remarks>If idempotency is necessary, the handler itself must ensure it</remarks>
/// <typeparam name="TRequest">The request to be handled</typeparam>
/// <typeparam name="TResult">The expected response of the handler</typeparam>
public interface IHandler<in TRequest, TResult>
    where TRequest : IBaseRequest<TResult>
{
    /// <summary>
    /// Handles the request
    /// </summary>
    /// <param name="request">The request to be handled</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A <see cref="ValueTask{T}"/> holding a <see cref="Result{TResponse}"/> of <see cref="Success"/> that represents the result of the operation </returns>
    ValueTask<Result<TResult>> HandleAsync(TRequest request, CancellationToken cancellationToken = default);
}

/// <summary>
/// Defines a handler for a <see cref="IRequest"/>
/// </summary>
/// <typeparam name="TRequest">The request to be handled</typeparam>
public interface IHandler<in TRequest> : IHandler<TRequest, Success>
    where TRequest : IBaseRequest<Success>
{ }
