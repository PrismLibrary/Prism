
namespace Prism.Navigation
{
    /// <summary>
    /// Provides a way for ViewModels involved in navigation to determine if a navigation request should continue.
    /// </summary>
    public interface IConfirmNavigation
    {
        /// <summary>
        /// Determines whether this instance accepts being navigated away from.
        /// </summary>
        /// <param name="parameters">The navigation parameters.</param>
        /// <returns><c>True</c> if navigation can continue, <c>False</c> if navigation is not allowed to continue</returns>
        bool CanNavigate(INavigationParameters parameters);
    }
}
