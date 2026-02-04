using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Prism.Mvvm
{
    /// <summary>
    /// Implementation of <see cref="INotifyPropertyChanged"/> to simplify models.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="BindableBase"/> is a base class designed for ViewModel classes in MVVM applications.
    /// It provides a convenient implementation of <see cref="INotifyPropertyChanged"/> that reduces boilerplate code
    /// when creating ViewModels that need to notify views of property changes.
    /// </para>
    /// <para>
    /// The class provides helper methods like <see cref="SetProperty{T}(ref T, T, string)"/> that automatically
    /// handle equality comparison and notification, ensuring that listeners are only notified when values actually change.
    /// </para>
    /// </remarks>
    public abstract class BindableBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

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
        /// <remarks>
        /// This method uses <see cref="EqualityComparer{T}.Default"/> to compare the current storage value with the new value.
        /// Only if they differ will the property be updated and listeners notified.
        /// </remarks>
        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
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
        /// <remarks>
        /// This method is useful when you need to perform additional logic when a property changes,
        /// such as recalculating a derived property or triggering a command.
        /// </remarks>
        protected virtual bool SetProperty<T>(ref T storage, T value, Action? onChanged,
            [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value)) return false;

            storage = value;
            onChanged?.Invoke();
            RaisePropertyChanged(propertyName);

            return true;
        }

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">Name of the property used to notify listeners. This
        /// value is optional and can be provided automatically when invoked from compilers
        /// that support <see cref="CallerMemberNameAttribute"/>.</param>
        /// <remarks>
        /// This method is typically called automatically by <see cref="SetProperty{T}(ref T, T, string)"/>,
        /// but can also be called directly to notify of changes to computed properties.
        /// </remarks>
        protected void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="args">The PropertyChangedEventArgs</param>
        /// <remarks>
        /// Override this method to intercept property change notifications before they are broadcast to subscribers.
        /// </remarks>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChanged?.Invoke(this, args);
        }
    }
}
