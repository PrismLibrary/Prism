namespace Prism.DryIoc.Maui.Tests.Mocks.ViewModels;

internal class MockContentRegionPageViewModel : IInitialize
{
    private IRegionManager _regionManager { get; }

    public MockContentRegionPageViewModel(IRegionManager regionManager)
    {
        _regionManager = regionManager;
    }

    public void Initialize(INavigationParameters parameters)
    {
        _regionManager.RequestNavigate("ContentRegion", "MockRegionViewA", parameters);
    }
}
