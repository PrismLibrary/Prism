

using Prism.Mvvm;
using System.ComponentModel;

namespace Prism.Tests.Mocks.ViewModels
{
    public class MockViewModel : BindableBase
    {
        private int mockProperty;

        public int MockProperty
        {
            get
            {
                return this.mockProperty;
            }

            set
            {
                this.SetProperty(ref mockProperty, value);
            }
        }

        internal MockNestedModel MockNestedModel { get; set; }
        
        public int MockNestedProperty
        {
            get => MockNestedModel.MockNestedProperty;
            set => SetNestedProperty(MockNestedModel, value);
        }

        internal void InvokeOnPropertyChanged()
        {
			RaisePropertyChanged(nameof(MockProperty));
        }

        internal void InvokeOnNestedPropertyChanged()
        {
            RaisePropertyChanged(nameof(MockNestedProperty));
        }
    }
}
