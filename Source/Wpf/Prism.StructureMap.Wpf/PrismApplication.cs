using CommonServiceLocator;
using Prism.Ioc;
using Prism.Regions;
using Prism.StructureMap.Ioc;
using StructureMap;
using System;

namespace Prism.StructureMap
{
    public abstract class PrismApplication : PrismApplicationBase
    {
        protected override IContainerExtension CreateContainerExtension()
        {
            return new StructureMapContainerExtension();
        }

        protected override void RegisterRequiredTypes(IContainerRegistry containerRegistry)
        {
            base.RegisterRequiredTypes(containerRegistry);
            containerRegistry.RegisterSingleton<IRegionNavigationContentLoader, RegionNavigationContentLoader>();
            containerRegistry.RegisterSingleton<IServiceLocator, StructureMapServiceLocatorAdapter>();
        }

        protected override void RegisterFrameworkExceptionTypes()
        {
            base.RegisterFrameworkExceptionTypes();
            ExceptionExtensions.RegisterFrameworkExceptionType(typeof(StructureMapBuildPlanException));
            ExceptionExtensions.RegisterFrameworkExceptionType(typeof(StructureMapConfigurationException));
            ExceptionExtensions.RegisterFrameworkExceptionType(typeof(StructureMapException));
        }
    }
}
