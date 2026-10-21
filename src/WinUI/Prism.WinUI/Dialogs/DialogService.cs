using Prism.Properties;
using Prism.Mvvm;
using Prism.Navigation.Regions;
using Prism.Windows;
using Windows.Foundation;

namespace Prism.Dialogs;

#nullable enable
/// <summary>Displays dialogs in the window that owns this scoped service.</summary>
/// <remarks>
/// Focus changes never retarget this service. Resolve it through a different explicit
/// <see cref="IWindowContext.Container"/> to target that window. Calls are serialized
/// per window and canceled when that window closes. All calls must use the original
/// WinUI UI thread, and the target must have loaded content with a live XamlRoot.
/// </remarks>
public class DialogService : IDialogService
{
    private readonly WindowContext _context;
    private readonly IDialogViewRegistry _registry;

    /// <summary>Creates a dialog service bound to its originating window scope.</summary>
    /// <param name="windowContext">The scoped native window context, already attached to a window.</param>
    /// <param name="dialogViewRegistry">The shared dialog registration catalog.</param>
    public DialogService(IWindowContext windowContext, IDialogViewRegistry dialogViewRegistry)
    {
        ArgumentNullException.ThrowIfNull(windowContext);
        _context = windowContext as WindowContext ??
            throw new ArgumentException(NativeResources.DialogServiceRequiresManagedWindowContext, nameof(windowContext));
        _registry = dialogViewRegistry ?? throw new ArgumentNullException(nameof(dialogViewRegistry));
        _context.VerifyAccess();
    }

    /// <inheritdoc/>
    public async void ShowDialog(string name, IDialogParameters parameters, DialogCallback callback)
    {
        IDialogResult result;
        try
        {
            result = await ShowDialogAsync(name, parameters ?? new DialogParameters());
        }
        catch (Exception exception)
        {
            result = new DialogResult { Exception = exception };
        }

        // A callback failure must not invoke that same callback a second time as an error.
        await callback.Invoke(result);
    }

