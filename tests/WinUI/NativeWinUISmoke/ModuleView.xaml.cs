using Microsoft.UI.Xaml.Controls;
using Prism.Ioc;

namespace NativeWinUISmoke;

public sealed partial class ModuleView : UserControl
{
    public ModuleView(WindowLifetime lifetime, IContainerProvider container)
    {
        Lifetime = lifetime;
        Container = container;
        InitializeComponent();
    }

    public WindowLifetime Lifetime { get; }
    public IContainerProvider Container { get; }
}
