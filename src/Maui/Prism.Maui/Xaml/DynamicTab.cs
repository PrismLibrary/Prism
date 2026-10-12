namespace Prism.Xaml;

/// <summary>Provides attached title and icon values for dynamically created tabs.</summary>
/// <remarks>When no attached value is set on a page, its own title or icon supplies the default.</remarks>
public static class DynamicTab
{
    /// <summary>Identifies the attached title used for a dynamic tab.</summary>
    public static readonly BindableProperty TitleProperty =
        BindableProperty.CreateAttached(nameof(Page.Title), typeof(string), typeof(DynamicTab), null, BindingMode.OneWay, defaultValueCreator: CreateDefaultTitle);

    /// <summary>Gets the title to display for a dynamic tab.</summary>
    /// <param name="bindable">The object carrying the attached title.</param>
    /// <returns>The attached title, defaulting to the page's title for a page.</returns>
    public static string GetTitle(BindableObject bindable) =>
        (string)bindable.GetValue(TitleProperty);

    /// <summary>Sets the title to display for a dynamic tab.</summary>
    /// <param name="bindable">The object carrying the attached title.</param>
    /// <param name="title">The tab title.</param>
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

    /// <summary>Identifies the attached icon used for a dynamic tab.</summary>
    public static readonly BindableProperty IconImageSourceProperty =
        BindableProperty.CreateAttached(nameof(Page.IconImageSource), typeof(ImageSource), typeof(DynamicTab), null, BindingMode.OneWay, defaultValueCreator: CreateDefaultIconImageSource);

    /// <summary>Gets the icon to display for a dynamic tab.</summary>
    /// <param name="bindable">The object carrying the attached icon.</param>
    /// <returns>The attached icon, defaulting to the page's icon for a page.</returns>
    public static ImageSource GetIconImageSource(BindableObject bindable) =>
        (ImageSource)bindable.GetValue(IconImageSourceProperty);

    /// <summary>Sets the icon to display for a dynamic tab.</summary>
    /// <param name="bindable">The object carrying the attached icon.</param>
    /// <param name="imageSource">The tab icon.</param>
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
