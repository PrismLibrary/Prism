using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Prism.Dialogs;

namespace NativeWinUISmoke;

public sealed class SmokeDialog : UserControl
{
    private readonly SmokeState _state;
    private readonly WindowLifetime _lifetime;
    private DispatcherQueueTimer? _closeTimer;

    public SmokeDialog(SmokeState state, WindowLifetime lifetime)
    {
        _state = state;
        _lifetime = lifetime;
        var content = new StackPanel { Spacing = 16, MinWidth = 320 };
        content.Children.Add(new TextBlock { Text = "Native Prism ContentDialog", FontSize = 24 });
        content.Children.Add(new TextBlock { Text = "The automatic checks close their dialogs without input." });
        var close = new Button { Content = "Close with OK" };
        close.Click += (_, _) => ((SmokeDialogViewModel)DataContext).RequestClose.Invoke(ButtonResult.OK);
        content.Children.Add(close);
        Content = content;
        Loaded += OnLoaded;
        Unloaded += (_, _) => _closeTimer?.Stop();
    }

    private void OnLoaded(object sender, RoutedEventArgs args)
    {
        if (DataContext is not SmokeDialogViewModel viewModel)
            return;

        var observation = _state.GetDialog(viewModel.Id);
        observation.LoadedCount++;
        observation.ViewLifetime = _lifetime;
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
        _closeTimer?.Stop();
        _closeTimer = DispatcherQueue.CreateTimer();
        _closeTimer.Interval = TimeSpan.FromMilliseconds(300);
        _closeTimer.IsRepeating = false;
        _closeTimer.Tick += (_, _) => viewModel.RequestClose.Invoke(ButtonResult.OK);
        _closeTimer.Start();
    }
}
