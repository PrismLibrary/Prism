using Prism.Common;
using Prism.Logging;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Navigation;
using System.Linq;
using Xamarin.Forms;

namespace Prism
{
    public abstract class PrismApplicationBase<T> : Application
    {
        Page _previousPage = null;

        /// <summary>
        /// The dependency injection container used to resolve objects
        /// </summary>
        public T Container { get; protected set; }

        /// <summary>
        /// Gets the <see cref="ILoggerFacade"/> for the application.
        /// </summary>
        /// <value>A <see cref="ILoggerFacade"/> instance.</value>
        protected ILoggerFacade Logger { get; set; }

        /// <summary>
        /// Gets the default <see cref="IModuleCatalog"/> for the application.
        /// </summary>
        /// <value>The default <see cref="IModuleCatalog"/> instance.</value>
        protected IModuleCatalog ModuleCatalog { get; set; }

        /// <summary>
        /// Get the Platform Initializer
        /// </summary>
        protected IPlatformInitializer<T> PlatformInitializer { get; }

        /// <summary>
        /// Gets the <see cref="INavigationService"/> for the application.
        /// </summary>
        protected INavigationService NavigationService { get; set; }

        protected PrismApplicationBase(IPlatformInitializer<T> initializer = null)
        {
            base.ModalPopping += PrismApplicationBase_ModalPopping;
            base.ModalPopped += PrismApplicationBase_ModalPopped;

            PlatformInitializer = initializer;
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
        /// Run the bootstrapper process.
        /// </summary>
        public virtual void Initialize()
        {
            Logger = CreateLogger();

            ModuleCatalog = CreateModuleCatalog();
            ConfigureModuleCatalog();

            Container = CreateContainer();

            ConfigureContainer();

            RegisterTypes();

            PlatformInitializer?.RegisterTypes(Container);
            
            NavigationService = CreateNavigationService();

            InitializeModules();
        }

        /// <summary>
        /// Create the <see cref="ILoggerFacade" /> used by the application.
        /// </summary>
        /// <remarks>
        /// The base implementation returns a new TextLogger.
        /// </remarks>
        protected virtual ILoggerFacade CreateLogger()
        {
#if DEBUG        
            return new DebugLogger();
#else
            return new EmptyLogger();
#endif
        }

        /// <summary>
        /// Creates the <see cref="IModuleCatalog"/> used by Prism.
        /// </summary>
        protected virtual IModuleCatalog CreateModuleCatalog()
        {
            return new ModuleCatalog();
        }

        /// <summary>
        /// Creates the <see cref="IModuleManager"/> used by Prism.
        /// </summary>
        /// <returns>The IModuleManager</returns>
        protected abstract IModuleManager CreateModuleManager();

        /// <summary>
        /// Configures the <see cref="IModuleCatalog"/> used by Prism.
        /// </summary>
        protected virtual void ConfigureModuleCatalog() { }

        /// <summary>
        /// Initializes the modules.
        /// </summary>
        protected virtual void InitializeModules()
        {
            if (ModuleCatalog.Modules.Count() > 0)
            {
                IModuleManager manager = CreateModuleManager();
                manager.Run();
            }
        }

        /// <summary>
        /// Creates the container used by Prism.
        /// </summary>
        /// <returns>The container</returns>
        protected abstract T CreateContainer();

        /// <summary>
        /// Creates the <see cref="INavigationService"/> for the application.
        /// </summary>
        /// <returns></returns>
        protected abstract INavigationService CreateNavigationService();

        /// <summary>
        /// Create instance of <see cref="INavigationService"/> and set the <see cref="IPageAware.Page"/> property to <paramref name="page"/>
        /// </summary>
        /// <param name="page">Active page</param>
        /// <returns>Instance of <see cref="INavigationService"/> with <see cref="IPageAware.Page"/> set</returns>
        protected INavigationService CreateNavigationService(Page page)
        {
            var navigationService = CreateNavigationService();
            ((IPageAware)navigationService).Page = page;
            return navigationService;
        }

        protected INavigationService CreateNavigationService(object view)
        {
            switch(view)
            {
                case Page page:
                    return CreateNavigationService(page);
                case Element element:
                    var parentPage = GetPageFromElement(element);
                    if (parentPage == null)
                    {
                        Logger.Log($"No Parent Page could be found for the View: {view.GetType().Name}", Category.Debug, Priority.None);
                        return null;
                    }
                    ChildViewRegistry.AddChildView(parentPage, element);
                    return CreateNavigationService(parentPage);
                default:
                    return null;
            }
        }

        protected Page GetPageFromElement(Element element)
        {
            switch(element.Parent)
            {
                case Page page:
                    return page;
                case null:
                    return null;
                default:
                    return GetPageFromElement(element.Parent);
            }
        }

        protected abstract void ConfigureContainer();

        /// <summary>
        /// Configures the <see cref="Prism.Mvvm.ViewModelLocator"/> used by Prism.
        /// </summary>
        protected abstract void ConfigureViewModelLocator();

        /// <summary>
        /// Called when the PrismApplication has completed it's initialization process.
        /// </summary>
        protected abstract void OnInitialized();

        /// <summary>
        /// Used to register types with the container that will be used by your application.
        /// </summary>
        protected abstract void RegisterTypes();

        protected override void OnResume()
        {
            var page = PageUtilities.GetCurrentPage(MainPage);
            PageUtilities.InvokeViewAndViewModelAction<AppModel.IApplicationLifecycle>(page, x => x.OnResume());
        }

        protected override void OnSleep()
        {
            var page = PageUtilities.GetCurrentPage(MainPage);
            PageUtilities.InvokeViewAndViewModelAction<AppModel.IApplicationLifecycle>(page, x => x.OnSleep());
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
