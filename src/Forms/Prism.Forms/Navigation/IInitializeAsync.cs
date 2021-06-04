using System.Threading.Tasks;

namespace Prism.Navigation
{
    /// <summary>
    /// Provides asynchronous initialization of the ViewModel prior to the View being added to the navigation stack.
    /// </summary>
    /// <remarks>For synchronous initialization see <see cref="IInitialize"/>.</remarks>
    public interface IInitializeAsync
    {
        /// <summary>
        /// Called before the implementor has been navigated to.
        /// </summary>
        /// <param name="parameters">The navigation parameters.</param>
        Task InitializeAsync(INavigationParameters parameters);
    }
}
