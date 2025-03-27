using System.Runtime.CompilerServices;
using MediatR;

namespace ServiceScopeMediator.Sample.ScopedMediator;

public class ScopedMediator : IMediator
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    private bool IsNested { get; set; }
    private IServiceProvider? ServiceProvider { get; set; }

    public ScopedMediator(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = new())
    {
        return await RunInScope(async sp =>
        {
            var mediator = new Mediator(sp);

            return await mediator.Send(request, cancellationToken);
        });
    }

    public async Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = new()) where TRequest : IRequest
    {
        await RunInScope(async sp =>
        {
            var mediator = new Mediator(sp);

            await mediator.Send(request, cancellationToken);
        });
    }

    public async Task<object?> Send(object request, CancellationToken cancellationToken = new())
    {
        return await RunInScope(async sp =>
        {
            var mediator = new Mediator(sp);

            return await mediator.Send(request, cancellationToken);
        });
    }

    public async IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request,
        [EnumeratorCancellation] CancellationToken cancellationToken = new())
    {
        var (scope, serviceProvider) = SetupScope();

        try
        {
            var mediator = new Mediator(serviceProvider);

            var items = mediator.CreateStream(request, cancellationToken);

            await foreach (var item in items)
            {
                yield return item;
            }
        }
        finally
        {
            await scope.DisposeAsync();
        }
    }

    public async IAsyncEnumerable<object?> CreateStream(object request, [EnumeratorCancellation] CancellationToken cancellationToken = new())
    {
        var (scope, serviceProvider) = SetupScope();

        try
        {
            var mediator = new Mediator(serviceProvider);

            var items = mediator.CreateStream(request, cancellationToken);

            await foreach (var item in items)
            {
                yield return item;
            }
        }
        finally
        {
            await scope.DisposeAsync();
        }
    }

    public async Task Publish(object notification, CancellationToken cancellationToken = new())
    {
        await RunInScope(async sp =>
        {
            var mediator = new Mediator(sp);

            await mediator.Publish(notification, cancellationToken);
        });
    }

    public async Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = new()) where TNotification : INotification
    {
        await RunInScope(async sp =>
        {
            var mediator = new Mediator(sp);

            await mediator.Publish(notification, cancellationToken);
        });
    }

    private async Task<TResult> RunInScope<TResult>(Func<IServiceProvider, Task<TResult>> func)
    {
        var (scope, serviceProvider) = SetupScope();

        try
        {
            return await func(serviceProvider);
        }
        finally
        {
            await scope.DisposeAsync();
        }
    }

    private async Task RunInScope(Func<IServiceProvider, Task> func)
    {
        var (scope, serviceProvider) = SetupScope();

        try
        {
            await func(serviceProvider);
        }
        finally
        {
            await scope.DisposeAsync();
        }
    }

    private (IAsyncDisposable, IServiceProvider) SetupScope()
    {
        if (IsNested)
        {
            return SetupFromNestedScope();
        }

        var scope = _serviceScopeFactory.CreateAsyncScope();

        scope.ServiceProvider.GetRequiredService<ScopedMediator>().IsNested = true;
        scope.ServiceProvider.GetRequiredService<ScopedMediator>().ServiceProvider = scope.ServiceProvider;

        return (scope, scope.ServiceProvider);
    }

    private (IAsyncDisposable, IServiceProvider) SetupFromNestedScope()
    {
        return (new NoOpDisposable(), ServiceProvider!);
    }

    private class NoOpDisposable : IAsyncDisposable
    {
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}