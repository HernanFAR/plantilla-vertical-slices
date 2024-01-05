using Shared.Core.Handlers;
using Shared.Core.Presentation;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the dependencies defined in the <see cref="IFeatureDependencyDefinition"/> implementations
    /// </summary>
    /// <typeparam name="TAnchor"></typeparam>
    /// <param name="services">Service collection</param>
    /// <returns>Service collection</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static IServiceCollection AddFeatureDependenciesFromAssemblyContaining<TAnchor>(this IServiceCollection services)
    {
        var definerTypes = typeof(TAnchor).Assembly.ExportedTypes
            .Where(e => typeof(IFeatureDependencyDefinition).IsAssignableFrom(e))
            .Where(e => e is { IsAbstract: false, IsInterface: false });

        foreach (var definerType in definerTypes)
        {
            var defineDependenciesMethod = definerType.GetMethod(nameof(IFeatureDependencyDefinition.DefineDependencies));

            if (defineDependenciesMethod is not null)
            {
                defineDependenciesMethod.Invoke(null, [services]);

                return services;
            }
                
            throw new InvalidOperationException($"{definerType.FullName} does not implement {nameof(IFeatureDependencyDefinition)}");
        }

        return services;
    }

    /// <summary>
    /// Adds <see cref="IHandler{TRequest,TResponse}"/> implementations from the specified assembly of the <typeparamref name="TAnchor"/> type, to the service collection.
    /// </summary>
    /// <typeparam name="TAnchor">Anchor type to search</typeparam>
    /// <param name="services">Service collection</param>
    /// <param name="lifetime">Lifetime</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddHandlersFromAssemblyContaining<TAnchor>(this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        var definerTypes = typeof(TAnchor).Assembly.ExportedTypes
            .Where(e => e.GetInterfaces().Where(o => o.IsGenericType).Any(o => o.GetGenericTypeDefinition() == typeof(IHandler<,>)))
            .Where(e => e is { IsAbstract: false, IsInterface: false })
            .Select(e => (e, e.GetInterfaces()
                .Where(o => o.IsGenericType)
                .Single(o => o.GetGenericTypeDefinition() == typeof(IHandler<,>))));

        foreach (var (handlerType, handlerInterface) in definerTypes)
        {
            services.Add(new ServiceDescriptor(handlerInterface, handlerType, lifetime));
        }

        return services;
    }
}
