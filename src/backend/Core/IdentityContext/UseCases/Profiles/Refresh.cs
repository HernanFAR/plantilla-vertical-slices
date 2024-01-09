using CrossCutting.DataAccess.Identity;
using CrossCutting.Security.Authentication;
using CrossCutting.Security.Authentication.Options;
using CrossCutting.Security.Authentication.Services;
using CrossCutting.Security.Authorization.Policies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Options;

// ReSharper disable once CheckNamespace
namespace Core.IdentityContext.UseCases.Profiles.Refresh;

public record RefreshResponse(string Jwt);

public class Endpoint : IEndpointDefinition
{
    public static void DefineDependencies(IServiceCollection services)
    {
        services.AddEndpointDefinition<Endpoint>()
            .AddHandler<Handler>();
    }

    public void Define(IEndpointRouteBuilder builder)
    {
        builder.MapPost("api/v1/profile/refresh", HandleAsync)
            .RequireCors()
            .RequireAuthorization(nameof(AppPolicies.RefreshPolicy))
            .WithName("Refresh")
            .WithSummary("Crea un token de identificación, usando el token de refresco")
            .WithTags("IdentityContext", "Profile")
            .Produces<RefreshResponse>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    public static async ValueTask<IResult> HandleAsync(
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.SendAsync(Command.Instance, cancellationToken);

        return result.MatchEndpointResult(TypedResults.Ok);
    }
}

public record Command : IRequest<RefreshResponse>
{
    public static readonly Command Instance = new();
}

public class Handler(JwtTokenGenerator jwtTokenGenerator,
    AppSignInManager signInManager,
    IOptions<IdentityBearerOptions> bearerOptions,
    UserIdentifiedAccessor userIdentifiedAccessor,
    SystemClock systemClock)
    : IHandler<Command, RefreshResponse>
{
    private readonly JwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;
    private readonly AppSignInManager _signInManager = signInManager;
    private readonly IOptions<IdentityBearerOptions> _bearerOptions = bearerOptions;
    private readonly SystemClock _systemClock = systemClock;
    private readonly UserIdentifiedAccessor _userIdentifiedAccessor = userIdentifiedAccessor;

    public async ValueTask<Result<RefreshResponse>> HandleAsync(Command request, CancellationToken cancellationToken)
    {
        var user = await _signInManager.UserManager
            .FindByIdAsync(_userIdentifiedAccessor.GetUserId());

        if (user == null)
        {
            return new BusinessFailure(FailureKind.ResourceNotFound,
                Title: "Tu usuario no existe en nuestros registros");
        }

        if (user.LastSessionStarted is null || user.LastJwtCreated is null)
        {
            return new BusinessFailure(FailureKind.UserNotAllowed,
                               Title: "Tu usuario no ha iniciado sesión");
        }

        var refreshTokenCreated = _userIdentifiedAccessor.GetIssuedTime() 
                                  ?? throw new InvalidOperationException("Refresh token not valid");

        if (user.LastSessionStarted.Value.Subtract(refreshTokenCreated).TotalSeconds > 5)
        {
            return new BusinessFailure(FailureKind.UserNotAllowed,
                Title: "Se ha iniciado sesión nuevamente, este token de refresco ya no es válido");
        }

        var lastJwtCreatedExpiration = user.LastJwtCreated + _bearerOptions.Value.Duration;

        if (lastJwtCreatedExpiration >= _systemClock.UtcNow)
        {
            return new BusinessFailure(FailureKind.UserNotAllowed,
                Title: "Tu token de identificación aún es válido");
        }

        var jwtToken = await _jwtTokenGenerator.GenerateAsync(user, cancellationToken);

        user.LastJwtCreated = _systemClock.UtcNow;

        var result = await _signInManager.UserManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            return new BusinessFailure(FailureKind.ValidationError,
                Errors: result.Errors
                .Select(e => new ValidationError(e.Code, e.Description))
                .ToArray());
        }

        return new RefreshResponse(jwtToken.Value);
    }
}