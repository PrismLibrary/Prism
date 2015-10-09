
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Prism.Windows.Validation
{
    /// <summary>
    /// The IValidatableBindableBase interface was created to add validation support for model classes that contain validation rules.
    /// The default implementation of IValidatableBindableBase is the ValidatableBindableBase class, which contains the logic to run the validation rules of the
    /// instance of a model class and return the results of this validation as a list of properties' errors.
    /// </summary>
    public interface IValidatableBindableBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is validation enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if validation is enabled for this instance; otherwise, <c>false</c>.
        /// </value>
        bool IsValidationEnabled { get; set; }

        /// <summary>
        /// Returns the BindableValidator instance that has an indexer property.
        /// </summary>
        /// <value>
        /// The Bindable Validator Indexer property.
        /// </value>
        BindableValidator Errors { get; }

        /// <summary>
        /// Occurs when the Errors collection changed because new errors were added or old errors were fixed.
        /// </summary>
        event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>
        /// Gets all errors.
        /// </summary>
        /// <returns> A ReadOnlyDictionary that's key is a property name and the value is a ReadOnlyCollection of the error strings.</returns>
        ReadOnlyDictionary<string, ReadOnlyCollection<string>> GetAllErrors();

        /// <summary>
        /// Validates the properties of the current instance.
        /// </summary>
        /// <returns>Returns <c>true</c> if all properties pass the validation rules; otherwise, false.</returns>
        bool ValidateProperties();

        /// <summary>
        /// Validates a single property with the given name of the current instance.
        /// </summary>
        /// <param name="propertyName">The property to be validated.</param>
        /// <returns>Returns <c>true</c> if the property passes the validation rules; otherwise, false.</returns>
        bool ValidateProperty(string propertyName);

        /// <summary>
        /// Sets the error collection of this instance.
        /// </summary>
        /// <param name="entityErrors">The entity errors.</param>
        void SetAllErrors(IDictionary<string, ReadOnlyCollection<string>> entityErrors);
    }
}