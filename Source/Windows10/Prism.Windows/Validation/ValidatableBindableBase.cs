using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using Prism.Mvvm;

namespace Prism.Windows.Validation
{
    /// <summary>
    /// The IValidatableBindableBase interface was created to add validation support for model classes that contain validation rules.
    /// The default implementation of IValidatableBindableBase is the ValidatableBindableBase class, which contains the logic to run the validation rules of the
    /// instance of a model class and return the results of this validation as a list of properties' errors.
    /// </summary>
    // Documentation on validating user input is at http://go.microsoft.com/fwlink/?LinkID=288817&clcid=0x409
    public class ValidatableBindableBase : BindableBase, IValidatableBindableBase
    {
        private readonly BindableValidator _bindableValidator;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidatableBindableBase"/> class.
        /// </summary>
        public ValidatableBindableBase()
        {
            _bindableValidator = new BindableValidator(this);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is validation enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if validation is enabled for this instance; otherwise, <c>false</c>.
        /// </value>
        public bool IsValidationEnabled
        {
            get { return _bindableValidator.IsValidationEnabled; }
            set { _bindableValidator.IsValidationEnabled = value; }
        }

        /// <summary>
        /// Returns the BindableValidator instance that has an indexer property.
        /// </summary>
        /// <value>
        /// The Bindable Validator Indexer property.
        /// </value>
        public BindableValidator Errors
        {
            get
            {
                return _bindableValidator;
            }
        }

        /// <summary>
        /// Occurs when the Errors collection changed because new errors were added or old errors were fixed.
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged
        {
            add { _bindableValidator.ErrorsChanged += value; }

            remove { _bindableValidator.ErrorsChanged -= value; }
        }

        /// <summary>
        /// Gets all errors.
        /// </summary>
        /// <returns> A ReadOnlyDictionary that's key is a property name and the value is a ReadOnlyCollection of the error strings.</returns>
        public ReadOnlyDictionary<string, ReadOnlyCollection<string>> GetAllErrors()
        {
            return _bindableValidator.GetAllErrors();
        }

        /// <summary>
        /// Validates the properties of the current instance.
        /// </summary>
        /// <returns>
        /// Returns <c>true</c> if all properties pass the validation rules; otherwise, false.
        /// </returns>
        public bool ValidateProperties()
        {
            return _bindableValidator.ValidateProperties();
        }

        /// <summary>
        /// Validates a single property with the given name of the current instance.
        /// </summary>
        /// <param name="propertyName">The property to be validated.</param>
        /// <returns>Returns <c>true</c> if the property passes the validation rules; otherwise, false.</returns>
        public bool ValidateProperty(string propertyName)
        {
            return !_bindableValidator.IsValidationEnabled // don't fail if validation is disabled
                || _bindableValidator.ValidateProperty(propertyName);
        }

        /// <summary>
        /// Sets the error collection of this instance.
        /// </summary>
        /// <param name="entityErrors">The entity errors.</param>
        public void SetAllErrors(IDictionary<string, ReadOnlyCollection<string>> entityErrors)
        {
            _bindableValidator.SetAllErrors(entityErrors);
        }

        /// <summary>
        /// Checks if a property already matches a desired value. Sets the property and
        /// notifies listeners only when necessary. We are overriding this property to ensure that the SetProperty and the ValidateProperty methods are fired in a
        /// deterministic way.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="storage">Reference to a property with both getter and setter.</param>
        /// <param name="value">Desired value for the property.</param>
        /// <param name="propertyName">Name of the property used to notify listeners. This
        /// value is optional and can be provided automatically when invoked from compilers that
        /// support CallerMemberName.</param>
        /// <returns>
        /// True if the value was changed, false if the existing value matched the
        /// desired value.
        /// </returns>
        protected override bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            var result = base.SetProperty(ref storage, value, propertyName);

            if (result && !string.IsNullOrEmpty(propertyName))
            {
                if (_bindableValidator.IsValidationEnabled)
                {
                    _bindableValidator.ValidateProperty(propertyName);
                }
            }
            return result;
        }
    }
}
