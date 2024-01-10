using CrossCutting.DataAccess.Identity;
using CrossCutting.Security.Authentication.Services;

// ReSharper disable once CheckNamespace
namespace Core.IdentityContext.UseCases.Profiles.LogIn;

public record LogInContract(string Email, string Password);

public record LogInResponse(string Jwt, string RefreshToken);

public class Endpoint : IEndpointDefinition
{
    public static void DefineDependencies(IServiceCollection services)
    {
        services.AddEndpointDefinition<Endpoint>()
            .AddHandler<Handler>();
    }

    public void Define(IEndpointRouteBuilder builder)
    {
        builder.MapPost("api/v1/profile/log-in", HandleAsync)
            .RequireCors()
            .WithName("LogIn")
            .WithSummary("Crea un token de identificación y un token de refresco")
            .WithTags("IdentityContext", "Profile")
            .Accepts<LogInContract>(MediaTypeNames.Application.Json)
            .Produces<LogInResponse>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden);
    }

    public static async ValueTask<IResult> HandleAsync(
        [FromServices] ISender sender,
        [FromBody] LogInContract contract,
        CancellationToken cancellationToken)
    {
        var command = new Command(contract.Email, contract.Password);

        var result = await sender.SendAsync(command, cancellationToken);

        return result.MatchEndpointResult(TypedResults.Ok);
    }
}

public record Command(string Email, string Password) : IRequest<LogInResponse>;

public class Handler(AppSignInManager signInManager,
    JwtTokenGenerator jwtTokenGenerator,
    RefreshTokenGenerator refreshTokenGenerator,
    SystemClock systemClock)
    : IHandler<Command, LogInResponse>
{
    private readonly AppSignInManager _signInManager = signInManager;
    private readonly JwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;
    private readonly RefreshTokenGenerator _refreshTokenGenerator = refreshTokenGenerator;
    private readonly SystemClock _systemClock = systemClock;

    public async ValueTask<Result<LogInResponse>> HandleAsync(Command request, CancellationToken cancellationToken)
    {
        var user = await _signInManager.UserManager.FindByEmailAsync(request.Email);

        if (user == null)
        {
            return new BusinessFailure(FailureKind.ResourceNotFound,
                Title: "Tu usuario no existe en nuestros registros");
        }

        var signInResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, true);

        if (signInResult.IsLockedOut)
        {
            return new BusinessFailure(FailureKind.UserNotAllowed,
                Title: "No ha sido posible iniciar sesión",
                Detail: "Tienes tu cuenta bloqueada por muchos intentos de inicio de sesión incorrectos");
        }

        if (signInResult.IsNotAllowed)
        {
            return new BusinessFailure(FailureKind.UserNotAllowed,
                    Title: "No ha sido posible iniciar sesión",
                    Detail: "Recuerda confirmar tu correo o tu teléfono");
        }

        if (!signInResult.Succeeded)
        {
            return new BusinessFailure(FailureKind.UserNotAuthenticated,
                    Title: "No ha sido posible iniciar sesión",
                    Detail: "Las credenciales son erróneas");
        }

        var jwtToken = await _jwtTokenGenerator.GenerateAsync(user, cancellationToken);
        var refreshToken = await _refreshTokenGenerator.GenerateAsync(user, cancellationToken);

        user.LastJwtCreated = _systemClock.UtcNow;
        user.LastSessionStarted = _systemClock.UtcNow;

        var result = await _signInManager.UserManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            return new BusinessFailure(FailureKind.ValidationError,
                Errors: result.Errors
                    .Select(e => new ValidationError(e.Code, e.Description))
                    .ToArray());
        }

        return new LogInResponse(jwtToken.Value, refreshToken.Value);
    }
}