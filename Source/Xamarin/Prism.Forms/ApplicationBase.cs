using Prism.Logging;
using Prism.Navigation;
using Xamarin.Forms;

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
