using Prism.Properties;
using System.Threading;
using Microsoft.UI.Dispatching;
using Prism.Navigation.Regions;

namespace Prism.Windows;

#nullable enable
internal sealed class WindowContext : IWindowContext
{
    private IScopedProvider? _scope;
    private Window? _window;
    private DispatcherQueue? _dispatcher;
    private readonly CancellationTokenSource _closed = new();
    private readonly TaskCompletionSource<bool> _closedCompletion = new(TaskCreationOptions.RunContinuationsAsynchronously);

    internal SemaphoreSlim DialogGate { get; } = new(1, 1);
    internal CancellationToken ClosedToken => _closed.Token;
    internal Task ClosedTask => _closedCompletion.Task;
    internal event Action? Closing;

    public Window Window => _window ?? throw new InvalidOperationException(NativeResources.WindowContextNotAttached);
    public bool IsClosed { get; private set; }
    public IContainerProvider Container
    {
        get
        {
            VerifyAccess();
            return _scope!;
        }
    }
    public IRegionManager RegionManager => Container.Resolve<IRegionManager>();

    internal void Attach(Window window, IScopedProvider scope, DispatcherQueue dispatcher)
    {
        if (_window is not null || IsClosed)
            throw new InvalidOperationException(NativeResources.WindowScopeCannotBeReused);
        _window = window;
        _scope = scope;
        _dispatcher = dispatcher;
        scope.IsAttached = true;
    }

    internal void VerifyAccess()
    {
        ObjectDisposedException.ThrowIf(IsClosed, this);
        if (_window is null || _scope is null || _dispatcher is null)
            throw new InvalidOperationException(NativeResources.WindowServiceRequiresOpenContext);
        if (!_dispatcher.HasThreadAccess)
            throw new InvalidOperationException(NativeResources.WindowServicesRequireApplicationThread);
    }

    internal XamlRoot GetDialogRoot()
    {
        VerifyAccess();
        // Never cache XamlRoot: replacing window content can replace the live root.
        var content = Window.Content;
        if (content is null || content.XamlRoot is null || content is FrameworkElement { IsLoaded: false })
            throw new InvalidOperationException(NativeResources.DialogRequiresLoadedWindow);
        return content.XamlRoot;
    }

    internal void Close(Action cleanUp)
    {
        if (IsClosed)
            return;

        IsClosed = true;
        var errors = new List<Exception>();
        void Attempt(Action action)
        {
            try { action(); }
            catch (Exception exception) { errors.Add(exception); }
        }

        // Dialog cleanup runs synchronously before scoped view models are disposed.
        Attempt(() => _closed.Cancel());
        _closedCompletion.TrySetResult(true);
        if (Closing is not null)
        {
            foreach (Action handler in Closing.GetInvocationList())
                Attempt(handler);
            Closing = null;
        }
        Attempt(cleanUp);
        var scope = _scope;
        _scope = null;
        if (scope is not null)
            Attempt(scope.Dispose);

        // Do not dispose the gate/CTS while canceled dialog continuations still use them.
        if (errors.Count != 0)
            throw new AggregateException(NativeResources.WindowCleanupFailed, errors);
    }
}
