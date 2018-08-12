using CommonServiceLocator;
using DryIoc;
using Prism.DryIoc.Ioc;
using Prism.Ioc;
using Prism.Regions;
using System;

namespace Prism.DryIoc
{
    public abstract class PrismApplication : PrismApplicationBase
    {
        /// <summary>
        /// Create <see cref="Rules" /> to alter behavior of <see cref="IContainer" />
        /// </summary>
        /// <returns>An instance of <see cref="Rules" /></returns>
        protected virtual Rules CreateContainerRules() => Rules.Default.WithAutoConcreteTypeResolution()
                                                                       .With(Made.Of(FactoryMethod.ConstructorWithResolvableArguments))
                                                                       .WithDefaultIfAlreadyRegistered(IfAlreadyRegistered.Replace);

        protected override IContainerExtension CreateContainerExtension()
        {
            return new DryIocContainerExtension(new Container(CreateContainerRules()));
        }

        protected override void RegisterRequiredTypes(IContainerRegistry containerRegistry)
        {
            base.RegisterRequiredTypes(containerRegistry);
            containerRegistry.RegisterSingleton<IRegionNavigationContentLoader, RegionNavigationContentLoader>();
            containerRegistry.RegisterSingleton<IServiceLocator, DryIocServiceLocatorAdapter>();
        }

        protected override void RegisterFrameworkExceptionTypes()
        {
            base.RegisterFrameworkExceptionTypes();
            ExceptionExtensions.RegisterFrameworkExceptionType(typeof(ContainerException));
        }
    }
}
