using System;
using System.Windows;
using DryIoc;
using Prism.DryIoc;

namespace Prism.Container.Wpf.Mocks
{
    internal class NullContainerBootstrapper : DryIocBootstrapper
    {
        protected override IContainer CreateContainer()
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
