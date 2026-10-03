namespace Prism.Navigation.Regions;

internal class RegionPage : ContentPage
{
    public RegionPage(RegionPageViewModel viewModel)
    {
        BindingContext = viewModel;
        this.SetBinding(TitleProperty, static (RegionPageViewModel vm) => vm.Title, BindingMode.OneWay);
    }
}
