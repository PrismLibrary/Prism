namespace Prism.Navigation
{
    /// <summary>
    /// Interface for objects that require cleanup of resources prior to Disposal
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="IDestructible"/> is implemented by Views and ViewModels that need to clean up resources
    /// when they are being removed or destroyed. This is particularly important in modular applications
    /// where views are dynamically loaded and unloaded.
    /// </para>
    /// <para>
    /// When a view is removed from a region or a dialog is closed, Prism will check if the view or its
    /// ViewModel implements this interface and call the <see cref="Destroy"/> method.
    /// </para>
    /// </remarks>
    public interface IDestructible
    {
        /// <summary>
        /// This method allows cleanup of any resources used by your View/ViewModel 
        /// </summary>
        /// <remarks>
        /// This method is called when the view is being removed from a region or when a dialog is being closed.
        /// Use this to unsubscribe from events, dispose of resources, or perform any other cleanup operations.
        /// </remarks>
        void Destroy();
    }
}
