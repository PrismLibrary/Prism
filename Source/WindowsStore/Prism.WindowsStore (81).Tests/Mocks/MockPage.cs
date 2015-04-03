// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved


using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Prism.WindowsStore.Tests.Mocks
{
    public class MockPage : Page
    {
        public object PageParameter { get; set; }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e != null)
            {
                this.PageParameter = e.Parameter;
            }
        }
    }
}
