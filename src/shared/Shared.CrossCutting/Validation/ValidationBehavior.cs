using FluentValidation;
using Shared.Abstracts.Requests;
using Shared.Abstracts.Responses;
using Shared.CrossCutting.Abstracts;

namespace Shared.CrossCutting.Validation;

/// <summary>
/// A validation behavior that uses FluentValidation
/// </summary>
/// <typeparam name="TRequest">The intercepted request to validate</typeparam>
/// <typeparam name="TResult">The expected successful response</typeparam>
public abstract class ValidationBehavior<TRequest, TResult>(IEnumerable<IValidator<TRequest>> validator) 
    : IPipelineBehavior<TRequest, TResult>
    where TRequest : IBaseRequest<TResult>
{
    readonly IValidator<TRequest>? _validator = validator.FirstOrDefault();

    /// <inheritdoc/>
    public async ValueTask<Result<TResult>> HandleAsync(TRequest request, RequestHandlerDelegate<TResult> next, CancellationToken cancellationToken = default)
    {
        var validationResult = await ValidateAsync(request, cancellationToken);

        if (validationResult.IsFailure)
        {
            return validationResult.BusinessFailure;
        }

        return await next();
    }
    /// <summary>
    /// Asynchronously validates the request
    /// </summary>
    /// <param name="request">The request to validate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A <see cref="ValueTask{T}"/> holding a <see cref="Result{Result}"/> of <see cref="Success"/> that represents the result of the operation </returns>
    protected async ValueTask<Result<Success>> ValidateAsync(TRequest request, CancellationToken cancellationToken)
    {
        if (_validator is null)
        {
            return Success.Value;
        }

        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (validationResult.IsValid) return Success.Value;

        var errors = validationResult.Errors
            .Select(e => new ValidationError(e.PropertyName, e.ErrorMessage))
            .ToArray();

        return new BusinessFailure(FailureKind.ValidationError, 
            "Error de validación", 
            "Se han encontrado errores en el objeto enviado", 
            request, errors);
    }
}
