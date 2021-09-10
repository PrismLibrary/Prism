namespace Prism.Navigation
{
    /// <summary>
    /// Synchronous initialization of navigation
    /// </summary>
    public interface IInitialize
    {
        /// <summary>
        /// Synchronously initialize method which initializes the navigation.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        void Initialize(INavigationParameters parameters);
    }
}
