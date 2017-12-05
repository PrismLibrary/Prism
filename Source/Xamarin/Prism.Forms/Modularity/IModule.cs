using Prism.Ioc;
using System;

namespace Prism.Modularity
{
    /// <summary>
    /// Defines the contract for the modules deployed in the application.
    /// </summary>
    public interface IModule
    {
        /// <summary>
        /// Notifies the module that it is being initialized.
        /// </summary>
        [Obsolete("Please move all code into the RegisterTypes and OnInitialized methods. This method will be removed.")]
        void Initialize();

        /// <summary>
        /// Used to register types with the container that will be used by your application.
        /// </summary>
        void RegisterTypes(IContainerRegistry containerRegistry);

        /// <summary>
        /// Notifies the module that it has be initialized.
        /// </summary>
        void OnInitialized();
    }
}
