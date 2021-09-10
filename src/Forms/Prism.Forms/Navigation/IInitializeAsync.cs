using System.Threading.Tasks;

namespace Prism.Navigation
{
    /// <summary>
    /// Asynchronous initialization of navigation
    /// </summary>
    public interface IInitializeAsync
    {
        /// <summary>
        /// Asynchronously initialize method which initializes the navigation.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        Task InitializeAsync(INavigationParameters parameters);
    }
}
