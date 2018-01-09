using System.ComponentModel;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Runtime.CompilerServices;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Prism.Forms.Validation
{
    public abstract class ValidateableBase : BindableBase, INotifyDataErrorInfo
    {
        private ErrorsContainer<string> _errorsContainer;
        public bool HasErrors => _errorsContainer.HasErrors;

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        protected ValidateableBase()
        {
            _errorsContainer = new ErrorsContainer<string>(RaiseErrorsChanged);
            ErrorsChanged += OnErrorsChanged;
        }

        private void OnErrorsChanged(object sender, DataErrorsChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(HasErrors));
        }

        private void RaiseErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        public IEnumerable GetErrors(string propertyName)
        {
            return _errorsContainer.GetErrors(propertyName);
        }

        protected virtual void ValidateProperty(object value, [CallerMemberName]string propertyName = null)
        {
            var context = new ValidationContext(this)
            {
                MemberName = propertyName
            };
            var validationErrors = new List<ValidationResult>();

            if (!Validator.TryValidateProperty(value, context, validationErrors))
            {
                IEnumerable<string> errors = validationErrors.Select(x => x.ErrorMessage);
                _errorsContainer.SetErrors(propertyName, errors);
            }
            else
            {
                _errorsContainer.ClearErrors(propertyName);
            }
        }

        protected override bool SetProperty<T>(ref T storage, T value, Action onChanged, [CallerMemberName] string propertyName = null)
        {
            ValidateProperty(value, propertyName);
            return base.SetProperty(ref storage, value, onChanged, propertyName);
        }

        protected override bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            ValidateProperty(value, propertyName);
            return base.SetProperty(ref storage, value, propertyName);
        }
    }
}
