// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using Windows.UI.Xaml.Controls;

namespace Prism.Windows.Tests.Mocks
{
    public class MockPageWithViewModel : Page
    {
        public MockPageWithViewModel()
        {
            var viewModel = new MockViewModelWithRestorableStateAttributes();
            viewModel.Title = "testtitle";
            this.DataContext = viewModel;
        }
    }
}
