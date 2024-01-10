using CrossCutting.DataAccess.Identity;
using CrossCutting.Security.Authentication.Services;

// ReSharper disable once CheckNamespace
namespace Core.IdentityContext.UseCases.Profiles.Get;

public record ProfileResponse(
    string Name,
    string FatherName,
    string MotherName,
    string FullName,
    string Rut,
    string Email,
    bool EmailConfirmed,
    string? PhoneNumber,
    bool PhoneNumberConfirmed);

public class Endpoint : IEndpointDefinition
{
    public static void DefineDependencies(IServiceCollection services)
    {
        services.AddEndpointDefinition<Endpoint>()
            .AddHandler<Handler>();
    }

    public void Define(IEndpointRouteBuilder builder)
    {
        builder.MapGet("api/v1/profile", HandleAsync)
            .RequireCors()
            .RequireAuthorization()
            .WithName("Profile")
            .WithSummary("Retorna la información del usuario en el sistema")
            .WithTags("IdentityContext", "Profile")
            .Produces<ProfileResponse>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
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

public record Command : IRequest<ProfileResponse>
{
    public static readonly Command Instance = new();
}

public class Handler(
    UserIdentifiedAccessor userIdentifiedAccessor,
    AppUserManager userManager)
    : IHandler<Command, ProfileResponse>
{
    private readonly UserIdentifiedAccessor _userIdentifiedAccessor = userIdentifiedAccessor;
    private readonly AppUserManager _userManager = userManager;

    public async ValueTask<Result<ProfileResponse>> HandleAsync(Command request, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(_userIdentifiedAccessor.GetUserId());

        if (user == null)
        {
            return new BusinessFailure(FailureKind.ResourceNotFound);
        }

        return new ProfileResponse(
            user.Name,
            user.FatherLastName,
            user.MotherLastName,
            user.UserName,
            user.Rut,
            user.Email!,
            user.EmailConfirmed,
            user.PhoneNumber,
            user.PhoneNumberConfirmed);
    }
}
