using Grace.DependencyInjection;
using Prism.AppModel;
using Prism.Behaviors;
using Prism.Common;
using Prism.Events;
using Prism.Logging;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Xamarin.Forms;
using DependencyService = Prism.Services.DependencyService;
using Prism.Grace.Navigation;
using Prism.Grace.Modularity;

namespace Prism.Grace
{
    public abstract class PrismApplication : PrismApplicationBase<DependencyInjectionContainer>
    {
        const string _navigationServiceName = "GracePageNavigationService";

        public PrismApplication(IPlatformInitializer initializer = null) : base(initializer) { }

        protected override void ConfigureViewModelLocator()
        {
            ViewModelLocationProvider.SetDefaultViewModelFactory((view, type) =>
            {
                var page = view as Page;

                if (page != null)
                {
                    return this.Container.Locate(type, new { navigationService = CreateNavigationService(page) });
                }

                return Container.Locate(type);
            });
        }

        protected override DependencyInjectionContainer CreateContainer()
        {
            return new DependencyInjectionContainer();
        }

        protected override IModuleManager CreateModuleManager()
        {
            return Container.Locate<IModuleManager>();
        }

        protected override INavigationService CreateNavigationService()
        {
            return Container.Locate<INavigationService>(withKey: _navigationServiceName);
        }

        protected override void ConfigureContainer()
        {
            Container.Configure(c => 
            {
                c.ExportInstance<DependencyInjectionContainer>(Container);
                c.ExportInstance<ILoggerFacade>(Logger);
                c.ExportInstance<IModuleCatalog>(ModuleCatalog);

                c.ExportAs<ApplicationProvider, IApplicationProvider>().Lifestyle.Singleton();
                c.ExportAs<ApplicationStore, IApplicationStore>().Lifestyle.Singleton();
                c.Export<GracePageNavigationService>().AsKeyed<INavigationService>(_navigationServiceName);
                c.ExportAs<ModuleManager, IModuleManager>().Lifestyle.Singleton();
                c.ExportAs<GraceModuleInitializer, IModuleInitializer>().Lifestyle.Singleton();
                c.ExportAs<EventAggregator, IEventAggregator>().Lifestyle.Singleton();
                c.ExportAs<DependencyService, IDependencyService>().Lifestyle.Singleton();
                c.ExportAs<PageDialogService, IPageDialogService>().Lifestyle.Singleton();
                c.ExportAs<DeviceService, IDeviceService>().Lifestyle.Singleton();
                c.ExportAs<PageBehaviorFactory, IPageBehaviorFactory>().Lifestyle.Singleton();
            });
        }
    }
}