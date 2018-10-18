

using Prism.Mvvm;

namespace Prism.Tests.Mocks.ViewModels
{
    public class MockChangingViewModel : ChangingBindableBase
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

        internal void InvokeOnPropertyChanging()
        {
            RaisePropertyChanging(nameof(MockProperty));
        }
    }
}
