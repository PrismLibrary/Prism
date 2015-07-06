

namespace Prism.Modularity
{
    /// <summary>
    /// Defines the states a <see cref="ModuleInfo"/> can be in, with regards to the module loading and initialization process. 
    /// </summary>
    public enum ModuleState
    {
        /// <summary>
        /// Initial state for <see cref="ModuleInfo"/>s. The <see cref="ModuleInfo"/> is defined, 
        /// but it has not been loaded, retrieved or initialized yet. 
        /// </summary>
        NotStarted,

        /// <summary>
        /// The assembly that contains the type of the module is currently being loaded by an instance of a
        /// <see cref="IModuleTypeLoader"/>. 
        /// </summary>
        LoadingTypes,

        /// <summary>
        /// The assembly that holds the Module is present. This means the type of the <see cref="IModule"/> can be instantiated and initialized. 
        /// </summary>
        ReadyForInitialization,

        /// <summary>
        /// The module is currently Initializing, by the <see cref="IModuleInitializer"/>
        /// </summary>
        Initializing,

        /// <summary>
        /// The module is initialized and ready to be used. 
        /// </summary>
        Initialized
    }
}
