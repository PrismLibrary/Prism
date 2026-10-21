using Microsoft.UI.Xaml.Controls;

namespace NativeWinUISmoke;

public sealed partial class HomeView : UserControl
{
    public HomeView()
    {
        InitializeComponent();
    }

    public ContentControl NestedRegionHost => NestedRegionControl;
}
