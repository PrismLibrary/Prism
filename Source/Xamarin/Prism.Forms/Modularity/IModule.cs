namespace Prism.Modularity
{
    /// <summary>
    /// Defines the contract for the modules deployed in the application.
    /// </summary>
    //public interface IModule
    //{
    //    /// <summary>
    //    /// Notifies the module that it has be initialized.
    //    /// </summary>
    //    void Initialize();
    //}

    /// <summary>
    /// Defines the contract for the modules deployed in the application.
    /// </summary>
    public interface IModule
    {
        /// <summary>
        /// Notifies the module that it has be initialized.
        /// </summary>
        void Initialize(); //TODO: SHould we keep both?

        void RegisterTypes(Ioc.IContainerRegistry containerRegistry);
    }
}
