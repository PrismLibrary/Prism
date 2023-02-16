namespace Prism.Maui.Tests.Mocks.Views;

public class NavigationPageEmptyMock_Reused : NavigationPageEmptyMock, INavigationPageOptions
{
    public bool ClearNavigationStackOnNavigation => false;

    public NavigationPageEmptyMock_Reused() : base()
    {

    }

    public NavigationPageEmptyMock_Reused(PageNavigationEventRecorder recorder) : base(recorder)
    {

    }
}
