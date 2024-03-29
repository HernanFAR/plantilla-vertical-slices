﻿using Microsoft.AspNetCore.Routing;

namespace Shared.Core.Presentation.AspNetCore;

/// <summary>
/// Defines an endpoint of a use case without dependencies
/// </summary>
public interface ISimpleEndpointDefinition
{
    /// <summary>
    /// Defines the endpoint of the use case.
    /// </summary>
    /// <param name="builder">Endpoint route builder</param>
    void Define(IEndpointRouteBuilder builder);
}

/// <summary>
/// Defines an endpoint of a use case with dependencies
/// </summary>
public interface IEndpointDefinition : IFeatureDependencyDefinition, ISimpleEndpointDefinition;
