using System;
using System.Collections.Generic;

namespace Prism.Modularity
{
    /// <summary>
    /// Defines the interface for the service that will retrieve and initialize the application's modules.
    /// </summary>
    public interface IModuleManager
    {
        /// <summary>
        /// Gets all the <see cref="IModuleInfo"/> classes that are in the <see cref="IModuleCatalog"/>.
        /// </summary>
        IEnumerable<IModuleInfo> Modules { get; }

        /// <summary>
        /// Initializes the modules marked as <see cref="InitializationMode.WhenAvailable"/> on the <see cref="IModuleCatalog"/>.
        /// </summary>
        void Run();

        /// <summary>
        /// Loads and initializes the module on the <see cref="IModuleCatalog"/> with the name <paramref name="moduleName"/>.
        /// </summary>
        /// <param name="moduleName">Name of the module requested for initialization.</param>
        void LoadModule(string moduleName);

        /// <summary>
        /// Raised repeatedly to provide progress as modules are downloaded.
        /// </summary>
        event EventHandler<ModuleDownloadProgressChangedEventArgs> ModuleDownloadProgressChanged;

        /// <summary>
        /// Raised when a module is loaded or fails to load.
        /// </summary>
        event EventHandler<LoadModuleCompletedEventArgs> LoadModuleCompleted;
    }
}
