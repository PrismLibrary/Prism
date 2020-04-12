using System.Windows;
using System.Windows.Controls;
using DryIoc;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Logging;

namespace Prism.Container.Wpf.Mocks
{
    internal class MockedContainerBootstrapper : DryIocBootstrapper
    {
        private readonly IContainer container;
        public ILoggerFacade BaseLogger => base.Logger;

        public void CallConfigureContainer()
        {
            base.ConfigureContainer();
        }

        public MockedContainerBootstrapper(IContainer container)
        {
            ContainerLocator.ResetContainer();
            this.container = container;
        }

        protected override IContainer CreateContainer()
        {
            return container;
        }

        protected override DependencyObject CreateShell()
        {
            return new UserControl();
        }

        protected override void InitializeShell()
        {
            // no op
        }
    }
}
