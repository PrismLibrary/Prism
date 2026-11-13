using Microsoft.UI.Xaml.Controls;
using Prism.Mvvm;
using Prism.Ioc;

namespace NativeWinUISmoke;

public sealed partial class DefaultView : UserControl
{
    public DefaultView(WindowLifetime lifetime, IContainerProvider container)
    {
        Lifetime = lifetime;
        Container = container;
        InitializeComponent();
        ViewModelLocator.SetAutowireViewModel(this, true);
    }

    public WindowLifetime Lifetime { get; }
    public IContainerProvider Container { get; }
}
