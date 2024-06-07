using Prism.Common;

namespace Prism.Maui.Tests.Fixtures.Common;

public class MvvmHelperFixture
{
    /// <summary>
    /// This test was introduced to verify GH3143
    /// </summary>
    /// <a href="https://github.com/PrismLibrary/Prism/issues/3143">Git Hub Issue 3143</a>
    [Fact]
    public async Task GetCurrentPageFromFlyoutPageWithModalReturnsDetailPage()
    {
        // Given
        var flyout = new FlyoutPage
            { Flyout = new ContentPage { Title = "Title" }, Detail = new NavigationPage(), };
        var window = new PrismWindow { Page = flyout };
        await window.Navigation.PushModalAsync(new DialogContainerPage());

        // When
        var result = MvvmHelpers.GetCurrentPage(window.Page);

        // Then
        Assert.Equal(flyout.Detail, result);
    }

    /// <summary>
    /// This test was introduced to verify GH3143
    /// </summary>
    /// <a href="https://github.com/PrismLibrary/Prism/issues/3143">Git Hub Issue 3143</a>
    [Fact]
    public async Task GetCurrentPageFromComplexFlyoutPageWithModalReturnsCorrectPage()
    {
        // Given
        var expected = new ContentPage { Title = "D" };
        var navigationPage = new NavigationPage();
        await navigationPage.PushAsync(new ContentPage { Title = "A" });
        await navigationPage.PushAsync(new ContentPage { Title = "B" });
        await navigationPage.PushAsync(new ContentPage { Title = "C" });
        await navigationPage.PushAsync(expected);

        var window = new Window
        {
            Page = new FlyoutPage
                { Flyout = new ContentPage { Title = "Title" }, Detail = navigationPage, }
        };

        await window.Navigation.PushModalAsync(new DialogContainerPage());

        // When
        var result = MvvmHelpers.GetCurrentPage(window.Page);

        // Then
        Assert.Equal(expected, result);
    }

    /// <summary>
    /// This test was introduced to verify GH3143
    /// </summary>
    /// <a href="https://github.com/PrismLibrary/Prism/issues/3143">Git Hub Issue 3143</a>
    [Fact]
    public async Task GetCurrentPageFromNavigationPageWithModalReturnsContentPage()
    {
        // Given
        var expected = new ContentPage();
        var window = new Window { Page = new NavigationPage(expected) };
        await window.Navigation.PushModalAsync(new DialogContainerPage());

        // When
        var result = MvvmHelpers.GetCurrentPage(window.Page);

        // Then
        Assert.Equal(expected, result);
    }

    /// <summary>
    /// This test was introduced to verify GH3143
    /// </summary>
    /// <a href="https://github.com/PrismLibrary/Prism/issues/3143">Git Hub Issue 3143</a>
    [Fact]
    public async Task GetCurrentPageFromContentPageWithModalReturnsContentPage()
    {
        // Given
        var expected = new ContentPage { Title = "Title" };
        var window = new Window { Page = expected };
        await window.Navigation.PushModalAsync(new DialogContainerPage());

        // When
        var result = MvvmHelpers.GetCurrentPage(window.Page);

        // Then
        Assert.Equal(expected, result);
    }

    /// <summary>
    /// This test was introduced to verify GH3143
    /// </summary>
    /// <a href="https://github.com/PrismLibrary/Prism/issues/3143">Git Hub Issue 3143</a>
    [Fact(Skip = "System.InvalidOperationException\nBindableObject was not instantiated on a thread with a dispatcher nor does the current application have a dispatcher.")]
    public async Task GetCurrentPageFromTabbedPageWithModalReturnsContentPage()
    {
        // Given
        var expected = new ContentPage();
        var tabbedPage = new TabbedPage { Title = "Tab", Children = { expected }};
        var window = new Window { Page = tabbedPage };
        await window.Navigation.PushModalAsync(new DialogContainerPage());

        // When
        var result = MvvmHelpers.GetCurrentPage(window.Page);

        // Then
        Assert.Equal(expected, result);
    }

    /// <summary>
    /// This test was introduced to verify GH3143
    /// </summary>
    /// <a href="https://github.com/PrismLibrary/Prism/issues/3143">Git Hub Issue 3143</a>
    [Fact(Skip = "System.InvalidOperationException\nBindableObject was not instantiated on a thread with a dispatcher nor does the current application have a dispatcher.")]
    public async Task GetCurrentPageFromTabbedNavigationPageWithModalReturnsContentPage()
    {
        // Given
        var expected = new ContentPage();
        var navigationPage = new NavigationPage(expected);
        var tabbedPage = new TabbedPage { Title = "Tab", Children = { navigationPage }};
        var window = new Window { Page = tabbedPage };
        await window.Navigation.PushModalAsync(new DialogContainerPage());

        // When
        var result = MvvmHelpers.GetCurrentPage(window.Page);

        // Then
        Assert.Equal(expected, result);
    }
}
