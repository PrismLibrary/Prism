using System.Windows;
using System.Windows.Controls;
using Prism.Ioc;
using Prism.Unity;
using Unity;

namespace Prism.Container.Wpf.Mocks
{
    internal class MockedContainerBootstrapper : PrismBootstrapper
    {
        private readonly IUnityContainer _container;

        public MockedContainerBootstrapper(IUnityContainer container)
        {
            ContainerLocator.ResetContainer();
            this._container = container;
        }

        bool _useDefaultConfiguration = true;

        public void Run(bool useDefaultConfiguration)
        {
            _useDefaultConfiguration = useDefaultConfiguration;

            base.Run();
        }

        protected override IContainerExtension CreateContainerExtension()
        {
            return new UnityContainerExtension(_container);
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
