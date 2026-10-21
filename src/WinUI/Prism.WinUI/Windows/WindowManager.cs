using Microsoft.UI.Dispatching;
using Prism.Common;
using Prism.Mvvm;
using Prism.Navigation.Regions;

namespace Prism.Windows;

#nullable enable
internal sealed class WindowManager : IWindowManager
{
    private readonly IContainerProvider _container;
    private readonly DispatcherQueue _dispatcher;
    private readonly Dictionary<Window, WindowContext> _windows = new();

    internal WindowManager(IContainerProvider container)
    {
        _container = container;
        _dispatcher = DispatcherQueue.GetForCurrentThread() ??
            throw new InvalidOperationException("The window manager must be created on the application's WinUI thread.");
    }

    public IReadOnlyList<IWindowContext> Windows
    {
        get
        {
            VerifyAccess();
            return _windows.Values.Cast<IWindowContext>().ToArray();
        }
    }

    public IWindowContext GetWindow(Window window)
    {
        ArgumentNullException.ThrowIfNull(window);
        VerifyAccess();
        return _windows.TryGetValue(window, out var context) ? context :
            throw new InvalidOperationException("The target window is not open in this window manager.");
    }

    public IWindowContext OpenWindow(Window window, Func<IContainerProvider, UIElement> createShell,
        Action<IWindowContext>? onInitialized = null)
    {
        ArgumentNullException.ThrowIfNull(window);
        ArgumentNullException.ThrowIfNull(createShell);
        VerifyAccess();
        if (!window.DispatcherQueue.HasThreadAccess)
            throw new InvalidOperationException("All native Prism windows must be created on the application's original WinUI thread.");
        if (_windows.ContainsKey(window) || window.Content is not null)
            throw new InvalidOperationException("OpenWindow requires an unused window with no existing content.");

        var scope = _container.CreateScope();
        WindowContext? context = null;
        UIElement? shell = null;
        FrameworkElement? element = null;
        IRegionManager? regionManager = null;
        var completed = false;
        var queued = false;
        var closing = false;

        void RemoveHandlers()
        {
            window.Closed -= OnClosed;
            if (element is not null)
                element.Loaded -= OnLoaded;
        }

        void CleanUp()
        {
            RemoveHandlers();
            _windows.Remove(window);
            if (shell is null)
                return;
            try
            {
                MvvmHelpers.ViewAndViewModelAction<IActiveAware>(shell, aware => aware.IsActive = false);
            }
            finally
            {
                // Region cleanup must not touch other windows or dispose this scope itself.
                if (regionManager is not null)
                    RegionManager.CleanupWindow(shell, regionManager);
                shell.ClearValue(ViewModelLocator.ContainerProviderProperty);
                shell.ClearValue(RegionManager.RegionManagerProperty);
            }
        }

        void OnClosed(object sender, WindowEventArgs args)
        {
            if (closing)
                return;
            closing = true;
            context!.Close(CleanUp);
        }

        void CloseFailedWindow()
        {
            try
            {
                // Closing a native window is terminal, even when its startup failed.
                window.Close();
            }
            finally
            {
                if (context is not null)
                    context.Close(CleanUp);
                else
                    scope.Dispose();
            }
        }

        void CompleteInitialization()
        {
            if (completed || context!.IsClosed)
                return;
            completed = true;
            try
            {
                RegionManager.UpdateRegions();
                onInitialized?.Invoke(context);
                if (context.IsClosed)
                    return;
                if (shell is ILoadableShell loadableShell)
                    loadableShell.FinishLoading();
                if (context.IsClosed)
                    return;
                MvvmHelpers.ViewAndViewModelAction<IActiveAware>(shell!, aware => aware.IsActive = true);
            }
            catch
            {
                CloseFailedWindow();
                throw;
            }
        }

        void QueueInitialization()
        {
            if (queued || context!.IsClosed)
                return;
            queued = true;
            if (!_dispatcher.TryEnqueue(CompleteInitialization))
                throw new InvalidOperationException("Unable to queue window initialization on the WinUI dispatcher.");
        }

        void OnLoaded(object sender, RoutedEventArgs args)
        {
            element!.Loaded -= OnLoaded;
            try { QueueInitialization(); }
            catch
            {
                CloseFailedWindow();
                throw;
            }
        }

        try
        {
            context = scope.Resolve<IWindowContext>() as WindowContext ??
                throw new InvalidOperationException("The native window context registration must be scoped and managed by Prism.");
            context.Attach(window, scope, _dispatcher);
            _windows.Add(window, context);
            window.Closed += OnClosed;
            regionManager = context.RegionManager;
            var candidate = createShell(scope) ?? throw new InvalidOperationException("The window's shell factory must return a shell.");
            context.VerifyAccess();
            // Do not retain a foreign shell for cleanup if ownership validation fails.
            NativeRegionScope.ConfigureView(candidate, scope);
            shell = candidate;
            ViewModelLocator.Autowire(shell);
            RegionManager.SetRegionManager(shell, regionManager);
            element = shell as FrameworkElement;
            if (element is not null && !element.IsLoaded)
                element.Loaded += OnLoaded;
            window.Content = shell;
            window.Activate();
            if (element is null || element.IsLoaded)
                QueueInitialization();
            return context;
        }
        catch
        {
            CloseFailedWindow();
            throw;
        }
    }

    private void VerifyAccess()
    {
        if (!_dispatcher.HasThreadAccess)
            throw new InvalidOperationException("The native window manager supports only the application's original WinUI thread.");
    }
}
