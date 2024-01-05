using Microsoft.Extensions.DependencyInjection;
using Shared.Abstracts.Requests;
using Shared.Abstracts.Responses;
using Shared.Core.Handlers;
using Shared.CrossCutting.Abstracts;

namespace Shared.Core.Sender.Internals;

internal abstract class AbstractHandlerWrapper
{
    public abstract ValueTask<Result<object?>> HandleAsync(
        object request,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken);
}

internal abstract class AbstractHandlerWrapper<TResult> : AbstractHandlerWrapper
{
    public abstract ValueTask<Result<TResult>> HandleAsync(
        IBaseRequest<TResult> request,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken);
}

internal class RequestHandlerWrapper<TRequest, TResponse> : AbstractHandlerWrapper<TResponse>
    where TRequest : IBaseRequest<TResponse>
{
    public override async ValueTask<Result<object?>> HandleAsync(
        object request, IServiceProvider serviceProvider, CancellationToken cancellationToken) =>
        await HandleAsync((IBaseRequest<TResponse>)request, serviceProvider, cancellationToken);

    public override ValueTask<Result<TResponse>> HandleAsync(
        IBaseRequest<TResponse> request, IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        return serviceProvider
            .GetServices<IPipelineBehavior<TRequest, TResponse>>()
            .Reverse()
            .Aggregate((RequestHandlerDelegate<TResponse>)Handler,
                (next, pipeline) =>
                    () => pipeline.HandleAsync((TRequest)request, next, cancellationToken))();

        ValueTask<Result<TResponse>> Handler()
        {
            return serviceProvider.GetRequiredService<IHandler<TRequest, TResponse>>()
                .HandleAsync((TRequest)request, cancellationToken);
        }
    }
}
