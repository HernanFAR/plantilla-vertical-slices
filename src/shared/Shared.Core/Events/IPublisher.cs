using Shared.Abstracts.Responses;
using Shared.Core.Events.Internals;
using Shared.Core.Events.Strategies;
using System.Collections.Concurrent;

namespace Shared.Core.Events;

/// <summary>
/// Publishes an event through an event pipeline to be handled by many handlers
/// </summary>
public interface IPublisher
{
    /// <summary>
    /// Asynchronously publishes an event to an event pipeline
    /// </summary>
    /// <param name="event">Event</param>
    /// <param name="cancellationToken">CancellationToken</param> 
    /// <returns><see cref="ValueTask"/> representing the action</returns>
    ValueTask PublishAsync(IEvent @event, CancellationToken cancellationToken);

}

/// <summary>
/// Sends a request through the VSlices pipeline to be handled by a many handlers, using reflection
/// </summary>
/// <remarks>
/// Creates a new instance of <see cref="ReflectionPublisher"/>
/// </remarks>
/// <param name="serviceProvider"><see cref="IServiceProvider"/> used to resolve handlers</param>
/// <param name="strategy">Strategy</param>
public class ReflectionPublisher(IServiceProvider serviceProvider, IPublishingStrategy strategy) : IPublisher
{
    internal static readonly ConcurrentDictionary<Type, AbstractHandlerWrapper> RequestHandlers = new();

    readonly IServiceProvider _serviceProvider = serviceProvider;
    readonly IPublishingStrategy _strategy = strategy;


    /// <inheritdoc />
    public async ValueTask PublishAsync(IEvent request, CancellationToken cancellationToken = default)
    {
        var handler = (AbstractHandlerWrapper<Success>)RequestHandlers.GetOrAdd(
            request.GetType(),
            requestType =>
            {
                var wrapperType = typeof(RequestHandlerWrapper<,>).MakeGenericType(requestType, typeof(Success));
                var wrapper = Activator.CreateInstance(wrapperType, _strategy)
                              ?? throw new InvalidOperationException($"Could not create wrapper type for {requestType}");
                return (AbstractHandlerWrapper)wrapper;
            });

        await handler.HandleAsync(request, _serviceProvider, cancellationToken);
    }
}
