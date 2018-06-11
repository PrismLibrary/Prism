using Xamarin.Forms;

namespace Prism.Navigation.Xaml
{
    public class NavigationParameter : BindableObject
    {
        public string Key { get; set; }

        public static readonly BindableProperty ValueProperty = BindableProperty.Create(nameof(Value),
            typeof(object),
            typeof(NavigationParameter));

        /// <summary>
        /// Navigation Parameter Value. This is a bindable property.
        /// </summary>
        public object Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
    }
}