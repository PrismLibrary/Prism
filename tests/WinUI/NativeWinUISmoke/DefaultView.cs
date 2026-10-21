using Microsoft.UI.Xaml.Controls;
using Prism.Mvvm;
using Prism.Ioc;

namespace NativeWinUISmoke;

public sealed class DefaultView : UserControl
{
    public DefaultView(WindowLifetime lifetime, IContainerProvider container)
    {
        Lifetime = lifetime;
        Container = container;
        Content = new TextBlock { Text = "Default view discovered in the owning window scope." };
        ViewModelLocator.SetAutowireViewModel(this, true);
    }

    public WindowLifetime Lifetime { get; }
    public IContainerProvider Container { get; }
}
