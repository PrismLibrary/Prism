using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Prism.Dialogs;

namespace NativeWinUISmoke;

public sealed partial class SmokeDialog : UserControl
{
    private readonly SmokeState _state;
    private DispatcherQueueTimer? _closeTimer;

    public SmokeDialog(SmokeState state)
    {
        _state = state;
        InitializeComponent();
        Loaded += OnLoaded;

    }

    private void CloseDialog(object sender, RoutedEventArgs args) =>
        ((SmokeDialogViewModel)DataContext).RequestClose.Invoke(ButtonResult.OK);

    private void OnLoaded(object sender, RoutedEventArgs args)
    {
        if (DataContext is not SmokeDialogViewModel viewModel)
            return;

        var observation = _state.GetDialog(viewModel.Id);
        observation.LoadedCount++;
        observation.ViewRoot = XamlRoot;
        // Inspect the actual Prism host, not an expected root supplied by the caller.
        DependencyObject? parent = this;
        while (parent is not null && parent is not ContentDialog)
            parent = VisualTreeHelper.GetParent(parent) ?? (parent as FrameworkElement)?.Parent;
        observation.Host = parent as ContentDialog;
        observation.HostRoot = observation.Host?.XamlRoot;
        observation.Loaded.TrySetResult(true);

        if (!viewModel.AutoClose)
            return;

        // Wait until the host's Loaded handler has initialized RequestClose.
        // ContentDialog reparents its content while opening, so Unloaded must not cancel this timer.
        _closeTimer?.Stop();
        _closeTimer = DispatcherQueue.CreateTimer();
        _closeTimer.Interval = TimeSpan.FromMilliseconds(300);
        _closeTimer.IsRepeating = false;
        _closeTimer.Tick += (_, _) => viewModel.RequestClose.Invoke(ButtonResult.OK);
        _closeTimer.Start();
    }
}
