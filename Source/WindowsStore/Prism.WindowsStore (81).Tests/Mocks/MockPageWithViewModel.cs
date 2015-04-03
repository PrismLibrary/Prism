// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved


using Windows.UI.Xaml.Controls;

namespace Prism.WindowsStore.Tests.Mocks
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
