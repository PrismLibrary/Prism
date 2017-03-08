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
using Xamarin.Forms;
using DependencyService = Prism.Services.DependencyService;

namespace Prism.Ninject
{
    public abstract class PrismApplication : PrismApplicationBase<IKernel>
    {
        const string _navigationServiceName = "NinjectPageNavigationService";

        public PrismApplication(IPlatformInitializer initializer = null) : base(initializer) { }

        protected override void ConfigureViewModelLocator()
        {
            ViewModelLocationProvider.SetDefaultViewModelFactory((view, type) =>
            {
                IParameter[] overrides = null;

                var page = view as Page;
                if (page != null)
                {
                    var navService = CreateNavigationService();
                    ((IPageAware)navService).Page = page;

                    overrides = new IParameter[]
                    {
                        new ConstructorArgument( "navigationService", navService )
                    };
                }

                return Container.Get(type, overrides);
            });
        }

        protected override IKernel CreateContainer()
        {
            return new StandardKernel();
        }

        protected override IModuleManager CreateModuleManager()
        {
            return Container.Get<IModuleManager>();
        }

        protected override INavigationService CreateNavigationService()
        {
            return Container.Get<INavigationService>(_navigationServiceName);
        }

        protected override void ConfigureContainer()
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
            Container.Bind<IDeviceService>().To<DeviceService>().InSingletonScope();
        }
    }
}
