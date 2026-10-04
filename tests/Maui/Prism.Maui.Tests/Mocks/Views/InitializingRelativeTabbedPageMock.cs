using Prism.Navigation;

namespace Prism.Maui.Tests.Mocks.Views;

public class InitializingRelativeTabbedPageMock : TabbedPage, IInitialize
{
    public void Initialize(INavigationParameters parameters)
    {
        Children.Add(new ContentPage { Title = "First" });
        Children.Add(new ContentPage { Title = "Second" });
    }
}
