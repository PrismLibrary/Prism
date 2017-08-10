using System.Linq;
using Autofac;
using Autofac.Features.ResolveAnything;
using Prism.AppModel;
using Prism.Autofac.Modularity;
using Prism.Autofac.Navigation;
using Prism.Common;
using Prism.Events;
using Prism.Logging;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Xamarin.Forms;
using DependencyService = Prism.Services.DependencyService;

namespace Prism.Autofac
{
    /// <summary>
    /// Application base class using Autofac
    /// </summary>
    public abstract class PrismApplication : PrismApplicationBase<ContainerBuilder>
    {
        /// <summary>
        /// Service key used when registering the <see cref="AutofacPageNavigationService"/> with the container
        /// </summary>
        const string _navigationServiceName = "AutofacPageNavigationService";

        /// <summary>
        /// Create a new instance of <see cref="PrismApplication"/>
        /// </summary>
        /// <param name="platformInitializer">Class to initialize platform instances</param>
        /// <remarks>
        /// The method <see cref="M:IPlatformInitializer.RegisterTypes(ContainerBuilder)"/> will be called after <see cref="M:PrismApplication.RegisterTypes()"/> 
        /// to allow for registering platform specific instances.
        /// </remarks>
        protected PrismApplication(IPlatformInitializer platformInitializer = null)
            : base(platformInitializer)
        {
        }

        /// <summary>
        ///  Gets or Sets the Autofac Container Builder
        /// </summary>
        protected ContainerBuilder Builder
        {
            get => base.Container;
            set => base.Container = value;
        }

        /// <summary>
        /// Gets or Sets the Autofac IContainer
        /// </summary>
        public new IContainer Container { get; set; }

        /// <summary>
        /// Creates the Container Builder
        /// </summary>
        /// <returns></returns>
        protected virtual ContainerBuilder CreateBuilder() => 
            new ContainerBuilder();

        /// <summary>
        /// Run the bootstrapper process.
        /// </summary>
        public override void Initialize()
        {
            Logger = CreateLogger();

            ModuleCatalog = CreateModuleCatalog();
            ConfigureModuleCatalog();

            Builder = CreateBuilder();

            ConfigureContainer();

            RegisterTypes();

            PlatformInitializer?.RegisterTypes(Builder);

            FinishContainerConfiguration();
            Container = Builder.Build();

            NavigationService = CreateNavigationService();

            InitializeModules();
        }

        /// <summary>
        /// Configures the ViewModel Locator to resolve the ViewModel type and ensure the correct
        /// instance of <see cref="INavigationService"/> is properly injected into the ViewModel.
        /// </summary>
        protected override void ConfigureViewModelLocator()
        {
            ViewModelLocationProvider.SetDefaultViewModelFactory((view, type) =>
            {
                var navService = CreateNavigationService(view);
                if (navService != null)
                {
                    return Container.Resolve(type, new NamedParameter("navigationService", navService));
                }

                return Container.Resolve(type);
            });
        }

        /// <summary>
        /// This is not used for Autofac and will throw a <see cref="System.NotImplementedException"/>
        /// </summary>
        /// <returns></returns>
        protected override ContainerBuilder CreateContainer()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Creates the <see cref="IModuleManager"/> from the container.
        /// </summary>
        /// <returns></returns>
        protected override IModuleManager CreateModuleManager() =>
            Container.Resolve<IModuleManager>();

        /// <summary>
        /// Create instance of <see cref="INavigationService"/>
        /// </summary>
        /// <remarks>
        /// The Autofac Navigation Service Name is used when resolving
        /// </remarks>
        /// <returns>Instance of <see cref="INavigationService"/></returns>
        protected override INavigationService CreateNavigationService()
        {
            return Container.ResolveNamed<INavigationService>(_navigationServiceName);
        }

        /// <summary>
        /// Registers all of the base Prism Services.
        /// </summary>
        protected override void ConfigureContainer()
        {
            Builder.RegisterInstance(Logger).As<ILoggerFacade>().SingleInstance();
            Builder.RegisterInstance(ModuleCatalog).As<IModuleCatalog>().SingleInstance();

            Builder.RegisterType<ApplicationProvider>().As<IApplicationProvider>().SingleInstance();
            Builder.RegisterType<ApplicationStore>().As<IApplicationStore>().SingleInstance();
            Builder.RegisterType<AutofacPageNavigationService>().Named<INavigationService>(_navigationServiceName);
            Builder.RegisterType<ModuleManager>().As<IModuleManager>().SingleInstance();
            Builder.RegisterType<AutofacModuleInitializer>().As<IModuleInitializer>().SingleInstance();
            Builder.RegisterType<EventAggregator>().As<IEventAggregator>().SingleInstance();
            Builder.RegisterType<DependencyService>().As<IDependencyService>().SingleInstance();
            Builder.RegisterType<PageDialogService>().As<IPageDialogService>().SingleInstance();
            Builder.RegisterType<DeviceService>().As<IDeviceService>().SingleInstance();
        }

        /// <summary>
        /// Finish the container's configuration after all other types are registered.
        /// </summary>
        protected virtual void FinishContainerConfiguration()
        {
            // Make sure any not specifically registered concrete type can resolve.
            Builder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());
        }
    }
}
