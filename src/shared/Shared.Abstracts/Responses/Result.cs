namespace Shared.Abstracts.Responses;

/// <summary>
/// Represents a result from a process.
/// </summary>
/// <typeparam name="TResult">The expected returned data in success case</typeparam>
public readonly struct Result<TResult>
{
    private readonly BusinessFailure? _businessFailure;
    private readonly TResult? _successValue;

    /// <summary>
    /// Indicates if process was successful
    /// </summary>
    public bool IsSuccess => _businessFailure is null;

    /// <summary>
    /// Indicates if process failed
    /// </summary>
    public bool IsFailure => _businessFailure is not null;

    /// <summary>
    /// The success response of the process, throws <see cref="InvalidOperationException"/> if accessed on failure
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public TResult Value => _successValue ?? throw new InvalidOperationException(nameof(_successValue));

    /// <summary>
    /// The failure response of the process, throws <see cref="InvalidOperationException"/> if accessed on success
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public BusinessFailure BusinessFailure => _businessFailure ?? throw new InvalidOperationException(nameof(_businessFailure));

    /// <summary>
    /// Creates a new instance of <see cref="Result{TResponse}"/> with a success value
    /// </summary>
    /// <param name="successValue">The success value of the process</param>
    public Result(TResult successValue)
    {
        _successValue = successValue;
    }

    /// <summary>
    /// Creates a new instance of <see cref="Result{TResponse}"/> with a failure value
    /// </summary>
    /// <param name="businessFailure">A struct with the failure detail</param>
    public Result(BusinessFailure businessFailure)
    {
        _businessFailure = businessFailure;
    }

    public static implicit operator Result<TResult>(BusinessFailure businessFailure) => new(businessFailure);

    public static implicit operator Result<TResult>(TResult businessFailure) => new(businessFailure);

    public static implicit operator Task<Result<TResult>>(Result<TResult> result) => Task.FromResult(result);

    public static implicit operator ValueTask<Result<TResult>>(Result<TResult> result) => ValueTask.FromResult(result);
}
