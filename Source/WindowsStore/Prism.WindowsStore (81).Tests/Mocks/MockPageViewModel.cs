// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved


using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Navigation;

namespace Prism.WindowsStore.Tests.Mocks
{
    public class MockPageViewModel : ViewModel
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
