// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved


using System.Collections.Generic;

namespace Prism.WindowsStore.Tests.Mocks
{
    public class MockViewModelWithRestorableStateAttributes : ViewModel
    {
        private string _title;
        private string _description;
        private ICollection<BindableBase> _childViewModels;
        
        [RestorableState]
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        [RestorableState]
        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }

        }

        public ICollection<BindableBase> ChildViewModels
        {
            get { return _childViewModels; }
            set { SetProperty(ref _childViewModels, value); }
        }
    }
}
