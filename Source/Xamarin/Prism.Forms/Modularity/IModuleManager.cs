namespace Prism.Modularity
{
    /// <summary>
    /// Defines the interface for the service that will retrieve and initialize the application's modules.
    /// </summary>
    public interface IModuleManager
    {
        /// <summary>
        /// Initializes the modules in the <see cref="ModuleCatalog"/>.
        /// </summary>
        void Run();
    }
}