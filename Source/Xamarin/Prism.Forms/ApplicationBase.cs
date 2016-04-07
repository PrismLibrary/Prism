using Prism.Logging;
using Prism.Modularity;
using Prism.Navigation;
#if TEST
using Application = Prism.FormsApplication;
#else
using Application = Xamarin.Forms.Application;
#endif

namespace Prism
{
    public abstract class PrismApplicationBase : Application
    {
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

        public PrismApplicationBase()
        {
            ConfigureViewModelLocator();
            Initialize();
        }

        /// <summary>
        /// Run the bootstrapper process.
        /// </summary>
        public abstract void Initialize();

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
        /// Configures the <see cref="IModuleCatalog"/> used by Prism.
        /// </summary>
        protected virtual void ConfigureModuleCatalog()
        {

        }

        /// <summary>
        /// Initializes the modules.
        /// </summary>
        protected abstract void InitializeModules();

        /// <summary>
        /// Creates the <see cref="INavigationService"/> for the application.
        /// </summary>
        /// <returns></returns>
        protected abstract INavigationService CreateNavigationService();

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
    }
}
