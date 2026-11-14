using Microsoft.UI.Xaml.Controls;
using Prism.Mvvm;
using Prism.Ioc;

namespace NativeWinUISmoke;

public sealed partial class DefaultView : UserControl
{
    public DefaultView(IContainerProvider container)
    {
        Container = container;
        InitializeComponent();
        ViewModelLocator.SetAutowireViewModel(this, true);
    }

    public IContainerProvider Container { get; }
}
