namespace Prism.Navigation
{
    /// <summary>
    /// This class is used to initialize views and viewmodels during the navigation process.
    /// </summary>
    public interface IInitialize
    {
        /// <summary>
        /// Invoked when the View or ViewModel is first created during the navigation process.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        void Initialize(INavigationParameters parameters);
    }
}
