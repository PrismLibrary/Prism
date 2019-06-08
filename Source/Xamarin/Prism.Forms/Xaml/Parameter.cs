using Xamarin.Forms;

namespace Prism.Xaml
{
    public class Parameter : BindableObject
    {
        public string Key { get; set; }

        public static readonly BindableProperty ValueProperty =
            BindableProperty.Create(nameof(Value), typeof(object), typeof(Parameter));

        public object Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
    }
}
