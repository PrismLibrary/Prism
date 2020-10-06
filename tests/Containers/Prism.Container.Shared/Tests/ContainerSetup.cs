using System;
using Prism.Ioc;
using Prism.Mvvm;

namespace Prism.Ioc.Tests
{
    public partial class ContainerSetup : IDisposable
    {
        public ContainerSetup()
        {
            ViewModelLocationProvider.SetDefaultViewModelFactory((view, type) => Container.Resolve(type));
        }

        public IContainerProvider Container => ContainerLocator.Container;

        public IContainerExtension Extension => ContainerLocator.Current;

        public IContainerRegistry Registry => Extension;

        public IContainerProvider CreateContainer()
        {
            ContainerLocator.ResetContainer();
            ContainerLocator.SetContainerExtension(() => CreateContainerInternal());
            var container = ContainerLocator.Current;
            container.CreateScope();
            return container;
        }

        private bool _disposed;

        private void Dispose(bool disposing)
        {
            _disposed = true;
            ContainerLocator.ResetContainer();
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
        }
    }
}
