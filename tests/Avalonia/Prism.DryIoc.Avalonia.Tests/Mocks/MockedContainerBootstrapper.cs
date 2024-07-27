using Avalonia;
using Avalonia.Controls;
using DryIoc;
using Prism.Container.DryIoc;
using Prism.DryIoc;
using Prism.Ioc;

namespace Prism.Container.Avalonia.Mocks
{
    internal class MockedContainerBootstrapper : PrismBootstrapper
    {
        private readonly IContainer _container;

        public MockedContainerBootstrapper(IContainer container)
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
            return new DryIocContainerExtension(_container);
        }

        protected override AvaloniaObject CreateShell()
        {
            return new UserControl();
        }

        protected override void InitializeShell(AvaloniaObject shell)
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
