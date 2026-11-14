using Microsoft.UI.Xaml.Controls;
using Prism;
using Prism.Navigation;
using Prism.Navigation.Regions;

namespace NativeWinUISmoke;

public sealed partial class SmokeNavigationView : UserControl, IDestructible
{
    public SmokeNavigationView() => InitializeComponent();
    public int DestroyCalls { get; private set; }
    public void Destroy() => DestroyCalls++;
}

public sealed class SmokeNavigationViewModel : IConfirmNavigationRequest, IRegionMemberLifetime, IDestructible, IActiveAware
{
    public string? Key { get; private set; }
    public string? Query { get; private set; }
    public object? Payload { get; private set; }
    public bool KeepAlive { get; set; } = true;
    public bool AllowNavigation { get; set; } = true;
    public bool DeferNavigation { get; set; }
    public Action<bool>? PendingConfirmation { get; private set; }
    public int NavigatedToCalls { get; private set; }
    public int NavigatedFromCalls { get; private set; }
    public int DestroyCalls { get; private set; }
    public int ActivationChanges { get; private set; }
    public event EventHandler? IsActiveChanged;
    public bool IsActive
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            ActivationChanges++;
            IsActiveChanged?.Invoke(this, EventArgs.Empty);
        }
    }
    public bool IsNavigationTarget(NavigationContext context) => Key == context.Parameters.GetValue<string>("key");
    public void OnNavigatedTo(NavigationContext context)
    {
        Key = context.Parameters.GetValue<string>("key");
        Query = context.Parameters.GetValue<string>("query");
        Payload = context.Parameters.GetValue<object>("payload");
        NavigatedToCalls++;
    }
    public void OnNavigatedFrom(NavigationContext context) => NavigatedFromCalls++;
    public void ConfirmNavigationRequest(NavigationContext context, Action<bool> continuationCallback)
    {
        if (DeferNavigation) PendingConfirmation = continuationCallback;
        else continuationCallback(AllowNavigation);
    }
    public void CompleteConfirmation(bool allow)
    {
        var callback = PendingConfirmation ?? throw new InvalidOperationException("No pending confirmation.");
        PendingConfirmation = null;
        DeferNavigation = false;
        callback(allow);
    }
    public void Destroy() => DestroyCalls++;
}
