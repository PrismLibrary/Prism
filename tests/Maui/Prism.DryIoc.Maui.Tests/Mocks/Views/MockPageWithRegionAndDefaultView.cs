namespace Prism.DryIoc.Maui.Tests.Mocks.Views;

public class MockPageWithRegionAndDefaultView : ContentPage
{
    public MockPageWithRegionAndDefaultView()
    {
        var view = new ContentView();
        view.SetValue(Prism.Navigation.Regions.Xaml.RegionManager.RegionNameProperty, "Demo");
        view.SetValue(Prism.Navigation.Regions.Xaml.RegionManager.DefaultViewProperty, "MockRegionViewA");
        Content = view;
    }
}
