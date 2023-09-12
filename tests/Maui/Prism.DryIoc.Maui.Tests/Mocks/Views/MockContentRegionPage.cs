namespace Prism.DryIoc.Maui.Tests.Mocks.Views;

public class MockContentRegionPage : ContentPage
{
    public MockContentRegionPage()
    {
        ContentRegion = new ContentView();
        ContentRegion.SetValue(Prism.Navigation.Regions.Xaml.RegionManager.RegionNameProperty, nameof(ContentRegion));

        FrameRegion = new Frame();
        FrameRegion.SetValue(Prism.Navigation.Regions.Xaml.RegionManager.RegionNameProperty, nameof(FrameRegion));

        Content = new StackLayout
        {
            ContentRegion,
            FrameRegion
        };
    }

    public ContentView ContentRegion { get; }

    public Frame FrameRegion { get; }
}
