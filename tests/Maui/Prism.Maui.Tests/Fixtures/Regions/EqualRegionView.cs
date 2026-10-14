using Microsoft.Maui.Controls;

namespace Prism.Maui.Tests.Fixtures.Regions;

internal sealed class EqualRegionView : ContentView
{
    public override bool Equals(object obj)
    {
        return obj is EqualRegionView;
    }

    public override int GetHashCode()
    {
        return typeof(EqualRegionView).GetHashCode();
    }
}
