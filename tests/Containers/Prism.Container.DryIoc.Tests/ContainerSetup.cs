using System;
using DryIoc;
using Prism.DryIoc;

namespace Prism.Ioc.Tests
{
    partial class ContainerSetup
    {
        protected virtual IContainerExtension CreateContainerInternal() => new DryIocContainerExtension();

        public Type NativeContainerType => typeof(IContainer);
    }
}