    private async Task<IDialogResult> ShowDialogAsync(string name, IDialogParameters parameters)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _ = _context.GetDialogRoot();
        await _context.DialogGate.WaitAsync(_context.ClosedToken);
        try
        {
            // The window may have closed or changed content while this request waited.
            _ = _context.GetDialogRoot();
            return await DisplayDialogAsync(name, parameters);
        }
        finally
        {
            _context.DialogGate.Release();
        }
    }

    private async Task<IDialogResult> DisplayDialogAsync(string name, IDialogParameters parameters)
    {
        var provider = _context.Container;
        var hostName = parameters.TryGetValue<string>(KnownDialogParameters.WindowName, out var value) ? value : null;
        var host = string.IsNullOrWhiteSpace(hostName) ? provider.Resolve<IDialogWindow>() : provider.Resolve<IDialogWindow>(hostName);
        if (host is not ContentDialog contentDialog)
            throw new InvalidOperationException(NativeResources.DialogHostMustDeriveFromContentDialog);
        // Validate before adding handlers, so a mistakenly shared host cannot damage another window.
        NativeRegionScope.ConfigureView(contentDialog, provider);

        IDialogAware? viewModel = null;
        IDialogResult? requestedResult = null;
        var opened = false;
        var cleaned = false;
        var windowClosing = false;
        var displayed = false;
        Exception? cleanupError = null;
        Exception? lifecycleError = null;

        void RequestClose(IDialogResult result)
        {
            _context.VerifyAccess();
            requestedResult = result ?? new DialogResult();
            host.Hide();
        }

        void OnLoaded(object sender, RoutedEventArgs args)
        {
            host.Loaded -= OnLoaded;
            try
            {
                if (viewModel is not null)
                    DialogUtilities.InitializeListener(viewModel, RequestClose);
            }
            catch (Exception exception)
            {
                lifecycleError = exception;
                host.Hide();
            }
        }

        void OnClosing(ContentDialog sender, ContentDialogClosingEventArgs args)
        {
            // A closed owner cannot keep a modal dialog alive by vetoing closure.
            if (windowClosing || _context.IsClosed || viewModel is null || lifecycleError is not null)
                return;
            try
            {
                args.Cancel = !viewModel.CanCloseDialog();
            }
            catch (Exception exception)
            {
                // Report user lifecycle failures through the dialog callback, not a WinRT event.
                lifecycleError = exception;
            }
        }

        void CleanUp()
        {
            if (cleaned)
                return;
            cleaned = true;
            _context.Closing -= OnWindowClosing;
            host.Loaded -= OnLoaded;
            host.Closing -= OnClosing;
            var errors = new List<Exception>();
            void Attempt(Action action)
            {
                try { action(); }
                catch (Exception exception) { errors.Add(exception); }
            }
            if (viewModel is not null)
            {
                Attempt(() => DialogUtilities.InitializeListener(viewModel, (Action<IDialogResult>)(_ => { })));
                if (opened)
                    Attempt(viewModel.OnDialogClosed);
            }
            // Release all retained references even if one user cleanup operation fails.
            Attempt(() => host.Content = null);
            Attempt(() => host.DataContext = null);
            Attempt(() => contentDialog.ClearValue(ViewModelLocator.ContainerProviderProperty));
            if (errors.Count != 0)
                throw new AggregateException(NativeResources.DialogCleanupFailed, errors);
        }

        void OnWindowClosing()
        {
            windowClosing = true;
            try
            {
                if (displayed)
                    host.Hide();
            }
            catch (Exception exception)
            {
                cleanupError = exception;
            }
            finally
            {
                // Execute before the window manager disposes scoped dependencies.
                try { CleanUp(); }
                catch (Exception exception) { cleanupError ??= exception; }
            }
        }

        host.Loaded += OnLoaded;
        host.Closing += OnClosing;
        _context.Closing += OnWindowClosing;
        try
        {
            if (_registry.CreateView(provider, name) is not FrameworkElement content)
                throw new InvalidOperationException(NativeResources.DialogContentMustBeFrameworkElement);
            viewModel = content.DataContext as IDialogAware ??
                throw new InvalidOperationException(NativeResources.DialogViewModelMustImplementIDialogAware);
            var style = Dialog.GetWindowStyle(content);
            if (style is not null)
                host.Style = style;
            host.Result = null;
            host.Content = content;
            host.DataContext = viewModel;
            opened = true;
            viewModel.OnDialogOpened(parameters);

            var placement = parameters.ContainsKey(KnownDialogParameters.DialogPlacement) ?
                parameters[KnownDialogParameters.DialogPlacement] is ContentDialogPlacement specified ? specified :
                    Enum.Parse<ContentDialogPlacement>(parameters[KnownDialogParameters.DialogPlacement]?.ToString() ?? string.Empty) :
                ContentDialogPlacement.Popup;
            contentDialog.XamlRoot = _context.GetDialogRoot();
            displayed = true;
            var operation = AwaitDialogAsync(host.ShowAsync(placement));
            var completed = await Task.WhenAny(operation, _context.ClosedTask);
            if (completed != operation || _context.IsClosed)
            {
                // Observe the WinRT operation even when destruction wins the race.
                _ = ObserveCompletionAsync(operation);
                throw new OperationCanceledException(NativeResources.DialogOwningWindowClosed, cleanupError, _context.ClosedToken);
            }
            await operation;
            if (lifecycleError is not null)
                throw lifecycleError;
            return requestedResult ?? host.Result ?? new DialogResult();
        }
        finally
        {
            CleanUp();
        }
    }

    private static async Task AwaitDialogAsync(IAsyncOperation<ContentDialogResult> operation) => await operation;

    private static async Task ObserveCompletionAsync(Task operation)
    {
        try { await operation; }
        catch { /* The owner-close result has already been reported to the caller. */ }
    }
}
