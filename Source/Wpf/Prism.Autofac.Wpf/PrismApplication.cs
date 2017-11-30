using AutofacCore = Autofac.Core;
using CommonServiceLocator;
using Prism.Autofac.Ioc;
using Prism.Ioc;
using System;
using Prism.Regions;
using Prism.Unity.Regions;

namespace Prism.Autofac
{
    public abstract class PrismApplication : PrismApplicationBase
    {
        /// <summary>
        /// Creates the <see cref="IAutofacContainerExtension"/>
        /// </summary>
        /// <returns></returns>
        protected override IContainerExtension CreateContainerExtension()
        {
            return new AutofacContainerExtension();
        }

        protected override void RegisterRequiredTypes(IContainerRegistry containerRegistry)
        {
            base.RegisterRequiredTypes(containerRegistry);
            containerRegistry.RegisterSingleton<IRegionNavigationContentLoader, AutofacRegionNavigationContentLoader>();
            containerRegistry.RegisterSingleton<IServiceLocator, AutofacServiceLocatorAdapter>();
        }

        protected override void RegisterFrameworkExceptionTypes()
        {
            base.RegisterFrameworkExceptionTypes();
            ExceptionExtensions.RegisterFrameworkExceptionType(typeof(AutofacCore.DependencyResolutionException));
            ExceptionExtensions.RegisterFrameworkExceptionType(typeof(AutofacCore.Registration.ComponentNotRegisteredException));
        }
    }
}
