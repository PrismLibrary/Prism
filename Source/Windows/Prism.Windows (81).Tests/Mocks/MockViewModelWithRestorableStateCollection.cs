// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using Prism.Mvvm;
using System.Collections.Generic;

namespace Prism.Windows.Tests.Mocks
{
    public class MockViewModelWithRestorableStateCollection : Prism.Mvvm.ViewModel
    {
        private string _title;
        private string _description;
        private ICollection<BindableBase> _childViewModels;

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }

        }

        [RestorableState]
        public ICollection<BindableBase> ChildViewModels
        {
            get { return _childViewModels; }
            set { SetProperty(ref _childViewModels, value); }
        }
    }
}
