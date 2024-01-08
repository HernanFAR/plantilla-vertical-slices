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

                continue;
            }
                
            throw new InvalidOperationException($"{definerType.FullName} does not implement {nameof(IFeatureDependencyDefinition)}");
        }

        return services;
    }

    public static IServiceCollection AddHandler<T>(this IServiceCollection services)
    {
        return services.AddHandler(typeof(T));
    }

    public static IServiceCollection AddHandler(this IServiceCollection services, 
        Type handlerType)
    {
        var handlerInterface = handlerType.GetInterfaces()
                .Where(o => o.IsGenericType)
                .SingleOrDefault(o => o.GetGenericTypeDefinition() == typeof(IHandler<,>));

        if (handlerInterface is null)
        {
            throw new InvalidOperationException(
                $"The type {handlerType.FullName} does not implement {typeof(IHandler<,>).FullName}");
        }

        services.AddTransient(handlerInterface, handlerType);

        return services;
    }
}
