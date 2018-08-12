using System;
using System.Threading.Tasks;

namespace Prism.Navigation
{
    internal interface INavigateInternal
    {
        Task<INavigationResult> GoBackInternal(INavigationParameters parameters, bool? useModalNavigation, bool animated);

        Task<INavigationResult> GoBackToRootInternal(INavigationParameters parameters);

        Task<INavigationResult> NavigateInternal(string name, INavigationParameters parameters, bool? useModalNavigation, bool animated);

        Task<INavigationResult> NavigateInternal(Uri uri, INavigationParameters parameters, bool? useModalNavigation, bool animated);
    }
}
