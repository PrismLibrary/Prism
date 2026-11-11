namespace Prism.DryIoc.Maui.Tests.Mocks.Views;

public class MockContentRegionPage : ContentPage
{
    public MockContentRegionPage()
    {
        ContentRegion = new ContentView();
        ContentRegion.SetValue(Prism.Navigation.Regions.Xaml.RegionManager.RegionNameProperty, nameof(ContentRegion));

#pragma warning disable CS0618 // Regression coverage for Frame-backed regions must retain Frame.
        FrameRegion = new Frame();
#pragma warning restore CS0618
        FrameRegion.SetValue(Prism.Navigation.Regions.Xaml.RegionManager.RegionNameProperty, nameof(FrameRegion));

        LayoutRegion = new StackLayout();
        LayoutRegion.SetValue(Prism.Navigation.Regions.Xaml.RegionManager.RegionNameProperty, nameof(LayoutRegion));

        Content = new StackLayout
        {
            ContentRegion,
            FrameRegion,
            LayoutRegion
        };
    }

    public ContentView ContentRegion { get; }

#pragma warning disable CS0618 // Expose the legacy control exercised by the Frame region tests.
    public Frame FrameRegion { get; }
#pragma warning restore CS0618

    public Layout LayoutRegion { get; }
}
