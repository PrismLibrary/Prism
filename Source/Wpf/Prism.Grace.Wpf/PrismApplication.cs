using CommonServiceLocator;
using Grace.DependencyInjection.Exceptions;
using Prism.Grace.Ioc;
using Prism.Grace.Regions;
using Prism.Ioc;
using Prism.Regions;
using System;

namespace Prism.Grace
{
    public abstract class PrismApplication : PrismApplicationBase
    {
        protected override IContainerExtension CreateContainerExtension()
        {
            return new GraceContainerExtension();
        }

        protected override void RegisterRequiredTypes(IContainerRegistry containerRegistry)
        {
            base.RegisterRequiredTypes(containerRegistry);
            containerRegistry.RegisterSingleton<IRegionNavigationContentLoader, GraceRegionNavigationContentLoader>();
            containerRegistry.RegisterSingleton<IServiceLocator, GraceServiceLocatorAdapter>();
        }

        protected override void RegisterFrameworkExceptionTypes()
        {
            base.RegisterFrameworkExceptionTypes();
            ExceptionExtensions.RegisterFrameworkExceptionType(typeof(LocateException));
        }
    }
}
