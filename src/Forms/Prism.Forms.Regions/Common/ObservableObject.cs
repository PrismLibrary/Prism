using System.ComponentModel;
using Xamarin.Forms;

namespace Prism.Common
{
    // TODO: Refactor this... we probably do not need this for Xamarin
    /// <summary>
    /// Class that wraps an object, so that other classes can notify for Change events. Typically, this class is set as
    /// a Bindable Property on BindableObjects, and allows other classes to observe any changes in the Value.
    /// </summary>
    /// <typeparam name="T">The type of the property that's wrapped in the Observable object</typeparam>
    public class ObservableObject<T> : BindableObject, INotifyPropertyChanged
    {
        /// <summary>
        /// Identifies the Value property of the ObservableObject
        /// </summary>
        public static readonly BindableProperty ValueProperty =
                BindableProperty.Create(nameof(Value), typeof(T), typeof(ObservableObject<T>), default);

        /// <summary>
        /// The value that's wrapped inside the ObservableObject.
        /// </summary>
        public T Value
        {
            get => (T)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
    }
}
