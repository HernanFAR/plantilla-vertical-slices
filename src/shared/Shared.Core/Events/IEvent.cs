using Shared.Abstracts.Requests;
using Shared.Abstracts.Responses;

namespace Shared.Core.Events;

/// <summary>
/// Represents the start point of a side effect of a use case
/// </summary>
public interface IEvent : IBaseRequest<Success>
{
    /// <summary>
    /// The unique identifier of this event
    /// </summary>
    Guid Id { get; }
}

/// <summary>
/// Abstract base class for all events
/// </summary>
public abstract record EventBase : IEvent
{
    /// <inheritdoc />
    public virtual Guid Id { get; } = Guid.NewGuid();
}
