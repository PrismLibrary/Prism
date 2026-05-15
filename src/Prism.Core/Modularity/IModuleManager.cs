using System;
using System.Collections.Generic;

namespace Prism.Modularity
{
    /// <summary>
    /// Defines the interface for the service that will retrieve and initialize the application's modules.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="IModuleManager"/> orchestrates the loading and initialization of all modules in the application.
    /// It works with the <see cref="IModuleCatalog"/> to determine which modules to load and when to load them,
    /// based on their <see cref="InitializationMode"/>.
    /// </para>
    /// <para>
    /// The module manager handles the complete lifecycle of modules from discovery through initialization,
    /// including error handling and reporting progress during module loading.
    /// </para>
    /// </remarks>
    public interface IModuleManager
    {
        /// <summary>
        /// Gets all the <see cref="IModuleInfo"/> classes that are in the <see cref="IModuleCatalog"/>.
        /// </summary>
        /// <value>An enumerable collection of all registered modules.</value>
        /// <remarks>
        /// This provides a read-only view of all modules that have been registered with the module catalog.
        /// </remarks>
        IEnumerable<IModuleInfo> Modules { get; }

        /// <summary>
        /// Initializes the modules marked as <see cref="InitializationMode.WhenAvailable"/> on the <see cref="IModuleCatalog"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method starts the module loading process. Only modules marked with <see cref="InitializationMode.WhenAvailable"/>
        /// will be loaded. Modules with <see cref="InitializationMode.OnDemand"/> must be explicitly loaded using <see cref="LoadModule(string)"/>.
        /// </para>
        /// <para>
        /// Events will be raised as modules load or fail to load.
        /// </para>
        /// </remarks>
        void Run();

        /// <summary>
        /// Loads and initializes the module on the <see cref="IModuleCatalog"/> with the name <paramref name="moduleName"/>.
        /// </summary>
        /// <param name="moduleName">Name of the module requested for initialization.</param>
        /// <remarks>
        /// This method can be used to load modules on-demand at runtime. The module will be loaded regardless of its
        /// <see cref="InitializationMode"/> setting.
        /// </remarks>
        void LoadModule(string moduleName);

        /// <summary>
        /// Raised repeatedly to provide progress as modules are downloaded.
        /// </summary>
        /// <remarks>
        /// This event is raised during module loading to report progress. It can be used to show loading progress in the UI.
        /// </remarks>
        event EventHandler<ModuleDownloadProgressChangedEventArgs> ModuleDownloadProgressChanged;

        /// <summary>
        /// Raised when a module is loaded or fails to load.
        /// </summary>
        /// <remarks>
        /// This event is raised for each module when its loading completes, either successfully or with an error.
        /// Check the <see cref="LoadModuleCompletedEventArgs.IsLoaded"/> property to determine if the module loaded successfully.
        /// </remarks>
        event EventHandler<LoadModuleCompletedEventArgs> LoadModuleCompleted;
    }
}
