namespace Prism.Navigation.Xaml;

/// <summary>
/// Provides attached properties for tabbed page navigation and appearance in .NET MAUI applications.
/// </summary>
public static class TabbedPage
{
    /// <summary>
    /// Identifies the <c>TitleBindingSource</c> attached property.
    /// Determines the source for the tab title binding.
    /// </summary>
    public static BindableProperty TitleBindingSourceProperty =
        BindableProperty.CreateAttached(
            "TitleBindingSource",
            typeof(TabBindingSource),
            typeof(TabbedPage),
            TabBindingSource.RootPage);

    /// <summary>
    /// Identifies the <c>Title</c> attached property.
    /// Sets the title for a tab in a <see cref="Microsoft.Maui.Controls.TabbedPage"/>.
    /// </summary>
    public static BindableProperty TitleProperty =
        BindableProperty.CreateAttached(
            "Title",
            typeof(string),
            typeof(TabbedPage),
            null);

    /// <summary>
    /// Identifies the <c>IconImageSource</c> attached property.
    /// Sets the icon image source for a tab in a <see cref="Microsoft.Maui.Controls.TabbedPage"/>.
    /// </summary>
    public static BindableProperty IconImageSourceProperty =
        BindableProperty.CreateAttached(
            "IconImageSource",
            typeof(ImageSource),
            typeof(TabbedPage),
            null);

    /// <summary>
    /// Gets the value of the <c>TitleBindingSource</c> attached property from the specified <see cref="Page"/>.
    /// </summary>
    /// <param name="page">The page from which to get the property value.</param>
    /// <returns>The <see cref="TabBindingSource"/> value.</returns>
    public static TabBindingSource GetTitleBindingSource(Page page) =>
        (TabBindingSource)page.GetValue(TitleBindingSourceProperty);

    /// <summary>
    /// Sets the value of the <c>TitleBindingSource</c> attached property on the specified <see cref="Page"/>.
    /// </summary>
    /// <param name="page">The page on which to set the property value.</param>
    /// <param name="bindingSource">The <see cref="TabBindingSource"/> value to set.</param>
    public static void SetTitleBindingSource(Page page, TabBindingSource bindingSource) =>
        page.SetValue(TitleBindingSourceProperty, bindingSource);

    /// <summary>
    /// Gets the value of the <c>Title</c> attached property from the specified <see cref="Page"/>.
    /// </summary>
    /// <param name="page">The page from which to get the property value.</param>
    /// <returns>The title string.</returns>
    public static string GetTitle(Page page) =>
        (string)page.GetValue(TitleProperty);

    /// <summary>
    /// Sets the value of the <c>Title</c> attached property on the specified <see cref="Page"/>.
    /// </summary>
    /// <param name="page">The page on which to set the property value.</param>
    /// <param name="title">The title string to set.</param>
    public static void SetTitle(Page page, string title) =>
        page.SetValue(TitleProperty, title);

    /// <summary>
    /// Gets the value of the <c>IconImageSource</c> attached property from the specified <see cref="Page"/>.
    /// </summary>
    /// <param name="page">The page from which to get the property value.</param>
    /// <returns>The <see cref="ImageSource"/> for the tab icon.</returns>
    public static ImageSource GetIconImageSource(Page page) =>
        (ImageSource)page.GetValue(IconImageSourceProperty);

    /// <summary>
    /// Sets the value of the <c>IconImageSource</c> attached property on the specified <see cref="Page"/>.
    /// </summary>
    /// <param name="page">The page on which to set the property value.</param>
    /// <param name="imageSource">The <see cref="ImageSource"/> to set as the tab icon.</param>
    public static void SetIconImageSource(Page page, ImageSource imageSource) =>
        page.SetValue(IconImageSourceProperty, imageSource);
}
