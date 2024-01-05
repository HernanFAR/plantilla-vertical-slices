using Shared.Core.Configurations;
using Shared.Core.Events;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class EventExtensions
{
    /// <summary>
    /// Add an in memory <see cref="IEventQueue"/> implementation to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">Service Collection</param>
    /// <param name="configAction">Configuration for the implementation</param>
    /// <returns>Service Collection</returns>
    public static IServiceCollection AddInMemoryEventQueue(this IServiceCollection services, 
        Action<InMemoryEventQueueConfiguration>? configAction = null)
    {
        var configuration = new InMemoryEventQueueConfiguration();

        configAction?.Invoke(configuration);

        services.AddSingleton(configuration);
        services.AddSingleton<IEventQueue, InMemoryEventQueue>();
        services.AddSingleton<IEventQueueWriter>(s => s.GetRequiredService<IEventQueue>());
        services.AddSingleton<IEventQueueReader>(s => s.GetRequiredService<IEventQueue>());

        return services;
    }

    /// <summary>
    /// Adds a hosted service that will listen for events in the background
    /// </summary>
    /// <param name="services">Service Collection</param>
    /// <param name="configAct">Action to configure the service</param>
    /// <returns>Service Collection</returns>
    public static IServiceCollection AddBackgroundEventListenerService(this IServiceCollection services, Action<BackgroundEventListenerConfiguration>? configAct = null)
    {
        services.AddHostedService<BackgroundEventListenerService>();

        var config = new BackgroundEventListenerConfiguration();
        configAct?.Invoke(config);

        services.AddSingleton(config);

        return services;
    }
}
