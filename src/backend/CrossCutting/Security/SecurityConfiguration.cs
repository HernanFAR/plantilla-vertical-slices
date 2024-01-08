using Microsoft.Extensions.Configuration;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

internal static class SecurityConfiguration
{
    public static IServiceCollection AddSecurityDependencies(this IServiceCollection services,
        IConfiguration conf)
        => services
            .AddAppCors(conf)
            .AddAppAuthentication(conf)
            .AddAppAuthorization();
}
