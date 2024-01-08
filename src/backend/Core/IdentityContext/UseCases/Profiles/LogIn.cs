using CrossCutting.DataAccess.Identity;
using CrossCutting.Security.Authentication.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Internal;
using Microsoft.OpenApi.Models;
using System.Net.Mime;

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
            .WithOpenApi(op =>
            {
                op.Tags = [new OpenApiTag
                {
                    Name = "IdentityContext"
                }];

                op.OperationId = "LogIn";
                op.Description = "Crea un token de identificación y un token de refresco";

                op.RequestBody = new OpenApiRequestBody
                {
                    Required = true,
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        [MediaTypeNames.Application.Json] = new OpenApiMediaType
                        {
                            Schema = new OpenApiSchema
                            {
                                Type = "object",
                                Properties = new Dictionary<string, OpenApiSchema>
                                {
                                    ["email"] = new OpenApiSchema
                                    {
                                        Type = "string",
                                        Description = "Correo electrónico del usuario"
                                    },
                                    ["password"] = new OpenApiSchema
                                    {
                                        Type = "string",
                                        Description = "Contraseña del usuario"
                                    }
                                }
                            }
                        }
                    }
                };

                op.Responses.Add("Success",
                    new OpenApiResponse
                    {
                        Content = new Dictionary<string, OpenApiMediaType>
                        {
                            [MediaTypeNames.Application.Json] = new OpenApiMediaType
                            {
                                Schema = new OpenApiSchema
                                {
                                    Type = "object",
                                    Properties = new Dictionary<string, OpenApiSchema>
                                    {
                                        ["jwt"] = new OpenApiSchema
                                        {
                                            Type = "string",
                                            Description = "Token de identificación"
                                        },
                                        ["refreshToken"] = new OpenApiSchema
                                        {
                                            Type = "string",
                                            Description = "Token de refresco"
                                        }
                                    }
                                }
                            }
                        },
                    });

                op.Responses.Add("Unauthorized",
                    new OpenApiResponse
                    {
                        Description = "No se pudo iniciar sesión",
                        Content = new Dictionary<string, OpenApiMediaType>
                        {
                            [MediaTypeNames.Application.Json] = new OpenApiMediaType
                            {
                                Schema = new OpenApiSchema
                                {
                                    Type = "object",
                                    Properties = new Dictionary<string, OpenApiSchema>
                                    {
                                        ["type"] = new OpenApiSchema
                                        {
                                            Type = "string",
                                            Description = "Tipo de error"
                                        },
                                        ["title"] = new OpenApiSchema
                                        {
                                            Type = "string",
                                            Description = "Título del error"
                                        },
                                        ["detail"] = new OpenApiSchema
                                        {
                                            Type = "string",
                                            Description = "Detalle del error"
                                        },
                                        ["errors"] = new OpenApiSchema
                                        {
                                            Type = "array",
                                            Description = "Errores de validación",
                                            Items = new OpenApiSchema
                                            {
                                                Type = "object",
                                                Properties = new Dictionary<string, OpenApiSchema>
                                                {
                                                    ["code"] = new OpenApiSchema
                                                    {
                                                        Type = "string",
                                                        Description = "Código del error"
                                                    },
                                                    ["description"] = new OpenApiSchema
                                                    {
                                                        Type = "string",
                                                        Description = "Descripción del error"
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    });

                op.Responses.Add("Forbidden",
                    new OpenApiResponse
                    {
                        Description = "No se pudo iniciar sesión",
                        Content = new Dictionary<string, OpenApiMediaType>
                        {
                            [MediaTypeNames.Application.Json] = new OpenApiMediaType
                            {
                                Schema = new OpenApiSchema
                                {
                                    Type = "object",
                                    Properties = new Dictionary<string, OpenApiSchema>
                                    {
                                        ["type"] = new OpenApiSchema
                                        {
                                            Type = "string",
                                            Description = "Tipo de error"
                                        },
                                        ["title"] = new OpenApiSchema
                                        {
                                            Type = "string",
                                            Description = "Título del error"
                                        },
                                        ["detail"] = new OpenApiSchema
                                        {
                                            Type = "string",
                                            Description = "Detalle del error"
                                        }
                                    }
                                }
                            }
                        }
                    });

                return op;
            });
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

public class Handler(ApplicationSignInManager signInManager,
    JwtTokenGenerator jwtTokenGenerator,
    RefreshTokenGenerator refreshTokenGenerator,
    SystemClock systemClock) : IHandler<Command, LogInResponse>
{
    private readonly ApplicationSignInManager _signInManager = signInManager;
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