

using System.ComponentModel;
using Prism.Mvvm;

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

        internal void InvokeOnPropertyChanged()
        {
            RaisePropertyChanged(nameof(MockProperty));
        }
    }
}
