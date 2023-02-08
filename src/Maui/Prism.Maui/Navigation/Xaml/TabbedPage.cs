namespace Prism.Navigation.Xaml;

public static class TabbedPage
{
    public static BindableProperty TitleBindingSourceProperty =
        BindableProperty.CreateAttached("TitleBindingSource", typeof(TabBindingSource), typeof(TabbedPage), TabBindingSource.RootPage);

    public static BindableProperty TitleProperty =
        BindableProperty.CreateAttached("Title", typeof(string), typeof(TabbedPage), null);

    public static BindableProperty IconImageSourceProperty =
        BindableProperty.CreateAttached("IconImageSource", typeof(ImageSource), typeof(TabbedPage), null);

    public static TabBindingSource GetTitleBindingSource(Page page) =>
        (TabBindingSource)page.GetValue(TitleBindingSourceProperty);

    public static void SetTitleBindingSource(Page page, TabBindingSource bindingSource) =>
        page.SetValue(TitleBindingSourceProperty, bindingSource);

    public static string GetTitle(Page page) =>
        (string)page.GetValue(TitleProperty);

    public static void SetTitle(Page page, string title) =>
        page.SetValue(TitleProperty, title);

    public static ImageSource GetIconImageSource(Page page) =>
        (ImageSource)page.GetValue(IconImageSourceProperty);

    public static void SetIconImageSource(Page page, ImageSource imageSource) =>
        page.SetValue(IconImageSourceProperty, imageSource);
}