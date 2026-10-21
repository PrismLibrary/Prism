using Microsoft.UI.Xaml.Controls;

namespace NativeWinUISmoke;

public sealed class HomeView : UserControl
{
    public HomeView()
    {
        Content = new TextBlock { Text = "Home view resolved through native Prism region navigation.", FontSize = 18 };
    }
}
