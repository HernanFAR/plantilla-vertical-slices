using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class CrossCuttingDependencies
{
    public static IServiceCollection AddCrossCuttingDependencies(this IServiceCollection services)
    {
        return services;
    }
}
