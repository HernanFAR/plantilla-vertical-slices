using CrossCutting.Security.Authentication;
using CrossCutting.Security.Authentication.Options;
using CrossCutting.Security.Authentication.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

internal static class AuthenticationConfiguration
{
    public static IServiceCollection AddAppAuthentication(this IServiceCollection services,
        IConfiguration conf)
    {
        using var scope = services.BuildServiceProvider().CreateScope();
        var provider = scope.ServiceProvider;

        return services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(AuthenticationForm.Default, opts => GetIdentityBearerOptions(opts, provider))
            .AddJwtBearer(AuthenticationForm.Refresh, opts => GetRefreshBearerOptions(opts, provider))
            .Services
            .Configure<IdentityBearerOptions>(conf.GetSection(IdentityBearerOptions.ConfigName))
            .Configure<RefreshBearerOptions>(conf.GetSection(RefreshBearerOptions.ConfigName))
            .AddScoped<JwtTokenGenerator>()
            .AddScoped<RefreshTokenGenerator>()
            .AddScoped<UserIdentifiedAccessor>();
    }

    private static void GetIdentityBearerOptions(JwtBearerOptions opts, IServiceProvider provider)
    {
        var bearerOptions = provider.GetRequiredService<IOptions<IdentityBearerOptions>>();

        opts.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = bearerOptions.Value.Issuer,
            ValidAudience = bearerOptions.Value.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(bearerOptions.Value.KeyBytes),
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    }

    private static void GetRefreshBearerOptions(JwtBearerOptions opts, IServiceProvider provider)
    {
        var bearerOptions = provider.GetRequiredService<IOptions<RefreshBearerOptions>>();

        opts.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = bearerOptions.Value.Issuer,
            ValidAudience = bearerOptions.Value.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(bearerOptions.Value.KeyBytes),
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    }
}
