using CrossCutting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Internal;
using Serilog;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

internal static class MiscellaneousConfiguration
{
    public static IServiceCollection AddMiscellaneousDependencies(this IServiceCollection services,
        IConfiguration conf, IHostEnvironment env)
        => services
            .AddAppLogging(conf, env)
            .AddSingleton<SystemClock>();

    public static IServiceCollection AddAppLogging(this IServiceCollection services,
        IConfiguration config, IHostEnvironment env)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(config)
            .Enrich.WithProperty("Environment", env.EnvironmentName)
            .Enrich.WithProperty("Application", "RubricaPUC")
            .CreateLogger();

        return services;
    }
}
