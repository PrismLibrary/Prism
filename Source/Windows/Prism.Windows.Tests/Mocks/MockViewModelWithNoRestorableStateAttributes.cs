// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Prism.Windows.Tests.Mocks
{
    public class MockViewModelWithNoRestorableStateAttributes : Prism.Mvvm.ViewModel
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

        private ICollection<Prism.Mvvm.ViewModel> childViewModels;

        public ICollection<Prism.Mvvm.ViewModel> ChildViewModels
        {
            get { return this.childViewModels; }
            set { this.SetProperty(ref this.childViewModels, value); }

        }
    }
}
