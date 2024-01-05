using Shared.Core.Events;
using Shared.Core.Events.Strategies;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class ReflectionPublisherExtensions
{
    /// <summary>
    /// Add a reflection <see cref="IPublisher"/> implementation to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <remarks>Default strategy is <see cref="AwaitInParallelStrategy"/></remarks>
    /// <param name="services">Service Collection</param>
    /// <param name="strategy">Strategy</param>
    /// <returns>Service Collection</returns>
    public static IServiceCollection AddReflectionPublisher(this IServiceCollection services, 
        IPublishingStrategy? strategy = null)
    {
        strategy ??= new AwaitInParallelStrategy();

        services.AddScoped<IPublisher, ReflectionPublisher>();
        services.AddSingleton(strategy);

        return services;
    }
}
