using Microsoft.Maui.Controls.Internals;
using Prism.Maui.Tests.Mocks;

#pragma warning disable CS0612 // Type or member is obsolete
[assembly: Dependency(typeof(MockResourcesProvider))]
[assembly: Dependency(typeof(MockFontNamedSizeService))]
#pragma warning restore CS0612 // Type or member is obsolete

namespace Prism.Maui.Tests.Mocks;

[Obsolete]
internal class MockResourcesProvider : ISystemResourcesProvider
{
    public IResourceDictionary GetSystemResources()
    {
        var dictionary = new ResourceDictionary();
        Style style;
        style = new Style(typeof(Label));
        dictionary[Device.Styles.BodyStyleKey] = style;

        style = new Style(typeof(Label));
        style.Setters.Add(Label.FontSizeProperty, 50);
        dictionary[Device.Styles.TitleStyleKey] = style;

        style = new Style(typeof(Label));
        style.Setters.Add(Label.FontSizeProperty, 40);
        dictionary[Device.Styles.SubtitleStyleKey] = style;

        style = new Style(typeof(Label));
        style.Setters.Add(Label.FontSizeProperty, 30);
        dictionary[Device.Styles.CaptionStyleKey] = style;

        style = new Style(typeof(Label));
        style.Setters.Add(Label.FontSizeProperty, 20);
        dictionary[Device.Styles.ListItemTextStyleKey] = style;

        style = new Style(typeof(Label));
        style.Setters.Add(Label.FontSizeProperty, 10);
        dictionary[Device.Styles.ListItemDetailTextStyleKey] = style;

        return dictionary;
    }
}

[Obsolete]
public class MockFontNamedSizeService : IFontNamedSizeService
{
    public double GetNamedSize(NamedSize size, Type targetElement, bool useOldSizes)
    {
        return size switch
        {
            NamedSize.Default => 12,// new MockFontManager().DefaultFontSize,
            NamedSize.Micro => (double)4,
            NamedSize.Small => (double)8,
            NamedSize.Medium => (double)12,
            NamedSize.Large => (double)16,
            _ => throw new ArgumentOutOfRangeException(nameof(size)),
        };
    }
}