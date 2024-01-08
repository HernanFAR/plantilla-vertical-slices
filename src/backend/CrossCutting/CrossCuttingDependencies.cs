using CrossCutting;
using CrossCutting.DataAccess.EntityFramework;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Internal;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class CrossCuttingDependencies
{
    public static IServiceCollection AddCrossCuttingDependencies(this IServiceCollection services,
        IConfiguration conf, IHostEnvironment env)
        => services
            .AddDataAccessDependencies()
            .AddSecurityDependencies(conf)
            .AddMiscellaneousDependencies(conf, env)
            .AddWebApiDependencies()
            .AddPipelineBehaviors();

    public static IServiceCollection AddPipelineBehaviors(this IServiceCollection services)
        => services
            .AddExceptionHandlingBehavior()
            .AddLoggingBehavior()
            .AddValidationBehavior();

}
