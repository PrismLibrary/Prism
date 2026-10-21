using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Prism.Dialogs;

namespace NativeWinUISmoke;

public sealed class SmokeDialog : UserControl
{
    private DispatcherQueueTimer? _closeTimer;

    public SmokeDialog()
    {
        var content = new StackPanel { Spacing = 16, MinWidth = 320 };
        content.Children.Add(new TextBlock { Text = "Native Prism ContentDialog", FontSize = 24 });
        content.Children.Add(new TextBlock { Text = "The automatic check closes this dialog after a brief pause." });
        var close = new Button { Content = "Close with OK" };
        close.Click += (_, _) => ((SmokeDialogViewModel)DataContext).RequestClose.Invoke(ButtonResult.OK);
        content.Children.Add(close);
        Content = content;
        Loaded += OnLoaded;
        Unloaded += (_, _) => _closeTimer?.Stop();
    }

    private void OnLoaded(object sender, RoutedEventArgs args)
    {
        if (DataContext is not SmokeDialogViewModel { AutoClose: true })
            return;

        // Wait until the host's Loaded handler has initialized RequestClose.
        _closeTimer = DispatcherQueue.CreateTimer();
        _closeTimer.Interval = TimeSpan.FromMilliseconds(600);
        _closeTimer.IsRepeating = false;
        _closeTimer.Tick += (_, _) => ((SmokeDialogViewModel)DataContext).RequestClose.Invoke(ButtonResult.OK);
        _closeTimer.Start();
    }
}
