using Shared.Core.Handlers;

namespace Shared.Core.Events.Strategies;

/// <summary>
/// Defines a publishing strategy for the <see cref="IPublisher"/>.
/// </summary>
public interface IPublishingStrategy
{
    /// <summary>
    /// Handles the execution of the <see cref="IHandler{TRequest}"/>'s related to the event
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="handlerDelegates"></param>
    /// <returns></returns>
    ValueTask HandleAsync<TResponse>(RequestHandlerDelegate<TResponse>[] handlerDelegates);
}
