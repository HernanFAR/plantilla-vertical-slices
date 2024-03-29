﻿// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class CoreDependencies
{
    public static IServiceCollection AddCoreDependencies(this IServiceCollection services)
        => services
            .AddReflectionSender()
            .AddReflectionPublisher()
            .AddInMemoryEventQueue()
            .AddBackgroundEventListenerService()
            .AddFeatureDependenciesFromAssemblyContaining<Anchor>();

}

internal class Anchor;
