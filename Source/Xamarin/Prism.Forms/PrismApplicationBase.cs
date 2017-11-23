using Prism.AppModel;
using Prism.Behaviors;
using Prism.Common;
using Prism.Events;
using Prism.Ioc;
using Prism.Logging;
using Prism.Modularity;
using Prism.Navigation;
using Prism.Services;
using System.Linq;
using Xamarin.Forms;
using DependencyService = Prism.Services.DependencyService;

namespace Prism
{
    public abstract class PrismApplicationBase<TContainer> : Application
    {
        const string _navigationServiceName = "PageNavigationService";
        IContainerExtension<TContainer> _containerExtension;
        IPlatformInitializer _platformInitializer;
        IModuleCatalog _moduleCatalog;
        Page _previousPage = null;        

        /// <summary>
        /// The dependency injection container used to resolve objects
        /// </summary>
        public IContainerProvider<TContainer> Container => _containerExtension;

        /// <summary>
        /// Gets the <see cref="INavigationService"/> for the application.
        /// </summary>
        protected INavigationService NavigationService { get; set; }

        protected PrismApplicationBase(IPlatformInitializer initializer = null)
        {
            base.ModalPopping += PrismApplicationBase_ModalPopping;
            base.ModalPopped += PrismApplicationBase_ModalPopped;

            _platformInitializer = initializer;
            InitializeInternal();
        }

        /// <summary>
        /// Run the initialization process.
        /// </summary>
        void InitializeInternal()
        {
            ConfigureViewModelLocator();
            Initialize();
            OnInitialized();
        }

        /// <summary>
        /// Configures the <see cref="Prism.Mvvm.ViewModelLocator"/> used by Prism.
        /// </summary>
        protected abstract void ConfigureViewModelLocator();

        /// <summary>
        /// Run the bootstrapper process.
        /// </summary>
        public virtual void Initialize()
        {
            _containerExtension = CreateContainerExtension();
            ConfigureContainer(_containerExtension);
            _platformInitializer?.RegisterTypes(_containerExtension);
            RegisterTypes(_containerExtension);

            _moduleCatalog = Container.Resolve<IModuleCatalog>();
            ConfigureModuleCatalog(_moduleCatalog);

            NavigationService = CreateNavigationService(null);

            InitializeModules();
        }

        /// <summary>
        /// Creates the container used by Prism.
        /// </summary>
        /// <returns>The container</returns>
        protected abstract IContainerExtension<TContainer> CreateContainerExtension();

        protected virtual void ConfigureContainer(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterInstance<IContainerExtension>(_containerExtension);
            containerRegistry.RegisterSingleton<ILoggerFacade, EmptyLogger>();
            containerRegistry.RegisterSingleton<IModuleCatalog, ModuleCatalog>();
            containerRegistry.RegisterSingleton<IApplicationProvider, ApplicationProvider>();
            containerRegistry.RegisterSingleton<IApplicationStore, ApplicationStore>();
            containerRegistry.RegisterSingleton<IModuleManager, ModuleManager>();
            containerRegistry.RegisterSingleton<IModuleInitializer, ModuleInitializer>();
            containerRegistry.RegisterSingleton<IEventAggregator, EventAggregator>();
            containerRegistry.RegisterSingleton<IDependencyService, DependencyService>();
            containerRegistry.RegisterSingleton<IPageDialogService, PageDialogService>();
            containerRegistry.RegisterSingleton<IDeviceService, DeviceService>();
            containerRegistry.RegisterSingleton<IPageBehaviorFactory, PageBehaviorFactory>();
            containerRegistry.RegisterType<INavigationService, PageNavigationService>(_navigationServiceName);
        }

        /// <summary>
        /// Used to register types with the container that will be used by your application.
        /// </summary>
        protected abstract void RegisterTypes(IContainerRegistry containerRegistry);

        /// <summary>
        /// Configures the <see cref="IModuleCatalog"/> used by Prism.
        /// </summary>
        /// <param name="moduleCatalog">The ModuleCatalog to configure</param>
        protected virtual void ConfigureModuleCatalog(IModuleCatalog moduleCatalog) { }

        /// <summary>
        /// Create instance of <see cref="INavigationService"/> and set the <see cref="IPageAware.Page"/> property to <paramref name="page"/>
        /// </summary>
        /// <param name="page">Active page</param>
        /// <returns>Instance of <see cref="INavigationService"/> with <see cref="IPageAware.Page"/> set</returns>
        protected INavigationService CreateNavigationService(Page page)
        {
            var navigationService = Container.Resolve<INavigationService>(_navigationServiceName);
            ((IPageAware)navigationService).Page = page;
            return navigationService;
        }

        /// <summary>
        /// Initializes the modules.
        /// </summary>
        protected virtual void InitializeModules()
        {
            if (_moduleCatalog.Modules.Count() > 0)
            {
                IModuleManager manager = Container.Resolve<IModuleManager>();
                manager.Run();
            }
        }

        /// <summary>
        /// Called when the PrismApplication has completed it's initialization process.
        /// </summary>
        protected abstract void OnInitialized();

        protected override void OnResume()
        {
            var page = PageUtilities.GetCurrentPage(MainPage);
            PageUtilities.InvokeViewAndViewModelAction<AppModel.IApplicationLifecycleAware>(page, x => x.OnResume());
        }

        protected override void OnSleep()
        {
            var page = PageUtilities.GetCurrentPage(MainPage);
            PageUtilities.InvokeViewAndViewModelAction<AppModel.IApplicationLifecycleAware>(page, x => x.OnSleep());
        }

        private void PrismApplicationBase_ModalPopping(object sender, ModalPoppingEventArgs e)
        {
            if (PageNavigationService.NavigationSource == PageNavigationSource.Device)
            {
                _previousPage = PageUtilities.GetOnNavigatedToTarget(e.Modal, MainPage, true);
            }
        }

        private void PrismApplicationBase_ModalPopped(object sender, ModalPoppedEventArgs e)
        {
            if (PageNavigationService.NavigationSource == PageNavigationSource.Device)
            {
                PageUtilities.HandleSystemGoBack(e.Modal, _previousPage);
                _previousPage = null;
            }
        }
    }
}
