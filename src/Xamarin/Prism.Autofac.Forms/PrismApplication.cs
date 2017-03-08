using System.Linq;
using Prism.Navigation;
using Prism.Mvvm;
using Prism.Common;
using Xamarin.Forms;
using Prism.Logging;
using Prism.Events;
using Prism.Services;
using DependencyService = Prism.Services.DependencyService;
using Prism.Modularity;
using Autofac;
using Autofac.Features.ResolveAnything;
using Prism.Autofac.Forms.Modularity;
using Prism.Autofac.Navigation;
using Prism.Autofac.Forms;

namespace Prism.Autofac
{
    /// <summary>
    /// Application base class using Autofac
    /// </summary>
    public abstract class PrismApplication : PrismApplicationBase<IContainer>
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
        /// The method <see cref="IPlatformInitializer.RegisterTypes(IContainer)"/> will be called after <see cref="PrismApplication.RegisterTypes()"/> 
        /// to allow for registering platform specific instances.
        /// </remarks>
        protected PrismApplication(IPlatformInitializer initializer = null)
            : base(initializer)
        {
        }

        public override void Initialize()
        {
            base.Initialize();

            FinishContainerConfiguration();
        }

        protected override void ConfigureViewModelLocator()
        {
            ViewModelLocationProvider.SetDefaultViewModelFactory((view, type) =>
            {
                NamedParameter parameter = null;
                var page = view as Page;
                if (page != null)
                {
                    var navService = CreateNavigationService();
                    ((IPageAware)navService).Page = page;

                    parameter = new NamedParameter("navigationService", navService);
                }

                return Container.Resolve(type, parameter);
            });
        }

        /// <summary>
        /// Create a default instance of <see cref="IContainer" />
        /// </summary>
        /// <returns>An instance of <see cref="IContainer" /></returns>
        protected override IContainer CreateContainer()
        {
            return new ContainerBuilder().Build();
        }

        protected override IModuleManager CreateModuleManager()
        {
            return Container.Resolve<IModuleManager>();
        }

        /// <summary>
        /// Create instance of <see cref="INavigationService"/>
        /// </summary>
        /// <remarks>
        /// The <see cref="_navigationServiceKey"/> is used as service key when resolving
        /// </remarks>
        /// <returns>Instance of <see cref="INavigationService"/></returns>
        protected override INavigationService CreateNavigationService()
        {
            return Container.ResolveNamed<INavigationService>(_navigationServiceName);
        }

        protected override void InitializeModules()
        {
            if (ModuleCatalog.Modules.Any())
            {
                var manager = Container.Resolve<IModuleManager>();
                manager.Run();
            }
        }

        protected override void ConfigureContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterInstance(Logger).As<ILoggerFacade>().SingleInstance();
            builder.RegisterInstance(ModuleCatalog).As<IModuleCatalog>().SingleInstance();

            builder.Register(ctx => new ApplicationProvider()).As<IApplicationProvider>().SingleInstance();
            builder.Register(ctx => new AutofacPageNavigationService(Container, Container.Resolve<IApplicationProvider>(), Container.Resolve<ILoggerFacade>())).Named<INavigationService>(_navigationServiceName);
            builder.Register(ctx => new ModuleManager(Container.Resolve<IModuleInitializer>(), Container.Resolve<IModuleCatalog>())).As<IModuleManager>().SingleInstance();
            builder.Register(ctx => new AutofacModuleInitializer(Container)).As<IModuleInitializer>().SingleInstance();
            builder.Register(ctx => new EventAggregator()).As<IEventAggregator>().SingleInstance();
            builder.Register(ctx => new DependencyService()).As<IDependencyService>().SingleInstance();
            builder.Register(ctx => new PageDialogService(ctx.Resolve<IApplicationProvider>())).As<IPageDialogService>().SingleInstance();
            builder.Register(ctx => new DeviceService()).As<IDeviceService>().SingleInstance();

            builder.Update(Container);
        }

        /// <summary>
        /// Finish the container's configuration after all other types are registered.
        /// </summary>
        private void FinishContainerConfiguration()
        {
            var containerUpdater = new ContainerBuilder();

            // Make sure any not specifically registered concrete type can resolve.
            containerUpdater.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());

            containerUpdater.Update(Container);
        }
    }
}
