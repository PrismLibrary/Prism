using System;
using Prism.Ioc;
using Prism.Unity;
using Unity;

namespace Prism.Ioc.Tests
{
    partial class ContainerSetup
    {
        IContainerExtension CreateContainerInternal() => new UnityContainerExtension();

        public Type NativeContainerType => typeof(IUnityContainer);
    }
}
