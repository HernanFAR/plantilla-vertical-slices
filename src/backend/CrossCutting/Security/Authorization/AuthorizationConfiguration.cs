using CrossCutting.Security.Authentication;
using CrossCutting.Security.Authorization;
using CrossCutting.Security.Authorization.Policies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

internal static class AuthorizationConfiguration
{
    public static IServiceCollection AddAppAuthorization(this IServiceCollection services)
    {
        return services.AddAuthorization(GetIdentityBearerOptions);
    }

    private static void GetIdentityBearerOptions(AuthorizationOptions opts)
    {
        opts.DefaultPolicy = AppPolicies.DefaultPolicy;

        opts.AddPolicy(nameof(AppPolicies.RefreshPolicy), AppPolicies.RefreshPolicy);
    }
}
