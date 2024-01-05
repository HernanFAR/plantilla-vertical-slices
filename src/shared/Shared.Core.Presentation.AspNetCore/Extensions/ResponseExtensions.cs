using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// ReSharper disable once CheckNamespace
namespace Shared.Abstracts.Responses;

public static class ResponseExtensions
{
    /// <summary>
    /// Maps a <see cref="Result{TResult}"/> to a <see cref="IResult"/>, using the provided function in success case.
    /// <para>For the errors, returns a <see cref="ProblemDetails"/>, which is an implementation of <see href="https://datatracker.ietf.org/doc/html/rfc7807"/></para>
    /// </summary>
    /// <typeparam name="TSuccess">Return type in success</typeparam>
    /// <param name="result">Result</param>
    /// <param name="successFunc">Function to execute in success case</param>
    /// <returns>The <see cref="IResult"/> of the use case</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static IResult MatchEndpointResult<TSuccess>(this Result<TSuccess> result,
        Func<TSuccess, IResult> successFunc)
    {
        if (result.IsSuccess)
            return successFunc(result.Value);

        var bf = result.BusinessFailure;

        return TypedResults.Problem(bf.ToProblemDetails());
    }
}
