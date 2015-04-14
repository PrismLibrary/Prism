using Prism.Logging;
using Prism.Navigation;
using Xamarin.Forms;

namespace Prism
{
    public abstract class Bootstrapper
    {
        protected Application App { get; set; }

        public INavigationService NavigationService { get; set; }

        /// <summary>
        /// Gets the <see cref="ILoggerFacade"/> for the application.
        /// </summary>
        /// <value>A <see cref="ILoggerFacade"/> instance.</value>
        protected ILoggerFacade Logger { get; set; }

        /// <summary>
        /// Runs the bootstrapper process.
        /// </summary>
        public void Run(Application app)
        {
            App = app;
            ConfigureViewModelLocator();
            Run();
        }

        public abstract void Run();

        /// <summary>
        /// Create the <see cref="ILoggerFacade" /> used by the bootstrapper.
        /// </summary>
        /// <remarks>
        /// The base implementation returns a new TextLogger.
        /// </remarks>
        protected virtual ILoggerFacade CreateLogger()
        {
            return new DebugLogger();
        }

        protected abstract Page CreateMainPage();

        protected abstract void ConfigureViewModelLocator();

        /// <summary>
        /// Configures the LocatorProvider for the <see cref="Microsoft.Practices.ServiceLocation.ServiceLocator" />.
        /// </summary>
        protected abstract void ConfigureServiceLocator();

        protected abstract void RegisterTypes();
    }
}
