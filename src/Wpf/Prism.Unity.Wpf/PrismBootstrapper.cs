using Prism.Ioc;
using System;
using Unity;

namespace Prism.Unity
{
    public abstract class PrismBootstrapper : PrismBootstrapperBase
    {
        protected override IContainerExtension CreateContainerExtension()
        {
            return new UnityContainerExtension();
        }

        protected override void RegisterFrameworkExceptionTypes()
        {
            ExceptionExtensions.RegisterFrameworkExceptionType(typeof(ResolutionFailedException));
        }
    }
}
