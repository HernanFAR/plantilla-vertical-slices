using Shared.Abstracts.Responses;
using Shared.Abstracts.Requests;

namespace Shared.Core.Requests;

/// <summary>
/// Represents the start point of a general purpose use case, with a <typeparamref name="TResult"/> response
/// </summary>
/// <typeparam name="TResult">The expected response of this request</typeparam>
public interface IRequest<TResult> : IBaseRequest<TResult>;

/// <inheritdoc />
public interface IRequest : IRequest<Success>;

