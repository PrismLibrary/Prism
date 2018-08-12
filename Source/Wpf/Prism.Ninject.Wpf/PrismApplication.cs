using CommonServiceLocator;
using Prism.Ioc;
using Prism.Ninject.Ioc;
using Prism.Ninject.Regions;
using Prism.Regions;
using System;

namespace Prism.Ninject
{
    public abstract class PrismApplication : PrismApplicationBase
    {
        protected override IContainerExtension CreateContainerExtension()
        {
            return new NinjectContainerExtension();
        }

        protected override void RegisterRequiredTypes(IContainerRegistry containerRegistry)
        {
            base.RegisterRequiredTypes(containerRegistry);
            containerRegistry.RegisterSingleton<IRegionNavigationContentLoader, NinjectRegionNavigationContentLoader>();
            containerRegistry.RegisterSingleton<IServiceLocator, NinjectServiceLocatorAdapter>();
        }

        protected override void RegisterFrameworkExceptionTypes()
        {
            base.RegisterFrameworkExceptionTypes();
            ExceptionExtensions.RegisterFrameworkExceptionType(typeof(global::Ninject.ActivationException));
        }
    }
}
