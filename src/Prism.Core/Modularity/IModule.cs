using Prism.Ioc;

namespace Prism.Modularity
{
    /// <summary>
    /// Defines the contract for the modules deployed in the application.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="IModule"/> is the base interface for modular components in Prism applications.
    /// Modules are self-contained units that encapsulate specific features or domain logic and can be
    /// developed, tested, and deployed independently.
    /// </para>
    /// <para>
    /// Each module must implement the registration of its types during the initialization phase
    /// and can perform additional setup in the OnInitialized method.
    /// </para>
    /// </remarks>
    public interface IModule
    {
        /// <summary>
        /// Used to register types with the container that will be used by your application.
        /// </summary>
        /// <param name="containerRegistry">The container registry where types can be registered.</param>
        /// <remarks>
        /// This method is called during the module initialization phase, before OnInitialized.
        /// Use this to register all services, views, and other components provided by this module.
        /// </remarks>
        void RegisterTypes(IContainerRegistry containerRegistry);

        /// <summary>
        /// Notifies the module that it has been initialized.
        /// </summary>
        /// <param name="containerProvider">The container provider that can be used to resolve services.</param>
        /// <remarks>
        /// This method is called after all modules have registered their types.
        /// Use this to initialize the module's functionality, register views with regions, or set up event subscriptions.
        /// </remarks>
        void OnInitialized(IContainerProvider containerProvider);
    }
}
