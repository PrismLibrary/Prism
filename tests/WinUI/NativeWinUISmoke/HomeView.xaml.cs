using Microsoft.UI.Xaml.Controls;
using Prism.Ioc;

namespace NativeWinUISmoke;

public sealed partial class HomeView : UserControl
{
    public HomeView(IContainerProvider container)
    {
        Container = container;
        InitializeComponent();
    }

    public IContainerProvider Container { get; }
    public ContentControl NestedRegionHost => NestedRegionControl;
}
