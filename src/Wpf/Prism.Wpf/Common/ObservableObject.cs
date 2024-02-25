using System.ComponentModel;

namespace Prism.Common
{
    /// <summary>
    /// Class that wraps an object, so that other classes can notify for Change events. Typically, this class is set as
    /// a Dependency Property on DependencyObjects, and allows other classes to observe any changes in the Value.
    /// </summary>
    /// <remarks>
    /// This class is required, because in Silverlight, it's not possible to receive Change notifications for Dependency properties that you do not own.
    /// </remarks>
    /// <typeparam name="T">The type of the property that's wrapped in the Observable object</typeparam>
    public partial class ObservableObject<T> : FrameworkElement, INotifyPropertyChanged
    {
        /// <summary>
        /// Identifies the Value property of the ObservableObject
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Justification = "This is the pattern for WPF dependency properties")]
        public static readonly DependencyProperty ValueProperty =
                DependencyProperty.Register(nameof(Value), typeof(T), typeof(ObservableObject<T>), new PropertyMetadata(null, ValueChangedCallback));

        /// <summary>
        /// Event that gets invoked when the Value property changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The value that's wrapped inside the ObservableObject.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
        public T Value
        {
            get => (T)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        private static void ValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ObservableObject<T> thisInstance = ((ObservableObject<T>)d);
            thisInstance.PropertyChanged?.Invoke(thisInstance, new PropertyChangedEventArgs(nameof(Value)));
        }
    }
}
