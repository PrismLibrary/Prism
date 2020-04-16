using System;
using System.Windows;
using Prism.Unity;
using Unity;

namespace Prism.Container.Wpf.Mocks
{
    internal class NullContainerBootstrapper : UnityBootstrapper
    {
        protected override IUnityContainer CreateContainer()
        {
            return null;
        }
        protected override DependencyObject CreateShell()
        {
            throw new NotImplementedException();
        }

        protected override void InitializeShell()
        {
            throw new NotImplementedException();
        }
    }
}
