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

    public Window Window => _window ?? throw new InvalidOperationException("The window context has not been attached to a window.");
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
            throw new InvalidOperationException("A window scope cannot be reused.");
        _window = window;
        _scope = scope;
        _dispatcher = dispatcher;
        scope.IsAttached = true;
    }

    internal void VerifyAccess()
    {
        ObjectDisposedException.ThrowIf(IsClosed, this);
        if (_window is null || _scope is null || _dispatcher is null)
            throw new InvalidOperationException("A window-owned service must be resolved through an open window context.");
        if (!_dispatcher.HasThreadAccess)
            throw new InvalidOperationException("Native Prism windows and their scoped services must use the application's WinUI thread.");
    }

    internal XamlRoot GetDialogRoot()
    {
        VerifyAccess();
        // Never cache XamlRoot: replacing window content can replace the live root.
        var content = Window.Content;
        if (content is null || content.XamlRoot is null || content is FrameworkElement { IsLoaded: false })
            throw new InvalidOperationException("Dialogs require an open window with loaded content and a live XamlRoot.");
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
            throw new AggregateException("One or more window cleanup operations failed.", errors);
    }
}
