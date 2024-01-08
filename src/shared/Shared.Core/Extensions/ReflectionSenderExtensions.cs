using Shared.Core.Sender;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class ReflectionSenderExtensions
{
    /// <summary>
    /// Add a reflection <see cref="ISender"/> implementation to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">Service Collection</param>
    /// <returns>Service Collection</returns>
    public static IServiceCollection AddReflectionSender(this IServiceCollection services)
    {
        services.AddScoped<ISender, ReflectionSender>();

        return services;
    }
}
