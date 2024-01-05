using Shared.Abstracts.Responses;
using Shared.Core.Requests;
using Shared.Core.Sender.Internals;
using System.Collections.Concurrent;

namespace Shared.Core.Sender;

/// <summary>
/// Sends a request to be handled by a single handler
/// </summary>
public interface ISender
{
    /// <summary>
    /// Asynchronously sends a request to a handler
    /// </summary>
    /// <typeparam name="TResult">Expected response type</typeparam>
    /// <param name="request">Request to be handled</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>
    /// A <see cref="ValueTask{T}"/> holding a <see cref="Result{Result}"/> of <see cref="Success"/> that represents
    /// the result of the operation
    /// </returns>
    ValueTask<Result<TResult>> SendAsync<TResult>(IRequest<TResult> request, CancellationToken cancellationToken);

}

/// <summary>
/// Sends a request through the VSlices pipeline to be handled by a single handler, using reflection
/// </summary>
/// <remarks>
/// Creates a new instance of <see cref="ReflectionSender"/>
/// </remarks>
/// <param name="serviceProvider"><see cref="IServiceProvider"/> used to resolve handlers</param>
public class ReflectionSender(IServiceProvider serviceProvider) : ISender
{
    static readonly ConcurrentDictionary<Type, AbstractHandlerWrapper> RequestHandlers = new();

    readonly IServiceProvider _serviceProvider = serviceProvider;

    /// <inheritdoc />
    public async ValueTask<Result<TResult>> SendAsync<TResult>(IRequest<TResult> request, CancellationToken cancellationToken = default)
    {
        var handler = (AbstractHandlerWrapper<TResult>)RequestHandlers.GetOrAdd(request.GetType(), static requestType =>
        {
            var wrapperType = typeof(RequestHandlerWrapper<,>).MakeGenericType(requestType, typeof(TResult));
            var wrapper = Activator.CreateInstance(wrapperType) ?? throw new InvalidOperationException($"Could not create wrapper type for {requestType}");
            return (AbstractHandlerWrapper)wrapper;
        });

        return await handler.HandleAsync(request, _serviceProvider, cancellationToken);
    }
}
