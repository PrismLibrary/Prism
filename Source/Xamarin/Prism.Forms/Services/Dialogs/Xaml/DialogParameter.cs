using Xamarin.Forms;

namespace Prism.Services.Dialogs.Xaml
{
    public class DialogParameter : BindableObject
    {
        public string Key { get; set; }

        public static readonly BindableProperty ValueProperty = 
            BindableProperty.Create(nameof(Value),
                typeof(object),
                typeof(DialogParameter));

        public object Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
    }
}
