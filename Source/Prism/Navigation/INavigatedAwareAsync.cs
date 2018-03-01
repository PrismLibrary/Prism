using System.Threading.Tasks;

namespace Prism.Navigation
{
    /// <summary>
    /// Provides a way for ViewModels involved in navigation to be notified of navigation activities after the target Page has been added to the navigation stack.
    /// </summary>
    /// <remarks>Not currently supported on Xamarin.Forms</remarks>
    public interface INavigatedAwareAsync
    {
        /// <summary>
        /// Called when the implementer has been navigated to.
        /// </summary>
        /// <remarks>Not currently supported on Xamarin.Forms</remarks>
        /// <param name="parameters">The navigation parameters.</param>
        Task OnNavigatedToAsync(INavigationParameters parameters);
    }
}
