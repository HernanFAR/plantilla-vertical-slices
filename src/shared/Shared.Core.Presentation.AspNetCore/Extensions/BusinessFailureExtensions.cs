using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// ReSharper disable once CheckNamespace
namespace Shared.Abstracts.Responses;

public static class BusinessFailureExtensions
{
    public static ProblemDetails ToProblemDetails(this BusinessFailure businessFailure)
    {
        var statusCode = businessFailure.Kind switch
        {
            FailureKind.Unspecified => StatusCodes.Status400BadRequest,
            FailureKind.UserNotAuthenticated => StatusCodes.Status401Unauthorized,
            FailureKind.UserNotAllowed => StatusCodes.Status403Forbidden,
            FailureKind.ResourceNotFound => StatusCodes.Status404NotFound,
            FailureKind.ConcurrencyError => StatusCodes.Status409Conflict,
            FailureKind.ValidationError => StatusCodes.Status422UnprocessableEntity,
            FailureKind.UnhandledException => StatusCodes.Status500InternalServerError,
            _ => throw new ArgumentOutOfRangeException(nameof(businessFailure))
        };

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Detail = businessFailure.Detail,
            Title = businessFailure.Title,
            Instance = businessFailure.Instance,
            Type = businessFailure.Type
        };

        var extensions = businessFailure.CustomExtensions ?? problemDetails.Extensions;

        if (businessFailure.Errors is null)
        {
            return problemDetails;
        }

        extensions["errors"] = businessFailure.Errors
            .Select(x => x.Name)
            .Distinct()
            .ToDictionary(
                propertyName => propertyName,
                propertyName => businessFailure.Errors
                    .Where(x => x.Name == propertyName)
                    .Select(e => e.Detail)
                    .ToArray()); ;

        return problemDetails;
    }
}
