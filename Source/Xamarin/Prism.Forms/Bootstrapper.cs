using Prism.Logging;
using Prism.Navigation;
using System;
using Xamarin.Forms;

namespace Prism
{
    /// <summary>
    /// Base class that provides a basic bootstrapping sequence and hooks that specific implementations can override
    /// </summary>
    /// <remarks>
    /// This class must be overridden to provide application specific configuration.
    /// </remarks>
    [Obsolete("Please have your App.cs derive from PrismApplication instead.")]
    public abstract class Bootstrapper
    {
        /// <summary>
        /// The current <see cref="Xamarin.Forms.Application"/>
        /// </summary>
        protected Application App { get; set; }

        /// <summary>
        /// Gets the <see cref="ILoggerFacade"/> for the application.
        /// </summary>
        /// <value>A <see cref="ILoggerFacade"/> instance.</value>
        protected ILoggerFacade Logger { get; set; }

        /// <summary>
        /// Gets the <see cref="INavigationService"/> for the application.
        /// </summary>
        protected INavigationService NavigationService { get; set; }

        /// <summary>
        /// Runs the bootstrapper process.
        /// </summary>
        /// <param name="app">The current <see cref="Xamarin.Forms.Application"/></param>
        public void Run(Application app)
        {
            App = app;
            ConfigureViewModelLocator();
            Run();
        }

        /// <summary>
        /// Run the bootstrapper process.
        /// </summary>
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

        /// <summary>
        /// Creates the root <see cref="Xamarin.Forms.Page"/> for the application.
        /// </summary>
        /// <returns>The <see cref="Xamarin.Forms.Page"/></returns>
        [Obsolete]
        protected virtual Page CreateMainPage() => null;

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
        /// Called when the bootstrapper has completed it's initialization process.
        /// </summary>
        protected abstract void OnInitialized();

        /// <summary>
        /// Used to register types with the container that will be used by your application.
        /// </summary>
        protected abstract void RegisterTypes();
    }
}
