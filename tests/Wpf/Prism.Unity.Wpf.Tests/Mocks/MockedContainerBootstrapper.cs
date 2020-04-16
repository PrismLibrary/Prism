using System.Windows;
using System.Windows.Controls;
using Prism.Ioc;
using Prism.Logging;
using Prism.Unity;
using Unity;

namespace Prism.Container.Wpf.Mocks
{
    internal class MockedContainerBootstrapper : UnityBootstrapper
    {
        private readonly IUnityContainer container;
        public ILoggerFacade BaseLogger => base.Logger;

        public void CallConfigureContainer()
        {
            base.ConfigureContainer();
        }

        public MockedContainerBootstrapper(IUnityContainer container)
        {
            ContainerLocator.ResetContainer();
            this.container = container;
        }

        protected override IUnityContainer CreateContainer()
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
