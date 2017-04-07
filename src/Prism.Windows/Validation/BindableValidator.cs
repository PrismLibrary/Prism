using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Windows.ApplicationModel.Resources;

namespace Prism.Windows.Validation
{
    /// <summary>
    /// The BindableValidator class run validation rules of an entity, and stores a collection of errors of the properties that did not pass validation.
    /// The validation is run on each property change or whenever the ValidateProperties method is called.
    /// It also provides an indexer property, that uses the property names as keys and return the error list for the specified property.
    /// </summary>
    public class BindableValidator : INotifyPropertyChanged
    {
        private readonly INotifyPropertyChanged _entityToValidate;

        private IDictionary<string, ReadOnlyCollection<string>> _errors = new Dictionary<string, ReadOnlyCollection<string>>();

        /// <summary>
        /// Represents a collection of empty error values.
        /// </summary>
        public static readonly ReadOnlyCollection<string> EmptyErrorsCollection = new ReadOnlyCollection<string>(new List<string>());

        private Func<string, string, string> _getResourceDelegate;

        /// <summary>
        /// Initializes a new instance of the BindableValidator class with the entity to validate.
        /// </summary>
        /// <param name="entityToValidate">The entity to validate</param>
        /// <param name="getResourceDelegate">A delegate that returns a string resource given a resource map Id and resource Id</param>
        /// <exception cref="ArgumentNullException">When <paramref name="entityToValidate"/> is <see langword="null" />.</exception>
        public BindableValidator(INotifyPropertyChanged entityToValidate, Func<string, string, string> getResourceDelegate)
            : this(entityToValidate)
        {
            _getResourceDelegate = getResourceDelegate;
        }

        /// <summary>
        /// Initializes a new instance of the BindableValidator class with the entity to validate.
        /// </summary>
        /// <param name="entityToValidate">The entity to validate</param>
        /// <exception cref="ArgumentNullException">When <paramref name="entityToValidate"/> is <see langword="null" />.</exception>
        public BindableValidator(INotifyPropertyChanged entityToValidate)
        {
            _entityToValidate = entityToValidate ?? throw new ArgumentNullException(nameof(entityToValidate));
            IsValidationEnabled = true;
            _getResourceDelegate = (mapId, key) =>
            {
                var resourceLoader = ResourceLoader.GetForCurrentView(mapId);
                return resourceLoader.GetString(key);
            };
        }

        /// <summary>
        /// Multicast event for errors change notifications.
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>
        /// Multicast event for property change notifications.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Returns the errors of the property.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The errors of the property, if it has errors. Otherwise, the BindableValidator.EmptyErrorsCollection.</returns>
        public ReadOnlyCollection<string> this[string propertyName]
        {
            get
            {
                return _errors.ContainsKey(propertyName) ? _errors[propertyName] : EmptyErrorsCollection;
            }
        }

        /// <summary>
        /// Gets the list of errors per property.
        /// </summary>
        /// <value>
        /// The dictionary of property names and errors collection pairs.
        /// </value>
        public IDictionary<string, ReadOnlyCollection<string>> Errors
        {
            get { return _errors; }
        }

        /// <summary>
        /// Returns true if the Validation functionality is enabled. Otherwise, false.
        /// </summary>
        public bool IsValidationEnabled { get; set; }

        /// <summary>
        /// Returns a new ReadOnlyDictionary containing all the errors of the Entity, separated by property.
        /// </summary>
        /// <returns>
        /// A ReadOnlyDictionary that contains a KeyValuePair for each property with errors. 
        /// Each KeyValuePair has a property name as the key, and the value is the collection of errors of that property.
        /// </returns>
        public ReadOnlyDictionary<string, ReadOnlyCollection<string>> GetAllErrors()
        {
            return new ReadOnlyDictionary<string, ReadOnlyCollection<string>>(_errors);
        }

        /// <summary>
        /// Updates the errors collection of the entity, notifying if the errors collection has changed.
        /// </summary>
        /// <param name="entityErrors">The collection of errors for the entity.</param>
        public void SetAllErrors(IDictionary<string, ReadOnlyCollection<string>> entityErrors)
        {
            if (entityErrors == null)
            {
                throw new ArgumentNullException(nameof(entityErrors));
            }

            _errors.Clear();

            foreach (var item in entityErrors)
            {
                SetPropertyErrors(item.Key, item.Value);
            }

            OnPropertyChanged("Item[]");
            OnErrorsChanged(string.Empty);
        }

