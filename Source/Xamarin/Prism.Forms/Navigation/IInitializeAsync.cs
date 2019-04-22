using System.Threading.Tasks;

namespace Prism.Navigation
{
    public interface IInitializeAsync
    {
        Task InitializedAsync(INavigationParameters parameters);
    }
}
