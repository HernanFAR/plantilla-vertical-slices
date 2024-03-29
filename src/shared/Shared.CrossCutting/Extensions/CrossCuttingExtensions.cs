﻿using Microsoft.Extensions.DependencyInjection;
using Shared.CrossCutting.Abstracts;
using Shared.CrossCutting.ExceptionHandling;
using Shared.CrossCutting.Logging;
using Shared.CrossCutting.Validation;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class CrossCuttingExtensions
{
    /// <summary>
    /// Adds the FluentValidationBehavior to the pipeline.
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddValidationBehavior(this IServiceCollection services)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }

    /// <summary>
    /// Adds the FluentValidationBehavior to the pipeline.
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddExceptionHandlingBehavior(this IServiceCollection services)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ExceptionHandlingBehavior<,>));

        return services;
    }

    /// <summary>
    /// Add the default logging behavior to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configAction">Setups the <see cref="LoggingBehaviorConfiguration"/></param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddLoggingBehavior(this IServiceCollection services,
        Action<LoggingBehaviorConfiguration>? configAction = null)
    {
        var configuration = new LoggingBehaviorConfiguration();

        configAction?.Invoke(configuration);

        services.AddSingleton(configuration);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

        return services;
    }
}
