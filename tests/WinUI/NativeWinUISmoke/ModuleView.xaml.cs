using Microsoft.UI.Xaml.Controls;
using Prism.Ioc;

namespace NativeWinUISmoke;

public sealed partial class ModuleView : UserControl
{
    public ModuleView(IContainerProvider container)
    {
        Container = container;
        InitializeComponent();
    }

    public IContainerProvider Container { get; }
}
