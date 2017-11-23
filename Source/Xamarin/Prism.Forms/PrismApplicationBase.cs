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
    public abstract class PrismApplicationBase : Application
    {
        const string _navigationServiceName = "PageNavigationService";
        IPlatformInitializer _platformInitializer;
        IModuleCatalog _moduleCatalog;
        Page _previousPage = null;

        /// <summary>
        /// The dependency injection container used to resolve objects
        /// </summary>
        public IContainer Container { get; protected set; }

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
            Container = CreateContainer();
            ConfigureContainer();
            _platformInitializer?.RegisterTypes(Container);
            RegisterTypes();

            _moduleCatalog = Container.Resolve<IModuleCatalog>();
            ConfigureModuleCatalog(_moduleCatalog);

            NavigationService = CreateNavigationService(null);

            InitializeModules();
        }

        /// <summary>
        /// Creates the container used by Prism.
        /// </summary>
        /// <returns>The container</returns>
        protected abstract IContainer CreateContainer();

        protected virtual void ConfigureContainer()
        {
            Container.RegisterInstance<IContainer>(Container);
            Container.RegisterSingleton<ILoggerFacade, EmptyLogger>();
            Container.RegisterSingleton<IModuleCatalog, ModuleCatalog>();
            Container.RegisterSingleton<IApplicationProvider, ApplicationProvider>();
            Container.RegisterSingleton<IApplicationStore, ApplicationStore>();
            Container.RegisterSingleton<IModuleManager, ModuleManager>();
            Container.RegisterSingleton<IModuleInitializer, ModuleInitializer>();
            Container.RegisterSingleton<IEventAggregator, EventAggregator>();
            Container.RegisterSingleton<IDependencyService, DependencyService>();
            Container.RegisterSingleton<IPageDialogService, PageDialogService>();
            Container.RegisterSingleton<IDeviceService, DeviceService>();
            Container.RegisterSingleton<IPageBehaviorFactory, PageBehaviorFactory>();
            Container.RegisterType<INavigationService, PageNavigationService>(_navigationServiceName);
        }

        /// <summary>
        /// Used to register types with the container that will be used by your application.
        /// </summary>
        protected abstract void RegisterTypes();

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
