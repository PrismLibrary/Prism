using System.Windows;
using Avalonia;
using Prism.DryIoc;
using Prism.Ioc;

namespace Prism.Container.Avalonia.Mocks
{
    internal partial class NullLoggerBootstrapper : PrismBootstrapper
    {
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            throw new System.NotImplementedException();
        }

        protected override AvaloniaObject CreateShell()
        {
            throw new System.NotImplementedException();
        }
    }
}
