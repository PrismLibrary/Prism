// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved


using System.Collections.Generic;

namespace Prism.WindowsStore.Tests.Mocks
{
    public class MockViewModelWithNoRestorableStateAttributes : ViewModel
    {
        private string title;
        public string Title
        {
            get { return this.title; }
            set { this.SetProperty(ref this.title, value); }

        }

        private string description;
        public string Description
        {
            get { return this.description; }
            set { this.SetProperty(ref this.description, value); }

        }

        private ICollection<ViewModel> childViewModels;

        public ICollection<ViewModel> ChildViewModels
        {
            get { return this.childViewModels; }
            set { this.SetProperty(ref this.childViewModels, value); }

        }
    }
}
