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
        FlyoutPage flyout = new FlyoutPage
            { Flyout = new ContentPage { Title = "Title" }, Detail = new NavigationPage(), };
        PrismWindow window = new PrismWindow { Page = flyout };
        await window.Navigation.PushModalAsync(new DialogContainerPage());

        // When
        var result = MvvmHelpers.GetCurrentPage(window.Page);

        // Then
        Assert.Equal(result, flyout.Detail);
    }

    /// <summary>
    /// This test was introduced to verify GH3143
    /// </summary>
    /// <a href="https://github.com/PrismLibrary/Prism/issues/3143">Git Hub Issue 3143</a>
    [Fact]
    public async Task GetCurrentPageFromComplexFlyoutPageWithModalReturnsCorrectPage()
    {
        // Given
        ContentPage current = new ContentPage { Title = "D" };
        var navigationPage = new NavigationPage();
        await navigationPage.PushAsync(new ContentPage { Title = "A" });
        await navigationPage.PushAsync(new ContentPage { Title = "B" });
        await navigationPage.PushAsync(new ContentPage { Title = "C" });
        await navigationPage.PushAsync(current);

        PrismWindow window = new PrismWindow
        {
            Page = new FlyoutPage
                { Flyout = new ContentPage { Title = "Title" }, Detail = navigationPage, }
        };

        await window.Navigation.PushModalAsync(new DialogContainerPage());

        // When
        var result = MvvmHelpers.GetCurrentPage(window.Page);

        // Then
        Assert.Equal(result, current);
    }

    /// <summary>
    /// This test was introduced to verify GH3143
    /// </summary>
    /// <a href="https://github.com/PrismLibrary/Prism/issues/3143">Git Hub Issue 3143</a>
    [Fact]
    public async Task GetCurrentPageFromNavigationPageWithModalReturnsContentPage()
    {
        // Given
        var contentPage = new ContentPage();
        NavigationPage navigationPage = new NavigationPage(contentPage);
        PrismWindow window = new PrismWindow { Page = navigationPage };
        await window.Navigation.PushModalAsync(new DialogContainerPage());

        // When
        var result = MvvmHelpers.GetCurrentPage(window.Page);

        // Then
        Assert.Equal(result, contentPage);
    }

    /// <summary>
    /// This test was introduced to verify GH3143
    /// </summary>
    /// <a href="https://github.com/PrismLibrary/Prism/issues/3143">Git Hub Issue 3143</a>
    [Fact]
    public async Task GetCurrentPageFromContentPageWithModalReturnsContentPage()
    {
        // Given
        ContentPage contentPage = new ContentPage { Title = "Title" };
        PrismWindow window = new PrismWindow { Page = contentPage };
        await window.Navigation.PushModalAsync(new DialogContainerPage());

        // When
        var result = MvvmHelpers.GetCurrentPage(window.Page);

        // Then
        Assert.Equal(result, contentPage);
    }
}
