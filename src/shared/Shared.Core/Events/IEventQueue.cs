using System.Threading.Channels;

namespace Shared.Core.Events;

/// <summary>
/// Represents a queue writer of events
/// </summary>
public interface IEventQueueWriter
{
    /// <summary>
    /// Asynchronously enqueue a event to the queue
    /// </summary>
    /// <param name="event"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    ValueTask EnqueueAsync(IEvent @event, CancellationToken cancellationToken = default);

}

/// <summary>
/// Represents a queue reader of events
/// </summary>
public interface IEventQueueReader
{
    /// <summary>
    /// Asynchronously dequeue the next event from the queue
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>the dequeued event from the queue</returns>
    ValueTask<IEvent> DequeueAsync(CancellationToken cancellationToken = default);
    
}

/// <summary>
/// Represents a queue of events, with write and read capabilities
/// </summary>
public interface IEventQueue : IEventQueueReader, IEventQueueWriter;

/// <summary>
/// Configuration for <see cref="InMemoryEventQueue"/>
/// </summary>
public class InMemoryEventQueueConfiguration
{
    /// <summary>
    /// Capacity of the queue
    /// </summary>
    /// <remarks>Defaults to 50</remarks>
    public int Capacity { get; set; } = 50;
}

/// <summary>
/// Allows to publish, peek and dequeue events through an in memory channel
/// </summary>
public sealed class InMemoryEventQueue : IEventQueue
{
    readonly Channel<IEvent> _channel;

    /// <summary>
    /// Creates a new instance of <see cref="InMemoryEventQueue"/>
    /// </summary>
    /// <param name="inMemoryPublishConfigOptions"></param>
    public InMemoryEventQueue(InMemoryEventQueueConfiguration inMemoryPublishConfigOptions)
    {
        var options = new BoundedChannelOptions(inMemoryPublishConfigOptions.Capacity)
        {
            FullMode = BoundedChannelFullMode.Wait
        };

        _channel = Channel.CreateBounded<IEvent>(options);
    }

    /// <inheritdoc />
    public async ValueTask<IEvent> DequeueAsync(CancellationToken cancellationToken)
    {
        var workItem = await _channel.Reader.ReadAsync(cancellationToken);

        return workItem;
    }

    /// <inheritdoc />
    public async ValueTask EnqueueAsync(IEvent @event, CancellationToken cancellationToken)
    {
        await _channel.Writer.WriteAsync(@event, cancellationToken);
    }
}
