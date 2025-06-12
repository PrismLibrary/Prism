using Prism.Mvvm;

namespace Prism.Avalonia.Tests.Mocks.ViewModels
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
            this.RaisePropertyChanged(nameof(MockProperty));
        }
    }
}