        /// <summary>
        /// Validates the property, based on the rules set in the property ValidationAttributes attributes. 
        /// It updates the errors collection with the new validation results (notifying if necessary). 
        /// </summary>
        /// <param name="propertyName">The name of the property to validate.</param>
        /// <returns>True if the property is valid. Otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="propertyName"/> is <see langword="null" /> or an empty string value.</exception>
        /// <exception cref="ArgumentException">When the <paramref name="propertyName"/> parameter does not match any property name.</exception>
        public bool ValidateProperty(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            var propertyInfo = _entityToValidate.GetType().GetRuntimeProperty(propertyName);
            if (propertyInfo == null)
            {
                var errorString = _getResourceDelegate(Constants.InfrastructureResourceMapId, "InvalidPropertyNameException");

                throw new ArgumentException(errorString, propertyName);
            }

            var propertyErrors = new List<string>();
            bool isValid = TryValidateProperty(propertyInfo, propertyErrors);
            bool errorsChanged = SetPropertyErrors(propertyInfo.Name, propertyErrors);

            if (errorsChanged)
            {
                OnErrorsChanged(propertyName);
                OnPropertyChanged(string.Format(CultureInfo.CurrentCulture, "Item[{0}]", propertyName));
            }

            return isValid;
        }

        /// <summary>
        /// Validates all the properties decorated with the ValidationAttribute attribute.
        /// It updates each property errors collection with the new validation results (notifying if necessary). 
        /// </summary>
        /// <returns>True if the property is valid. Otherwise, false.</returns>
        public bool ValidateProperties()
        {
            var propertiesWithChangedErrors = new List<string>();

            // Get all the properties decorated with the ValidationAttribute attribute.
            var propertiesToValidate = _entityToValidate.GetType()
                                                        .GetRuntimeProperties()
                                                        .Where(c => c.GetCustomAttributes(typeof(ValidationAttribute)).Any());

            foreach (PropertyInfo propertyInfo in propertiesToValidate)
            {
                var propertyErrors = new List<string>();
                TryValidateProperty(propertyInfo, propertyErrors);

                // If the errors have changed, save the property name to notify the update at the end of this method.
                bool errorsChanged = SetPropertyErrors(propertyInfo.Name, propertyErrors);
                if (errorsChanged && !propertiesWithChangedErrors.Contains(propertyInfo.Name))
                {
                    propertiesWithChangedErrors.Add(propertyInfo.Name);
                }
            }

            // Notify each property whose set of errors has changed since the last validation.  
            foreach (string propertyName in propertiesWithChangedErrors)
            {
                OnErrorsChanged(propertyName);
                OnPropertyChanged(string.Format(CultureInfo.CurrentCulture, "Item[{0}]", propertyName));
            }

            return _errors.Values.Count == 0;
        }

        /// <summary>
        /// Performs a validation of a property, adding the results in the propertyErrors list. 
        /// </summary>
        /// <param name="propertyInfo">The PropertyInfo of the property to validate</param>
        /// <param name="propertyErrors">A list containing the current error messages of the property.</param>
        /// <returns>True if the property is valid. Otherwise, false.</returns>
        private bool TryValidateProperty(PropertyInfo propertyInfo, List<string> propertyErrors)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(_entityToValidate) { MemberName = propertyInfo.Name };
            var propertyValue = propertyInfo.GetValue(_entityToValidate);

            // Validate the property
            bool isValid = Validator.TryValidateProperty(propertyValue, context, results);

            if (results.Any())
            {
                propertyErrors.AddRange(results.Select(c => c.ErrorMessage));
            }

            return isValid;
        }

        /// <summary>
        /// Updates the errors collection of the property.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="propertyNewErrors">The new collection of property errors.</param>
        /// <returns>True if the property errors have changed. Otherwise, false.</returns>
        private bool SetPropertyErrors(string propertyName, IList<string> propertyNewErrors)
        {
            bool errorsChanged = false;

            // If the property does not have errors, simply add them
            if (!_errors.ContainsKey(propertyName))
            {
                if (propertyNewErrors.Count > 0)
                {
                    _errors.Add(propertyName, new ReadOnlyCollection<string>(propertyNewErrors));
                    errorsChanged = true;
                }
            }
            else
            {
                // If the property has errors, check if the number of errors are different.
                // If the number of errors is the same, check if there are new ones
                if (propertyNewErrors.Count != _errors[propertyName].Count || _errors[propertyName].Intersect(propertyNewErrors).Count() != propertyNewErrors.Count)
                {
                    if (propertyNewErrors.Count > 0)
                    {
                        _errors[propertyName] = new ReadOnlyCollection<string>(propertyNewErrors);
                    }
                    else
                    {
                        _errors.Remove(propertyName);
                    }

                    errorsChanged = true;
                }
            }

            return errorsChanged;
        }

        /// <summary>
        /// Notifies listeners that a property value has changed.
        /// </summary>
        /// <param name="propertyName">Name of the property used to notify listeners.</param>
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Notifies listeners that the errors of a property have changed.
        /// </summary>
        /// <param name="propertyName">Name of the property used to notify listeners.</param>
        private void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }
    }
}
