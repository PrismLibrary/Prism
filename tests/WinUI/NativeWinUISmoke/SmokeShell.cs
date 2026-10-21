using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Prism;
using Prism.Dialogs;
using Prism.Ioc;
using Prism.Navigation.Regions;

namespace NativeWinUISmoke;

public sealed class SmokeShell : Grid, ILoadableShell
{
    public const string RegionName = "SmokeContent";
    public const string NestedRegionName = "NestedSmokeContent";
    private readonly SmokeState _state;
    private readonly Button _dialogButton;

    public SmokeShell(SmokeState state, IDialogService dialogService,
        WindowLifetime lifetime, IContainerProvider container)
    {
        _state = state;
        DialogService = dialogService;
        Lifetime = lifetime;
        Container = container;
        Padding = new Thickness(24);
        RowSpacing = 16;
        RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        RowDefinitions.Add(new RowDefinition());
        Children.Add(new TextBlock { Text = "Prism native WinUI smoke", FontSize = 28 });

        RegionHost = new ContentControl { HorizontalContentAlignment = HorizontalAlignment.Stretch };
        RegionManager.SetRegionName(RegionHost, RegionName);
        SetRow(RegionHost, 1);
        Children.Add(RegionHost);

        _dialogButton = new Button { Content = "Open Prism dialog", IsEnabled = false };
        _dialogButton.Click += (_, _) =>
        {
            _dialogButton.IsEnabled = false;
            var callback = new DialogCallback()
                .OnClose(result =>
                {
                    state.Write($"Manual dialog closed: {result.Result}.");
                    _dialogButton.IsEnabled = true;
                })
                .OnError((Exception exception) =>
                {
                    state.Check(false, exception.ToString());
                    _dialogButton.IsEnabled = true;
                });
            dialogService.ShowDialog(nameof(SmokeDialog), new DialogParameters(), callback);
        };
        SetRow(_dialogButton, 2);
        Children.Add(_dialogButton);

        var log = new ListView { ItemsSource = state.Messages, SelectionMode = ListViewSelectionMode.None };
        SetRow(log, 3);
        Children.Add(log);
    }

    public ContentControl RegionHost { get; }
    public IDialogService DialogService { get; }
    public WindowLifetime Lifetime { get; }
    public IContainerProvider Container { get; }
    public bool OnInitializedCompleted { get; set; }
    public int FinishLoadingCalls { get; private set; }

    public void FinishLoading()
    {
        FinishLoadingCalls++;
        _state.Check(OnInitializedCompleted, "FinishLoading runs after the owning window initialization callback returns.");
    }

    public void EnableManualChecks() => _dialogButton.IsEnabled = true;
}
