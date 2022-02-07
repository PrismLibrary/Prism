using System.Windows;
using System.Windows.Controls;
using DryIoc;
using Prism.Container.Wpf.Tests;
using Prism.DryIoc;
using Prism.Ioc;

namespace Prism.Container.Wpf.Mocks
{
    internal class MockedContainerBootstrapper : PrismBootstrapper
    {
        private readonly IContainerExtension _container;

        public MockedContainerBootstrapper(IContainerExtension container)
        {
            ContainerLocator.ResetContainer();
            this._container = container;
        }

        public MockedContainerBootstrapper(IContainer container)
            : this(ContainerHelper.CreateContainerExtension(container))
        {
        }

        bool _useDefaultConfiguration = true;

        public void Run(bool useDefaultConfiguration)
        {
            _useDefaultConfiguration = useDefaultConfiguration;

            base.Run();
        }

        protected override IContainerExtension CreateContainerExtension()
        {
            return _container;
        }

        protected override DependencyObject CreateShell()
        {
            return new UserControl();
        }

        protected override void InitializeShell(DependencyObject shell)
        {

        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }

        protected override void RegisterRequiredTypes(IContainerRegistry containerRegistry)
        {
            if (_useDefaultConfiguration)
                base.RegisterRequiredTypes(containerRegistry);
        }
    }
}
