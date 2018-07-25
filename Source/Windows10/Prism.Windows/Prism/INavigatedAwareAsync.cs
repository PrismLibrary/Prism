using System.Threading.Tasks;

namespace Prism.Navigation
{
    public interface INavigatedAwareAsync
    {
        Task OnNavigatedToAsync(INavigationParameters parameters);
    }
}
