// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Navigation;

namespace Prism.Windows.Tests.Mocks
{
    public class MockPageViewModel : Prism.Mvvm.ViewModel
    {
        public Action<object, NavigationMode, Dictionary<string, object>> OnNavigatedToCommand { get; set; }
        public Action<Dictionary<string, object>, bool> OnNavigatedFromCommand { get; set; }

        public override void OnNavigatedTo(object navigationParameter, NavigationMode navigationMode, Dictionary<string, object> viewState)
        {
            this.OnNavigatedToCommand(navigationParameter, navigationMode, viewState);
        }

        public override void OnNavigatedFrom(Dictionary<string, object> viewState, bool suspending)
        {
            this.OnNavigatedFromCommand(viewState, suspending);
        }
    }
}
