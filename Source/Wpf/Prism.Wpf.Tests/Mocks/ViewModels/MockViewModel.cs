using Prism.Mvvm;

namespace Prism.Wpf.Tests.Mocks.ViewModels
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
            this.OnPropertyChanged(() => this.MockProperty);
        }
    }
}
