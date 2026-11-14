using Microsoft.UI.Xaml.Controls;

namespace NativeWinUISmoke;

public sealed partial class AdapterFixture : Grid
{
    public AdapterFixture() => InitializeComponent();
    public NavigationView Navigation => NavigationHost;
    public NavigationViewItem First => FirstDestination;
    public NavigationViewItem Second => SecondDestination;
    public ItemsControl Items => ItemsHost;
    public TextBlock OriginalItem => InitialItem;
    public ListView Selection => SelectionHost;
}
