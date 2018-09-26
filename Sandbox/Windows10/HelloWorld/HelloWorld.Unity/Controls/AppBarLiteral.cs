using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace Sample.Controls
{
[ContentProperty(Name = nameof(LiteralContent))]
public sealed class AppBarLiteral : AppBarSeparator
{
    public AppBarLiteral()
    {
        DefaultStyleKey = typeof(AppBarLiteral);
    }

    public object LiteralContent
    {
        get { return (object)GetValue(LiteralContentProperty); }
        set { SetValue(LiteralContentProperty, value); }
    }
    public static readonly DependencyProperty LiteralContentProperty =
        DependencyProperty.Register(nameof(LiteralContent), typeof(object),
            typeof(AppBarLiteral), new PropertyMetadata(null));
}
}
