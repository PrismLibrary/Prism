﻿using Prism.Navigation;

namespace Prism.Forms.Tests.Mocks.ViewModels
{
    public class ViewModelBase : INavigationAware
    {
        public NavigationParameters NavigatedToParameters { get; private set; }
        public NavigationParameters NavigatedFromParameters { get; private set; }

        public bool OnNavigatedToCalled { get; private set; } = false;

        public bool OnNavigatedFromCalled { get; private set; } = false;

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
            OnNavigatedFromCalled = true;
            NavigatedFromParameters = parameters;
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            OnNavigatedToCalled = true;
            NavigatedToParameters = parameters;
        }
    }
}
