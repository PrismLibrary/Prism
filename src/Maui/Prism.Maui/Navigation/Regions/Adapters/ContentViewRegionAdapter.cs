namespace Prism.Navigation.Regions.Adapters;

/// <summary>
/// Adapter that creates a new <see cref="SingleActiveRegion"/> and monitors its
/// active view to set it on the adapted <see cref="ContentView"/>.
/// </summary>
public class ContentViewRegionAdapter : ContentViewRegionAdapter<ContentView>
{
    /// <summary>
    /// Initializes a new instance of <see cref="ContentViewRegionAdapter"/>.
    /// </summary>
    /// <param name="regionBehaviorFactory">The factory used to create the region behaviors to attach to the created regions.</param>
    public ContentViewRegionAdapter(IRegionBehaviorFactory regionBehaviorFactory)
        : base(regionBehaviorFactory)
    {
    }
}
