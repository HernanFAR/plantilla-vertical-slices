using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Configuration;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

internal static class CorsConfiguration
{
    public static IServiceCollection AddAppCors(this IServiceCollection services,
        IConfiguration conf)
        => services.AddCors(b => DefaultPolicy(b, conf));

    public static void DefaultPolicy(CorsOptions opts, IConfiguration conf)
    {
        opts.AddDefaultPolicy(builder =>
        {
            var origins = conf.GetValue<string>("Cors:Origins");

            if (origins is null) throw new ArgumentNullException(nameof(origins));

            builder.WithOrigins(origins.Split(";"))
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
    }
}
