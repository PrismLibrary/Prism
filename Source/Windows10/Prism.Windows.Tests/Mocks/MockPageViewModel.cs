using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Navigation;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;

namespace Prism.Windows.Tests.Mocks
{
    public class MockPageViewModel : ViewModelBase
    {
        public Action<object, NavigationMode, Dictionary<string, object>> OnNavigatedToCommand { get; set; }
        public Action<Dictionary<string, object>, bool> OnNavigatedFromCommand { get; set; }

        public override void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewState)
        {
            this.OnNavigatedToCommand(e.Parameter, e.NavigationMode, viewState);
        }

        public override void OnNavigatingFrom(NavigatingFromEventArgs e, Dictionary<string, object> viewState, bool suspending)
        {
            this.OnNavigatedFromCommand(viewState, suspending);
        }
    }
}
