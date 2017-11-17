using System;
using System.Threading.Tasks;

namespace Prism.Navigation
{
    internal interface INavigateInternal
    {
        Task<bool> GoBackInternal(NavigationParameters parameters, bool? useModalNavigation, bool animated);

        Task GoBackToRootInternal(NavigationParameters parameters);

        Task NavigateInternal(string name, NavigationParameters parameters, bool? useModalNavigation, bool animated);

        Task NavigateInternal(Uri uri, NavigationParameters parameters, bool? useModalNavigation, bool animated);
    }
}
