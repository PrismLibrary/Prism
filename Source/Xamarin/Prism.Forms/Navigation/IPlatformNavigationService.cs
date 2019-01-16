using System;
using System.Threading.Tasks;

namespace Prism.Navigation
{
    public interface IPlatformNavigationService
    {
        Task<INavigationResult> GoBackAsync(INavigationParameters parameters, bool? useModalNavigation, bool animated);

        Task<INavigationResult> GoBackToRootAsync(INavigationParameters parameters);

        Task<INavigationResult> NavigateAsync(string name, INavigationParameters parameters, bool? useModalNavigation, bool animated);

        Task<INavigationResult> NavigateAsync(Uri uri, INavigationParameters parameters, bool? useModalNavigation, bool animated);
    }
}
