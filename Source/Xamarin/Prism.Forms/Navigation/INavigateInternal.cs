using System;
using System.Threading.Tasks;

namespace Prism.Navigation
{
    internal interface INavigateInternal
    {
        Task<bool> GoBackInternal(INavigationParameters parameters, bool? useModalNavigation, bool animated);

        Task GoBackToRootInternal(INavigationParameters parameters);

        Task NavigateInternal(string name, INavigationParameters parameters, bool? useModalNavigation, bool animated);

        Task NavigateInternal(Uri uri, INavigationParameters parameters, bool? useModalNavigation, bool animated);
    }
}
