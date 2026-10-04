using Prism.Navigation;

namespace Prism.Maui.Tests.Mocks.Views;

public class RelativeFlyoutPageMock : FlyoutPage, IInitialize
{
    public RelativeNavigationPageMock OriginalDetail { get; } = new();

    public RelativeFlyoutPageMock() : this(false)
    {
    }

    protected RelativeFlyoutPageMock(bool navigationDetail)
    {
        Flyout = new ContentPage { Title = "Menu" };
        Detail = navigationDetail ? new NavigationPage(OriginalDetail) : OriginalDetail;
    }

    public void Initialize(INavigationParameters parameters)
    {
        if (parameters.TryGetValue<Action<RelativeFlyoutPageMock>>("flyoutObserver", out var observer))
            observer(this);
        if (parameters.TryGetValue<bool>("failFlyoutInitialize", out var fail) && fail)
            throw new InvalidOperationException("Flyout initialization failed.");
    }
}
