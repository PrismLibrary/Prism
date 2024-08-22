namespace Prism.DryIoc.Maui.Tests.Mocks.Views;

public class MockContentRegionPage : ContentPage
{
    public MockContentRegionPage()
    {
        ContentRegion = new ContentView();
        ContentRegion.SetValue(Prism.Navigation.Regions.Xaml.RegionManager.RegionNameProperty, nameof(ContentRegion));

        FrameRegion = new Frame();
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

    public Frame FrameRegion { get; }

    public Layout LayoutRegion { get; }
}
