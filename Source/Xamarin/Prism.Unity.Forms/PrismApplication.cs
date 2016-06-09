using System.Linq;
using Prism.Navigation;
using Microsoft.Practices.Unity;
using Prism.Mvvm;
using Prism.Unity.Navigation;
using Prism.Common;
using Xamarin.Forms;
using Prism.Unity.Extensions;
using Prism.Logging;
using Prism.Events;
using Prism.Services;
using DependencyService = Prism.Services.DependencyService;
using Prism.Modularity;
using Prism.Unity.Modularity;

namespace Prism.Unity
{
    public abstract class PrismApplication : PrismApplicationBase
    {
        const string _navigationServiceName = "UnityPageNavigationService";

        IPlatformInitializer _platformInitializer;

        public IUnityContainer Container { get; protected set; }

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

        protected override void ConfigureViewModelLocator()
        {
            ViewModelLocationProvider.SetDefaultViewModelFactory((view, type) =>
            {
                ParameterOverrides overrides = null;

                var page = view as Page;
                if (page != null)
                {
                    var navService = Container.Resolve<INavigationService>(_navigationServiceName);
                    ((IPageAware)navService).Page = page;

                    overrides = new ParameterOverrides
                    {
                        { "navigationService", navService }
                    };
                }

                return Container.Resolve(type, overrides);
            });
        }

        protected virtual IUnityContainer CreateContainer()
        {
            return new UnityContainer();
        }

        protected override INavigationService CreateNavigationService()
        {
            return Container.Resolve<INavigationService>(_navigationServiceName);
        }

        protected virtual void ConfigureContainer()
        {
            Container.AddNewExtension<DependencyServiceExtension>();

            Container.RegisterInstance<ILoggerFacade>(Logger);
            Container.RegisterInstance<IModuleCatalog>(ModuleCatalog);

            Container.RegisterType<IApplicationProvider, ApplicationProvider>(new ContainerControlledLifetimeManager());
            Container.RegisterType<INavigationService, UnityPageNavigationService>(_navigationServiceName);
            Container.RegisterType<IModuleManager, ModuleManager>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IModuleInitializer, UnityModuleInitializer>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IEventAggregator, EventAggregator>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IDependencyService, DependencyService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IPageDialogService, PageDialogService>(new ContainerControlledLifetimeManager());
        }

        protected override void InitializeModules()
        {
            if (ModuleCatalog.Modules.Count() > 0)
            {
                IModuleManager manager = Container.Resolve<IModuleManager>();
                manager.Run();
            }
        }
    }
}
