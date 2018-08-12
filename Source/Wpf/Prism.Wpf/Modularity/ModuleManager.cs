

using Prism.Logging;
using Prism.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Prism.Modularity
{
    /// <summary>
    /// Component responsible for coordinating the modules' type loading and module initialization process.
    /// </summary>
    public partial class ModuleManager : IModuleManager, IDisposable
    {
        private readonly IModuleInitializer moduleInitializer;
        private readonly ILoggerFacade loggerFacade;
        private IEnumerable<IModuleTypeLoader> typeLoaders;
        private HashSet<IModuleTypeLoader> subscribedToModuleTypeLoaders = new HashSet<IModuleTypeLoader>();

        /// <summary>
        /// Initializes an instance of the <see cref="ModuleManager"/> class.
        /// </summary>
        /// <param name="moduleInitializer">Service used for initialization of modules.</param>
        /// <param name="moduleCatalog">Catalog that enumerates the modules to be loaded and initialized.</param>
        /// <param name="loggerFacade">Logger used during the load and initialization of modules.</param>
        public ModuleManager(IModuleInitializer moduleInitializer, IModuleCatalog moduleCatalog, ILoggerFacade loggerFacade)
        {
            this.moduleInitializer = moduleInitializer ?? throw new ArgumentNullException(nameof(moduleInitializer));
            this.ModuleCatalog = moduleCatalog ?? throw new ArgumentNullException(nameof(moduleCatalog));
            this.loggerFacade = loggerFacade ?? throw new ArgumentNullException(nameof(loggerFacade));
        }

        /// <summary>
        /// The module catalog specified in the constructor.
        /// </summary>
        protected IModuleCatalog ModuleCatalog { get; }

        /// <summary>
        /// Raised repeatedly to provide progress as modules are loaded in the background.
        /// </summary>
        public event EventHandler<ModuleDownloadProgressChangedEventArgs> ModuleDownloadProgressChanged;

        private void RaiseModuleDownloadProgressChanged(ModuleDownloadProgressChangedEventArgs e)
        {
            ModuleDownloadProgressChanged?.Invoke(this, e);
        }

        /// <summary>
        /// Raised when a module is loaded or fails to load.
        /// </summary>
        public event EventHandler<LoadModuleCompletedEventArgs> LoadModuleCompleted;

        private void RaiseLoadModuleCompleted(IModuleInfo moduleInfo, Exception error)
        {
            this.RaiseLoadModuleCompleted(new LoadModuleCompletedEventArgs(moduleInfo, error));
        }

        private void RaiseLoadModuleCompleted(LoadModuleCompletedEventArgs e)
        {
            this.LoadModuleCompleted?.Invoke(this, e);
        }

        /// <summary>
        /// Initializes the modules marked as <see cref="InitializationMode.WhenAvailable"/> on the <see cref="ModuleCatalog"/>.
        /// </summary>
        public void Run()
        {
            this.ModuleCatalog.Initialize();

            this.LoadModulesWhenAvailable();
        }


        /// <summary>
        /// Loads and initializes the module on the <see cref="ModuleCatalog"/> with the name <paramref name="moduleName"/>.
        /// </summary>
        /// <param name="moduleName">Name of the module requested for initialization.</param>
        public void LoadModule(string moduleName)
        {
            var module = this.ModuleCatalog.Modules.Where(m => m.ModuleName == moduleName);
            if (module == null || module.Count() != 1)
            {
                throw new ModuleNotFoundException(moduleName, string.Format(CultureInfo.CurrentCulture, Resources.ModuleNotFound, moduleName));
            }

            var modulesToLoad = this.ModuleCatalog.CompleteListWithDependencies(module);

            this.LoadModuleTypes(modulesToLoad);
        }

        /// <summary>
        /// Checks if the module needs to be retrieved before it's initialized.
        /// </summary>
        /// <param name="moduleInfo">Module that is being checked if needs retrieval.</param>
        /// <returns></returns>
        protected virtual bool ModuleNeedsRetrieval(IModuleInfo moduleInfo)
        {
            if (moduleInfo == null)
                throw new ArgumentNullException(nameof(moduleInfo));

            if (moduleInfo.State == ModuleState.NotStarted)
            {
                // If we can instantiate the type, that means the module's assembly is already loaded into
                // the AppDomain and we don't need to retrieve it.
                bool isAvailable = Type.GetType(moduleInfo.ModuleType) != null;
                if (isAvailable)
                {
                    moduleInfo.State = ModuleState.ReadyForInitialization;
                }

                return !isAvailable;
            }

            return false;
        }

        private void LoadModulesWhenAvailable()
        {
            var whenAvailableModules = this.ModuleCatalog.Modules.Where(m => m.InitializationMode == InitializationMode.WhenAvailable);
            var modulesToLoadTypes = this.ModuleCatalog.CompleteListWithDependencies(whenAvailableModules);
            if (modulesToLoadTypes != null)
            {
                this.LoadModuleTypes(modulesToLoadTypes);
            }
        }

        private void LoadModuleTypes(IEnumerable<IModuleInfo> moduleInfos)
        {
            if (moduleInfos == null)
            {
                return;
            }

            foreach (var moduleInfo in moduleInfos)
            {
                if (moduleInfo.State == ModuleState.NotStarted)
                {
                    if (this.ModuleNeedsRetrieval(moduleInfo))
                    {
                        this.BeginRetrievingModule(moduleInfo);
                    }
                    else
                    {
                        moduleInfo.State = ModuleState.ReadyForInitialization;
                    }
                }
            }

            this.LoadModulesThatAreReadyForLoad();
        }

        /// <summary>
        /// Loads the modules that are not intialized and have their dependencies loaded.
        /// </summary>
        protected virtual void LoadModulesThatAreReadyForLoad()
        {
            bool keepLoading = true;
            while (keepLoading)
            {
                keepLoading = false;
                var availableModules = this.ModuleCatalog.Modules.Where(m => m.State == ModuleState.ReadyForInitialization);

                foreach (var moduleInfo in availableModules)
                {
                    if ((moduleInfo.State != ModuleState.Initialized) && (this.AreDependenciesLoaded(moduleInfo)))
                    {
                        moduleInfo.State = ModuleState.Initializing;
                        this.InitializeModule(moduleInfo);
                        keepLoading = true;
                        break;
                    }
                }
            }
        }

        private void BeginRetrievingModule(IModuleInfo moduleInfo)
        {
            var moduleInfoToLoadType = moduleInfo;
            IModuleTypeLoader moduleTypeLoader = this.GetTypeLoaderForModule(moduleInfoToLoadType);
            moduleInfoToLoadType.State = ModuleState.LoadingTypes;

            // Delegate += works differently betweem SL and WPF.
            // We only want to subscribe to each instance once.
            if (!this.subscribedToModuleTypeLoaders.Contains(moduleTypeLoader))
            {
                moduleTypeLoader.ModuleDownloadProgressChanged += this.IModuleTypeLoader_ModuleDownloadProgressChanged;
                moduleTypeLoader.LoadModuleCompleted += this.IModuleTypeLoader_LoadModuleCompleted;
                this.subscribedToModuleTypeLoaders.Add(moduleTypeLoader);
            }

            moduleTypeLoader.LoadModuleType(moduleInfo);
        }

        private void IModuleTypeLoader_ModuleDownloadProgressChanged(object sender, ModuleDownloadProgressChangedEventArgs e)
        {
            this.RaiseModuleDownloadProgressChanged(e);
        }

        private void IModuleTypeLoader_LoadModuleCompleted(object sender, LoadModuleCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                if ((e.ModuleInfo.State != ModuleState.Initializing) && (e.ModuleInfo.State != ModuleState.Initialized))
                {
                    e.ModuleInfo.State = ModuleState.ReadyForInitialization;
                }

                // This callback may call back on the UI thread, but we are not guaranteeing it.
                // If you were to add a custom retriever that retrieved in the background, you
                // would need to consider dispatching to the UI thread.
                this.LoadModulesThatAreReadyForLoad();
            }
            else
            {
                this.RaiseLoadModuleCompleted(e);

                // If the error is not handled then I log it and raise an exception.
                if (!e.IsErrorHandled)
                {
                    this.HandleModuleTypeLoadingError(e.ModuleInfo, e.Error);
                }
            }
        }

        /// <summary>
        /// Handles any exception occurred in the module typeloading process,
        /// logs the error using the <see cref="ILoggerFacade"/> and throws a <see cref="ModuleTypeLoadingException"/>.
        /// This method can be overridden to provide a different behavior.
        /// </summary>
        /// <param name="moduleInfo">The module metadata where the error happenened.</param>
        /// <param name="exception">The exception thrown that is the cause of the current error.</param>
        /// <exception cref="ModuleTypeLoadingException"></exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1")]
        protected virtual void HandleModuleTypeLoadingError(IModuleInfo moduleInfo, Exception exception)
        {
            if (moduleInfo == null)
                throw new ArgumentNullException(nameof(moduleInfo));


            if (!(exception is ModuleTypeLoadingException moduleTypeLoadingException))
            {
                moduleTypeLoadingException = new ModuleTypeLoadingException(moduleInfo.ModuleName, exception.Message, exception);
            }

            this.loggerFacade.Log(moduleTypeLoadingException.Message, Category.Exception, Priority.High);

            throw moduleTypeLoadingException;
        }

        private bool AreDependenciesLoaded(IModuleInfo moduleInfo)
        {
            var requiredModules = this.ModuleCatalog.GetDependentModules(moduleInfo);
            if (requiredModules == null)
            {
                return true;
            }

            int notReadyRequiredModuleCount =
                requiredModules.Count(requiredModule => requiredModule.State != ModuleState.Initialized);

            return notReadyRequiredModuleCount == 0;
        }

        private IModuleTypeLoader GetTypeLoaderForModule(IModuleInfo moduleInfo)
        {
            foreach (IModuleTypeLoader typeLoader in this.ModuleTypeLoaders)
            {
                if (typeLoader.CanLoadModuleType(moduleInfo))
                {
                    return typeLoader;
                }
            }

            throw new ModuleTypeLoaderNotFoundException(moduleInfo.ModuleName, string.Format(CultureInfo.CurrentCulture, Resources.NoRetrieverCanRetrieveModule, moduleInfo.ModuleName), null);
        }

        private void InitializeModule(IModuleInfo moduleInfo)
        {
            if (moduleInfo.State == ModuleState.Initializing)
            {
                this.moduleInitializer.Initialize(moduleInfo);
                moduleInfo.State = ModuleState.Initialized;
                this.RaiseLoadModuleCompleted(moduleInfo, null);
            }
        }

        #region Implementation of IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <remarks>Calls <see cref="Dispose(bool)"/></remarks>.
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the associated <see cref="IModuleTypeLoader"/>s.
        /// </summary>
        /// <param name="disposing">When <see langword="true"/>, it is being called from the Dispose method.</param>
        protected virtual void Dispose(bool disposing)
        {
            foreach (IModuleTypeLoader typeLoader in this.ModuleTypeLoaders)
            {
                if (typeLoader is IDisposable disposableTypeLoader)
                {
                    disposableTypeLoader.Dispose();
                }
            }
        }

        #endregion
    }
}
