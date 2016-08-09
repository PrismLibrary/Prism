using System;

namespace Prism.Navigation
{
    public interface IMultiPageNavigationAware
    {
        void OnInternalNavigatedFrom(NavigationParameters parameters);
        void OnInternalNavigatedTo(NavigationParameters parameters);
    }
}

