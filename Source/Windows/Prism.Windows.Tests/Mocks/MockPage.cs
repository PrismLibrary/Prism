// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Prism.Windows.Tests.Mocks
{
    using Prism.Mvvm;

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
