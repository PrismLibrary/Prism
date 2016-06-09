using Ninject;
using Ninject.Parameters;
using Ninject.Planning.Bindings.Resolvers;
using Prism.Common;
using Prism.Events;
using Prism.Logging;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Ninject.Extensions;
using Prism.Ninject.Modularity;
using Prism.Ninject.Navigation;
using Prism.Services;
using System.Linq;
using Xamarin.Forms;
using DependencyService = Prism.Services.DependencyService;

namespace Prism.Ninject
{
    public abstract class PrismApplication : PrismApplicationBase
    {
        const string _navigationServiceName = "NinjectPageNavigationService";

        IPlatformInitializer _platformInitializer;

        /// <summary>
        /// The Ninject Kernel
        /// </summary>
        public IKernel Container { get; protected set; }

        public PrismApplication(IPlatformInitializer initializer = null)
        {
            _platformInitializer = initializer;
            InitializeInternal();
        }

        public override void Initialize()
        {
            Logger = CreateLogger();

            ModuleCatalog = CreateModuleCatalog();
            ConfigureModuleCatalog();

            Container = CreateContainer();

            ConfigureContainer();

            NavigationService = CreateNavigationService();

            RegisterTypes();

            _platformInitializer?.RegisterTypes(Container);

            InitializeModules();
        }

        /// <inheritDoc />
        protected override void ConfigureViewModelLocator()
        {
            ViewModelLocationProvider.SetDefaultViewModelFactory((view, type) =>
            {
                IParameter[] overrides = null;

                var page = view as Page;
                if (page != null)
                {
                    var navService = Container.Get<INavigationService>(_navigationServiceName);
                    ((IPageAware)navService).Page = page;

                    overrides = new IParameter[]
                    {
                        new ConstructorArgument( "navigationService", navService )
                    };
                }

                return Container.Get(type, overrides);
            });
        }

        /// <summary>
        /// Override to change the creation of the Ninject kernel.
        /// If you are using <see cref="Xamarin.Forms.DependencyService"/>,
        /// you should return a <see cref="Prism.Ninject.Extensions.DependencyServiceKernel"/>.
        /// </summary>
        /// <returns>A Ninject <see cref="IKernel"/></returns>
        protected virtual IKernel CreateContainer()
        {
            return new StandardKernel();
        }

        /// <summary>
        /// Override to add your own Ninject kernel bindings. Make sure you call to base.
        /// </summary>
        protected virtual void ConfigureContainer()
        {
            Container.Components.Add<IMissingBindingResolver, DependencyServiceBindingResolver>();

            Container.Bind<ILoggerFacade>().ToConstant(Logger).InSingletonScope();
            Container.Bind<IModuleCatalog>().ToConstant(ModuleCatalog).InSingletonScope();

            Container.Bind<IApplicationProvider>().To<ApplicationProvider>().InSingletonScope();
            Container.Bind<INavigationService>().To<NinjectPageNavigationService>().Named(_navigationServiceName);
            Container.Bind<IModuleManager>().To<ModuleManager>().InSingletonScope();
            Container.Bind<IModuleInitializer>().To<NinjectModuleInitializer>().InSingletonScope();
            Container.Bind<IEventAggregator>().To<EventAggregator>().InSingletonScope();
            Container.Bind<IDependencyService>().To<DependencyService>().InSingletonScope();
            Container.Bind<IPageDialogService>().To<PageDialogService>().InSingletonScope();
        }

        protected override INavigationService CreateNavigationService()
        {
            return Container.Get<INavigationService>(_navigationServiceName);
        }

        protected override void InitializeModules()
        {
            if (ModuleCatalog.Modules.Count() > 0)
            {
                IModuleManager manager = Container.Get<IModuleManager>();
                manager.Run();
            }
        }
    }
}
