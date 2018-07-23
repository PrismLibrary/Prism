using Prism.AppModel;
using Prism.Behaviors;
using Prism.Common;
using Prism.Events;
using Prism.Ioc;
using Prism.Logging;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using DependencyService = Prism.Services.DependencyService;

namespace Prism
{
    public abstract class PrismApplicationBase : Application
    {
        public new static PrismApplicationBase Current => (PrismApplicationBase) Application.Current;
        public const string NavigationServiceName = "PageNavigationService";
        public const string NavigationServiceParameterName = "navigationService";
        IContainerExtension _containerExtension;
        IModuleCatalog _moduleCatalog;
        Page _previousPage = null;
        bool _setFormsDependencyResolver { get; }

        /// <summary>
        /// The dependency injection container used to resolve objects
        /// </summary>
        public IContainerProvider Container => _containerExtension;

        /// <summary>
        /// Gets the <see cref="INavigationService"/> for the application.
        /// </summary>
        protected INavigationService NavigationService { get; set; }

        /// <summary>
        /// Get the Platform Initializer
        /// </summary>
        protected IPlatformInitializer PlatformInitializer { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="PrismApplicationBase" /> using the default constructor
        /// </summary>
        protected PrismApplicationBase() : this(null, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="PrismApplicationBase" /> with a <see cref="IPlatformInitializer" />.
        /// Used when there are specific types that need to be registered on the platform.
        /// </summary>
        /// <param name="platformInitializer">The <see cref="IPlatformInitializer"/>.</param>
        protected PrismApplicationBase(IPlatformInitializer platformInitializer) : this(platformInitializer, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="PrismApplicationBase" /> with a <see cref="IPlatformInitializer" />.
        /// Used when there are specific types that need to be registered on the platform.
        /// Also determines whether to set the <see cref="DependencyResolver" /> for resolving Renderers and Platform Effects.
        /// </summary>
        /// <param name="platformInitializer">The <see cref="IPlatformInitializer"/>.</param>
        /// <param name="setFormsDependencyResolver">Should <see cref="PrismApplicationBase" /> set the <see cref="DependencyResolver" />.</param>
        protected PrismApplicationBase(IPlatformInitializer platformInitializer, bool setFormsDependencyResolver)
        {
            base.ModalPopping += PrismApplicationBase_ModalPopping;
            base.ModalPopped += PrismApplicationBase_ModalPopped;
            _setFormsDependencyResolver = setFormsDependencyResolver;

            PlatformInitializer = platformInitializer;
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
        /// Configures the <see cref="ViewModelLocator"/> used by Prism.
        /// </summary>
        protected virtual void ConfigureViewModelLocator()
        {
            ViewModelLocationProvider.SetDefaultViewModelFactory((view, type) =>
            {
                return _containerExtension.ResolveViewModelForView(view, type);
            });
        }

        /// <summary>
        /// Run the bootstrapper process.
        /// </summary>
        public virtual void Initialize()
        {
            _containerExtension = CreateContainerExtension();
            RegisterRequiredTypes(_containerExtension);
            PlatformInitializer?.RegisterTypes(_containerExtension);
            RegisterTypes(_containerExtension);
            _containerExtension.FinalizeExtension();

            if(_setFormsDependencyResolver)
                SetDependencyResolver(_containerExtension);

            _moduleCatalog = Container.Resolve<IModuleCatalog>();
            ConfigureModuleCatalog(_moduleCatalog);

            NavigationService = _containerExtension.CreateNavigationService(null);

            InitializeModules();
        }

        /// <summary>
        /// Sets the <see cref="DependencyResolver" /> to use the App Container for resolving types
        /// </summary>
        protected virtual void SetDependencyResolver(IContainerProvider containerProvider) =>
            DependencyResolver.ResolveUsing(type => containerProvider.Resolve(type));


        /// <summary>
        /// Creates the container used by Prism.
        /// </summary>
        /// <returns>The container</returns>
        protected abstract IContainerExtension CreateContainerExtension();

        /// <summary>
        /// Registers all types that are required by Prism to function with the container.
        /// </summary>
        /// <param name="containerRegistry"></param>
        protected virtual void RegisterRequiredTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterInstance<IContainerExtension>(_containerExtension);
            containerRegistry.RegisterSingleton<ILoggerFacade, EmptyLogger>();
            containerRegistry.RegisterSingleton<IApplicationProvider, ApplicationProvider>();
            containerRegistry.RegisterSingleton<IApplicationStore, ApplicationStore>();
            containerRegistry.RegisterSingleton<IEventAggregator, EventAggregator>();
            containerRegistry.RegisterSingleton<IDependencyService, DependencyService>();
            containerRegistry.RegisterSingleton<IPageDialogService, PageDialogService>();
            containerRegistry.RegisterSingleton<IDeviceService, DeviceService>();
            containerRegistry.RegisterSingleton<IPageBehaviorFactory, PageBehaviorFactory>();
            containerRegistry.RegisterSingleton<IModuleCatalog, ModuleCatalog>();
            containerRegistry.RegisterSingleton<IModuleManager, ModuleManager>();
            containerRegistry.RegisterSingleton<IModuleInitializer, ModuleInitializer>();
            containerRegistry.Register<INavigationService, PageNavigationService>(NavigationServiceName);
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
        /// Initializes the modules.
        /// </summary>
        protected virtual void InitializeModules()
        {
            if (_moduleCatalog.Modules.Count() > 0)
            {
                if (!_containerExtension.SupportsModules)
                    throw new NotSupportedException("Container does not support the use of Modules.");

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
            PageUtilities.InvokeViewAndViewModelAction<IApplicationLifecycleAware>(page, x => x.OnResume());
        }

        protected override void OnSleep()
        {
            var page = PageUtilities.GetCurrentPage(MainPage);
            PageUtilities.InvokeViewAndViewModelAction<IApplicationLifecycleAware>(page, x => x.OnSleep());
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
