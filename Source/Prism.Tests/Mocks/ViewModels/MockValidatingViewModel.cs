using Prism.Mvvm;
using System.ComponentModel;

namespace Prism.Tests.Mocks.ViewModels
{
    public class MockValidatingViewModel : BindableBase, INotifyDataErrorInfo
    {
        ErrorsContainer<string> _errorsContainer;

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

                if (mockProperty < 0)
                    _errorsContainer.SetErrors<int>(() => MockProperty, new string[] { "value cannot be less than 0" });
            }
        }

        internal void ClearMockPropertyErrors()
        {
            _errorsContainer.ClearErrors<int>(() => MockProperty);
        }

        internal void SetMockPropertyErrorsWithNullCollection()
        {
            _errorsContainer.SetErrors<int>(() => MockProperty, null);
        }

        public MockValidatingViewModel()
        {
            _errorsContainer = new ErrorsContainer<string>(OnErrorsChanged);
        }

        public event System.EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        public void OnErrorsChanged(string propertyName)
        {
            var handler = ErrorsChanged;
        }

        public System.Collections.IEnumerable GetErrors(string propertyName)
        {
            return _errorsContainer.GetErrors(propertyName);
        }

        public bool HasErrors
        {
            get { return _errorsContainer.HasErrors; }
        }
    }
}
