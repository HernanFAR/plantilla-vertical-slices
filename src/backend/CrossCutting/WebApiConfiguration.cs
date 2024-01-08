using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CrossCutting;

internal static class WebApiConfiguration
{
    public static IServiceCollection AddWebApiDependencies(this IServiceCollection services) 
        => services
            .AddEndpointsApiExplorer()
            .AddSwaggerGen(ConfigureSwagger);

    public static void ConfigureSwagger(SwaggerGenOptions opts)
    {
        opts.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Rubrica PUC",
            Version = "v1.0.0",
            Description = "Web API restfull para uso de la Pontificia Universidad Católica."
        });

        opts.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "JWT Authorization header using the Bearer scheme.",
        });

        opts.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Id = "Bearer",
                        Type = ReferenceType.SecurityScheme
                    }
                },
                []
            }
        });
    }
}
