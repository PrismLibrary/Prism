using Microsoft.UI.Xaml.Controls;

namespace NativeWinUISmoke;

public sealed class ModuleView : UserControl
{
    public ModuleView()
    {
        Content = new TextBlock { Text = "Module view: registration and navigation succeeded.", FontSize = 18 };
    }
}
