using System;
using System.Threading.Tasks;

namespace Prism.Navigation
{
    public interface IMultiPageNavigationAwareAsync
    {
        Task OnInternalNavigatedFrom(NavigationParameters parameters);
        Task OnInternalNavigatedTo(NavigationParameters parameters);
    }
}

