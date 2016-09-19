using System;
using System.Threading.Tasks;

namespace Prism.Navigation
{
    /// <summary>
    /// Provides a way for Pages or ViewModels to be aware of Internal (Sideways Navigation) in an asynchronous manner
    /// </summary>
    public interface IMultiPageNavigationAwareAsync
    {
        /// <summary>
        /// Called when the Parent changes the CurrentPage or when the Parent Disappears
        /// </summary>
        /// <param name="parameters">The navigation parameters</param>
        Task OnInternalNavigatedFromAsync(NavigationParameters parameters);

        /// <summary>
        /// Called when the Parent changes the CurrentPage or when the Parent Appears
        /// </summary>
        /// <param name="parameters">The navigation parameters</param>
        Task OnInternalNavigatedToAsync(NavigationParameters parameters);
    }
}

