namespace Prism.AppModel
{
    /// <summary>
    /// Interface to handle OS related events when Application is put to sleep, etc.
    /// </summary>
    public interface IApplicationLifecycleAware
    {
        /// <summary>
        /// Called when application is resumed
        /// </summary>
        void OnResume();

        /// <summary>
        /// Called when application is put to sleep
        /// </summary>
        void OnSleep();
    }
}
