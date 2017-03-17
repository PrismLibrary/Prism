using System.Collections.Generic;
using Prism.Mvvm;
using Prism.Windows.AppModel;
using Prism.Windows.Mvvm;

namespace Prism.Windows.Tests.Mocks
{
    public class MockViewModelWithRestorableStateCollection : ViewModelBase
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
