using Microsoft.UI.Xaml.Controls;
using Prism.Ioc;

namespace NativeWinUISmoke;

public sealed class ModuleView : UserControl
{
    public ModuleView(WindowLifetime lifetime, IContainerProvider container)
    {
        Lifetime = lifetime;
        Container = container;
        Content = new TextBlock { Text = "Module view: registration and navigation succeeded.", FontSize = 18 };
    }

    public WindowLifetime Lifetime { get; }
    public IContainerProvider Container { get; }
}
