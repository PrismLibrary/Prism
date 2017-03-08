using Prism.Windows.Mvvm;
using System.Collections.Generic;

namespace Prism.Windows.Tests.Mocks
{
    public class MockViewModelWithNoRestorableStateAttributes : ViewModelBase
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

        private ICollection<ViewModelBase> childViewModels;

        public ICollection<ViewModelBase> ChildViewModels
        {
            get { return this.childViewModels; }
            set { this.SetProperty(ref this.childViewModels, value); }

        }
    }
}
