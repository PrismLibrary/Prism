using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Prism.Mvvm
{
    /// <summary>
    /// Implementation of <see cref="INotifyPropertyChanged"/> to simplify models.
    /// </summary>
    public abstract class BindableBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Checks if a property already matches a desired value. Sets the property and
		/// notifies listeners only when necessary.
		/// </summary>
		/// <typeparam name="T">Type of the property.</typeparam>
		/// <param name="storage">Reference to a property with both getter and setter.</param>
		/// <param name="value">Desired value for the property.</param>
		/// <param name="propertyName">Name of the property used to notify listeners. This
		/// value is optional and can be provided automatically when invoked from compilers that
		/// support CallerMemberName.</param>
		/// <returns>True if the value was changed, false if the existing value matched the
		/// desired value.</returns>
		protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
		{
			if (EqualityComparer<T>.Default.Equals(storage, value)) return false;

			storage = value;
			RaisePropertyChanged(propertyName);

			return true;
		}

		/// <summary>
		/// Checks if a property already matches a desired value. Sets the property and
		/// notifies listeners only when necessary.
		/// </summary>
		/// <typeparam name="T">Type of the property.</typeparam>
		/// <param name="storage">Reference to a property with both getter and setter.</param>
		/// <param name="value">Desired value for the property.</param>
		/// <param name="propertyName">Name of the property used to notify listeners. This
		/// value is optional and can be provided automatically when invoked from compilers that
		/// support CallerMemberName.</param>
		/// <param name="onChanged">Action that is called after the property value has been changed.</param>
		/// <returns>True if the value was changed, false if the existing value matched the
		/// desired value.</returns>
		protected virtual bool SetProperty<T>(ref T storage, T value, Action onChanged, [CallerMemberName] string propertyName = null)
		{
			if (EqualityComparer<T>.Default.Equals(storage, value)) return false;

			storage = value;
			onChanged?.Invoke();
			RaisePropertyChanged(propertyName);

			return true;
		}

        /// <summary>
        /// Checks if the property on the given object already matches a desired value. Sets the property and
        /// notifies listeners only when necessary.
        /// </summary>
        /// <typeparam name="TObject">Type of the nested object.</typeparam>
        /// <typeparam name="TProperty">Type of the property.</typeparam>
        /// <param name="objectWithProperty">Reference to a nested object on which the property shall be set.</param>
        /// <param name="propertyName">The name of the property. The object must have a property with this name
        /// otherwise an <see cref="ArgumentNullException"/> will be thrown. By default it is expected that
        /// the name of the ViewModel-property matches the name of the nested property</param>
        /// <param name="value">Desired value for the property.</param>
        /// <returns>True if the value was changed, false if the existing value matched the
        /// desired value.</returns>
        protected virtual bool SetNestedProperty<TObject, TProperty>(TObject objectWithProperty, TProperty value, [CallerMemberName]string propertyName = null)
        {
            if (objectWithProperty == null)
                throw new ArgumentNullException("objectWithProperty");

            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException("propertyName");

            PropertyInfo pInfo = objectWithProperty.GetType().GetProperty(propertyName, typeof(TProperty));

            if (pInfo == null)
                throw new ArgumentException($"Could not find a property named '{propertyName}' " +
                    $"with type {typeof(TProperty)} in class {typeof(TObject)}");

            TProperty currentPropertyValue = (TProperty)pInfo.GetValue(objectWithProperty);

            if (EqualityComparer<TProperty>.Default.Equals(currentPropertyValue, value)) return false;

            pInfo.SetValue(objectWithProperty, value);
            RaisePropertyChanged(propertyName);

            return true;
        }

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">Name of the property used to notify listeners. This
        /// value is optional and can be provided automatically when invoked from compilers
        /// that support <see cref="CallerMemberNameAttribute"/>.</param>
        protected void RaisePropertyChanged([CallerMemberName]string propertyName = null)
		{
            //TODO: when we remove the old OnPropertyChanged method we need to uncomment the below line
            //OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
#pragma warning disable CS0618 // Type or member is obsolete
            OnPropertyChanged(propertyName);
#pragma warning restore CS0618 // Type or member is obsolete
        }

		/// <summary>
		/// Notifies listeners that a property value has changed.
		/// </summary>
		/// <param name="propertyName">Name of the property used to notify listeners. This
		/// value is optional and can be provided automatically when invoked from compilers
		/// that support <see cref="CallerMemberNameAttribute"/>.</param>
		[Obsolete("Please use the new RaisePropertyChanged method. This method will be removed to comply wth .NET coding standards. If you are overriding this method, you should overide the OnPropertyChanged(PropertyChangedEventArgs args) signature instead.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

		/// <summary>
		/// Raises this object's PropertyChanged event.
		/// </summary>
		/// <param name="args">The PropertyChangedEventArgs</param>
		protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
		{
			PropertyChanged?.Invoke(this, args);
		}

		/// <summary>
		/// Raises this object's PropertyChanged event.
		/// </summary>
		/// <typeparam name="T">The type of the property that has a new value</typeparam>
		/// <param name="propertyExpression">A Lambda expression representing the property that has a new value.</param>
		[Obsolete("Please use RaisePropertyChanged(nameof(PropertyName)) instead. Expressions are slower, and the new nameof feature eliminates the magic strings.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        protected virtual void OnPropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            var propertyName = PropertySupport.ExtractPropertyName(propertyExpression);
            OnPropertyChanged(propertyName);
        }
    }
}
