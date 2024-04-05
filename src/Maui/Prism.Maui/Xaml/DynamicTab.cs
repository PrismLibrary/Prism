namespace Prism.Xaml;

public static class DynamicTab
{
    public static readonly BindableProperty TitleProperty =
        BindableProperty.CreateAttached(nameof(Page.Title), typeof(string), typeof(DynamicTab), null, BindingMode.OneWay, defaultValueCreator: CreateDefaultTitle);

    public static string GetTitle(BindableObject bindable) =>
        (string)bindable.GetValue(TitleProperty);

    public static void SetTitle(BindableObject bindable, string title) =>
        bindable.SetValue(TitleProperty, title);

    private static object CreateDefaultTitle(BindableObject bindable)
    {
        if (bindable is Page page)
        {
            return page.Title;
        }

        return null;
    }

    public static readonly BindableProperty IconImageSourceProperty =
        BindableProperty.CreateAttached(nameof(Page.IconImageSource), typeof(ImageSource), typeof(DynamicTab), null, BindingMode.OneWay, defaultValueCreator: CreateDefaultIconImageSource);

    public static ImageSource GetIconImageSource(BindableObject bindable) =>
        (ImageSource)bindable.GetValue(IconImageSourceProperty);

    public static void SetIconImageSource(BindableObject bindable, ImageSource imageSource) =>
        bindable.SetValue(IconImageSourceProperty, imageSource);

    private static object CreateDefaultIconImageSource(BindableObject bindable)
    {
        if (bindable is Page page)
        {
            return page.IconImageSource;
        }

        return null;
    }
}
