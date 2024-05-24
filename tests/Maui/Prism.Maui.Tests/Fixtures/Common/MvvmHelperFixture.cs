using System.Collections;
using Prism.Common;
using Prism.Maui.Tests.Mocks.Views;

namespace Prism.Maui.Tests.Fixtures.Common;

public class MvvmHelperFixture
{
    /// <summary>
    /// This test was introduced to verify GH3143
    /// </summary>
    /// <a href="https://github.com/PrismLibrary/Prism/issues/3143">Git Hub Issue 3143</a>
    /// <param name="window">The window representing the root of the navigation.</param>
    /// <param name="page">The actual page we expect to return.</param>
    /// <param name="modal">The model page we are pushing on the stack that causes the problem.</param>
    [Theory]
    [ClassData(typeof(GetCurrentPageTestData))]
    public async Task GetCurrentPageWithModalReturnsPrismWindowPage(Window window, Page page, Page modal)
    {
        // Given
        await window.Navigation.PushModalAsync(modal);

        // When
        var result = MvvmHelpers.GetCurrentPage(window.Page);

        // Then
        Assert.Equal(result, page);
    }

    private class GetCurrentPageTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            ContentPage contentPage = new ContentPage { Title = "Title" };
            NavigationPage navigationPage = new NavigationPage();
            // TabbedPage tabbedPage = new TabbedPage { CurrentPage = contentPage };
            FlyoutPage flyout = new FlyoutPage { Flyout = contentPage, Detail = new NavigationPage(), };
            DialogContainerPage dialogContainerPage = new DialogContainerPage();
            yield return new object[] { PrismWindow(contentPage), contentPage, dialogContainerPage };
            yield return new object[] { PrismWindow(navigationPage), navigationPage, dialogContainerPage };
            // yield return new object[] { PrismWindow(tabbedPage), tabbedPage, dialogContainerPage };
            yield return new object[] { PrismWindow(flyout), flyout.Detail, dialogContainerPage };
        }

        private static PrismWindow PrismWindow(Page page) => new() { Page = page };

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
