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
        IServiceLocator _serviceLocator;

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

            _serviceLocator = new AutofacServiceLocatorAdapter();
            containerRegistry.RegisterInstance<IServiceLocator>(_serviceLocator);
        }

        protected override void RegisterFrameworkExceptionTypes()
        {
            base.RegisterFrameworkExceptionTypes();
            ExceptionExtensions.RegisterFrameworkExceptionType(typeof(AutofacCore.DependencyResolutionException));
            ExceptionExtensions.RegisterFrameworkExceptionType(typeof(AutofacCore.Registration.ComponentNotRegisteredException));
        }

        protected override void ConfigureServiceLocator()
        {
            ((AutofacServiceLocatorAdapter)_serviceLocator).Container = Container.GetContainer();

            ServiceLocator.SetLocatorProvider(() => _serviceLocator);
        }
    }
}
