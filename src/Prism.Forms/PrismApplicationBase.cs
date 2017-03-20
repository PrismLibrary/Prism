﻿using Prism.Common;
using Prism.Logging;
using Prism.Modularity;
using Prism.Navigation;
using System.Linq;
using Xamarin.Forms;
#if TEST
using Application = Prism.FormsApplication;
#endif

namespace Prism
{
    public abstract class PrismApplicationBase<T> : Application
    {
        IPlatformInitializer<T> _platformInitializer = null;
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
        /// Gets the <see cref="INavigationService"/> for the application.
        /// </summary>
        protected INavigationService NavigationService { get; set; }

        protected PrismApplicationBase(IPlatformInitializer<T> initializer = null)
        {
            base.ModalPopping += PrismApplicationBase_ModalPopping;
            base.ModalPopped += PrismApplicationBase_ModalPopped;

            _platformInitializer = initializer;
            InitializeInternal();
        }

        /// <summary>
        /// Run the intialization process.
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

            _platformInitializer?.RegisterTypes(Container);
            
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
            return new DebugLogger();
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
