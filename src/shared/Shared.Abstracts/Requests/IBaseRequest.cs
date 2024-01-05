namespace Shared.Abstracts.Requests;

/// <summary>
/// Represents the start point of any feature in the application, with a <typeparamref name="TResult"/> response
/// </summary>
/// <typeparam name="TResult">Expected result of the feature</typeparam>
public interface IBaseRequest<TResult>;
