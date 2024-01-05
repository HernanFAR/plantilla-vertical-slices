using Microsoft.Extensions.DependencyInjection;

namespace Shared.Core.Presentation;

/// <summary>
/// Specifies dependencies in a given feature
/// </summary>
public interface IFeatureDependencyDefinition
{
    /// <summary>
    /// Defines the dependencies for the feature
    /// </summary>
    /// <param name="services">Service collection</param>
    static abstract void DefineDependencies(IServiceCollection services);
}
