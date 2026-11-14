using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Prism;
using Prism.Dialogs;
using Prism.Ioc;
using Prism.Navigation.Regions;

namespace NativeWinUISmoke;

public sealed partial class SmokeShell : Grid, ILoadableShell
{
    public const string RegionName = "SmokeContent";
    public const string NestedRegionName = "NestedSmokeContent";
    private readonly SmokeState _state;

    public SmokeShell(SmokeState state, IDialogService dialogService,
        IContainerProvider container)
    {
        _state = state;
        DialogService = dialogService;
        Container = container;
        InitializeComponent();
        Log.ItemsSource = state.Messages;
        DialogButton.Click += (_, _) =>
        {
            DialogButton.IsEnabled = false;
            var callback = new DialogCallback()
                .OnClose(result =>
                {
                    state.Write($"Manual dialog closed: {result.Result}.");
                    DialogButton.IsEnabled = true;
                })
                .OnError((Exception exception) =>
                {
                    state.Check(false, exception.ToString());
                    DialogButton.IsEnabled = true;
                });
            dialogService.ShowDialog(nameof(SmokeDialog), new DialogParameters(), callback);
        };

    }

    public ContentControl RegionHost => RegionControl;
    public IDialogService DialogService { get; }
    public IContainerProvider Container { get; }
    public bool OnInitializedCompleted { get; set; }
    public int FinishLoadingCalls { get; private set; }

    public void FinishLoading()
    {
        FinishLoadingCalls++;
        _state.Check(OnInitializedCompleted, "FinishLoading runs after the application initialization callback returns.");
    }

    public void EnableManualChecks() => DialogButton.IsEnabled = true;
}
