namespace Prism.Navigation
{
    /// <summary>
    /// Provides synchronous initialization of the ViewModel prior to the View being added to the navigation stack.
    /// </summary>
    /// <remarks>For asynchronous initialization see <see cref="IInitializeAsync"/>.</remarks>
    public interface IInitialize
    {
        /// <summary>
        /// Called before the implementor has been navigated to.
        /// </summary>
        /// <param name="parameters">The navigation parameters.</param>
        void Initialize(INavigationParameters parameters);
    }
}
