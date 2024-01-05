using Shared.Core.Presentation;
using Shared.Core.Presentation.AspNetCore;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Scan a specified assembly for <see cref="IEndpointDefinition"/> implementations and adds as <see cref="ISimpleEndpointDefinition"/> (as well of specified dependencies) to the service collection.
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="lifetime">Lifetime of the <see cref="ISimpleEndpointDefinition"/></param>
    /// <typeparam name="TAnchor">Assembly to Scan</typeparam>
    /// <returns>Service collection</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static IServiceCollection AddEndpointDefinitionsFromAssemblyContaining<TAnchor>(
        this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        var definerTypes = typeof(TAnchor).Assembly.ExportedTypes
            .Where(e => typeof(IEndpointDefinition).IsAssignableFrom(e))
            .Where(e => e is { IsAbstract: false, IsInterface: false });

        foreach (var definerType in definerTypes)
        {
            var defineDependenciesMethod = definerType.GetMethod(nameof(IFeatureDependencyDefinition.DefineDependencies));

            if (defineDependenciesMethod is not null)
            {
                services.Add(new ServiceDescriptor(typeof(ISimpleEndpointDefinition), definerType, lifetime));

                defineDependenciesMethod.Invoke(null, [services]);

                return services;
            }

            throw new InvalidOperationException(
                $"{definerType.FullName} does not implement {nameof(IFeatureDependencyDefinition)}");
        }

        return services;
    }
}
