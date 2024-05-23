using Prism.Common;

namespace Prism.Maui.Tests.Fixtures.Common;

public class MvvmHelperFixture
{
    [Fact]
    public async Task GetCurrentPageWithModalReturnsPrismWindowPage()
    {
        // Given
        var contentPage = new ContentPage();
        var prismWindow = new PrismWindow { Page = contentPage };
        await prismWindow.Navigation.PushModalAsync(new DialogContainerPage());

        // When
        var result = MvvmHelpers.GetCurrentPage(prismWindow.Page);

        // Then
        Assert.Equal(result, contentPage);
    }
}
