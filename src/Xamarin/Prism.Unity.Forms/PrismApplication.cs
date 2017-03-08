using Microsoft.Practices.Unity;
using Prism.Common;
using Prism.Events;
using Prism.Logging;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Prism.Unity.Extensions;
using Prism.Unity.Modularity;
using Prism.Unity.Navigation;
using Xamarin.Forms;
using DependencyService = Prism.Services.DependencyService;

namespace Prism.Unity
{
    public abstract class PrismApplication : PrismApplicationBase<IUnityContainer>
    {
        const string _navigationServiceName = "UnityPageNavigationService";

        public PrismApplication(IPlatformInitializer initializer = null) : base (initializer) { }

        protected override void ConfigureViewModelLocator()
        {
            ViewModelLocationProvider.SetDefaultViewModelFactory((view, type) =>
            {
                ParameterOverrides overrides = null;

                var page = view as Page;
                if (page != null)
                {
                    var navService = CreateNavigationService();
                    ((IPageAware)navService).Page = page;

                    overrides = new ParameterOverrides
                    {
                        { "navigationService", navService }
                    };
                }

                return Container.Resolve(type, overrides);
            });
        }

        protected override IUnityContainer CreateContainer()
        {
            return new UnityContainer();
        }

        protected override IModuleManager CreateModuleManager()
        {
            return Container.Resolve<IModuleManager>();
        }

        protected override INavigationService CreateNavigationService()
        {
            return Container.Resolve<INavigationService>(_navigationServiceName);
        }

        protected override void ConfigureContainer()
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
            Container.RegisterType<IDeviceService, DeviceService>(new ContainerControlledLifetimeManager());
        }
    }
}
