using CrossCutting.DataAccess.EntityFramework.IdentityContext.Models;
using CrossCutting.DataAccess.Identity;

// ReSharper disable once CheckNamespace
namespace Core.IdentityContext.UseCases.Users.Register;

public record RegisterContract(
    string Name,
    string FatherName,
    string MotherName,
    string Rut,
    string Email,
    string? PhoneNumber,
    string Password);

public class Endpoint : IEndpointDefinition
{
    public static void DefineDependencies(IServiceCollection services)
    {
        services.AddEndpointDefinition<Endpoint>()
            .AddHandler<Handler>();
    }

    public void Define(IEndpointRouteBuilder builder)
    {
        builder.MapPost("api/v1/user", HandleAsync)
            .RequireCors()
            .WithName("Register")
            .WithSummary("Crea un usuario en el sistema")
            .WithTags("IdentityContext", "User")
            .Produces(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity);
    }

    public static async ValueTask<IResult> HandleAsync(
        [FromServices] ISender sender,
        [FromBody] RegisterContract contract,
        CancellationToken cancellationToken)
    {
        var command = new Command(
            contract.Name,
            contract.FatherName,
            contract.MotherName,
            contract.Rut,
            contract.Email,
            contract.PhoneNumber,
            contract.Password);

        var result = await sender.SendAsync(command, cancellationToken);

        return result.MatchEndpointResult(_ => TypedResults.Created());
    }
}

public record Command(
    string Name,
    string FatherName,
    string MotherName,
    string Rut,
    string Email,
    string? PhoneNumber,
    string Password)
    : IRequest;

public class Handler(AppUserManager userManager)
    : IHandler<Command>
{
    private readonly AppUserManager _userManager = userManager;

    public async ValueTask<Result<Success>> HandleAsync(Command request, CancellationToken cancellationToken = default)
    {
        var user = new AppUser
        {
            Name = request.Name,
            FatherLastName = request.FatherName,
            MotherLastName = request.MotherName,
            Rut = request.Rut,
            PhoneNumber = request.PhoneNumber,
            Email = request.Email
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (result.Succeeded) return Success.Value;

        return new BusinessFailure(FailureKind.ValidationError,
            Errors: result.Errors
                .Select(e => new ValidationError(e.Code, e.Description))
                .ToArray());
    }
}
