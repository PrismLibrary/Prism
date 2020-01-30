using System.Collections.Generic;
using Xamarin.Forms;

namespace HelloWorld.Controls
{
    [ContentProperty(nameof(Children))]
    public class SectionGroup : Frame
    {
        public static readonly BindableProperty TitleProperty =
           BindableProperty.Create(nameof(Title), typeof(string), typeof(SectionGroup), null);

        private readonly StackLayout _children = new StackLayout();

        public SectionGroup()
        {
            var layout = new StackLayout();
            var label = new Label
            {
                BindingContext = this,
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                Margin = new Thickness(10, 0)
            };
            label.SetBinding(Label.TextProperty, nameof(Title));
            layout.Children.Add(label);
            layout.Children.Add(_children);
            Content = layout;
        }

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public new IList<View> Children => _children.Children;
    }
}
