using Shared.Core.Handlers;
using Shared.CrossCutting.Abstracts;

namespace Shared.Core.Events.Strategies;

/// <summary>
/// A publishing strategy that awaits all handlers in parallel.
/// </summary>
public sealed class AwaitInParallelStrategy : IPublishingStrategy
{
    /// <summary>
    /// Handles the given handlers in parallel using <see cref="Task.WhenAll{Result}(IEnumerable{Task{TResult}})"/>.
    /// </summary>
    /// <typeparam name="TResponse">Expected response</typeparam>
    /// <param name="handlerDelegates">Request Handlers</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async ValueTask HandleAsync<TResponse>(RequestHandlerDelegate<TResponse>[] handlerDelegates)
    {
        await Task.WhenAll(handlerDelegates.Select(async handlerDelegate => await handlerDelegate()));
    }
}