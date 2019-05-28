using System.Threading.Tasks;

namespace Prism.Navigation
{
    public interface IInitializeAsync
    {
        Task InitializeAsync(INavigationParameters parameters);
    }
}
