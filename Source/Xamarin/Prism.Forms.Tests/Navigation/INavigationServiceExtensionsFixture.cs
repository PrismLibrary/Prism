using Prism.Common;
using Prism.Forms.Tests.Mocks;
using Prism.Forms.Tests.Mocks.Views;
using Prism.Navigation;
using System.Threading.Tasks;
using Xunit;

namespace Prism.Forms.Tests.Navigation
{
    public class INavigationServiceExtensionsFixture
    {
        [Fact]
        public async Task PopToRootAsync_PopsToRoot()
        {
            var navigationService = new PageNavigationServiceMock(null, null, null);
            var rootPage = new Xamarin.Forms.NavigationPage();
            ((IPageAware)navigationService).Page = rootPage;

            var page1 = new ContentPageMock() { Title = "Page 1" };
            await rootPage.Navigation.PushAsync(page1);

            await rootPage.Navigation.PushAsync(new ContentPageMock() { Title = "Page 2" });
            await rootPage.Navigation.PushAsync(new ContentPageMock() { Title = "Page 3" });
            await rootPage.Navigation.PushAsync(new ContentPageMock() { Title = "Page 4" });

            Assert.True(rootPage.Navigation.NavigationStack.Count == 4);

            await navigationService.PopToRootAsync();

            Assert.Equal(1, rootPage.Navigation.NavigationStack.Count);
            Assert.Equal(page1, rootPage.Navigation.NavigationStack[0]);
        }

        [Fact]
        public async Task PopToRootAsync_PopsToRoot_Destroy()
        {
            var recorder = new PageNavigationEventRecorder();
            var navigationService = new PageNavigationServiceMock(null, null, null);
            var rootPage = new Xamarin.Forms.NavigationPage();
            ((IPageAware)navigationService).Page = rootPage;

            var page1 = new ContentPageMock(recorder) { Title = "Page 1" };
            await rootPage.Navigation.PushAsync(page1);

            var page2 = new ContentPageMock(recorder) { Title = "Page 2" };
            var page2ViewModel = page2.BindingContext;
            await rootPage.Navigation.PushAsync(page2);

            var page3 = new ContentPageMock(recorder) { Title = "Page 3" };
            var page3ViewModel = page3.BindingContext;
            await rootPage.Navigation.PushAsync(page3);

            var page4 = new ContentPageMock(recorder) { Title = "Page 4" };
            var page4ViewModel = page4.BindingContext;
            await rootPage.Navigation.PushAsync(page4);

            Assert.True(rootPage.Navigation.NavigationStack.Count == 4);

            await navigationService.PopToRootAsync();

            Assert.Equal(1, rootPage.Navigation.NavigationStack.Count);
            Assert.Equal(page1, rootPage.Navigation.NavigationStack[0]);
            Assert.Equal(6, recorder.Records.Count);

            //page 4
            var record = recorder.TakeFirst();
            Assert.Equal(page4, record.Sender);
            Assert.Equal(PageNavigationEvent.Destroy, record.Event);

            record = recorder.TakeFirst();
            Assert.Equal(page4ViewModel, record.Sender);
            Assert.Equal(PageNavigationEvent.Destroy, record.Event);

            //page 3
            record = recorder.TakeFirst();
            Assert.Equal(page3, record.Sender);
            Assert.Equal(PageNavigationEvent.Destroy, record.Event);

            record = recorder.TakeFirst();
            Assert.Equal(page3ViewModel, record.Sender);
            Assert.Equal(PageNavigationEvent.Destroy, record.Event);

            //page 2
            record = recorder.TakeFirst();
            Assert.Equal(page2, record.Sender);
            Assert.Equal(PageNavigationEvent.Destroy, record.Event);

            record = recorder.TakeFirst();
            Assert.Equal(page2ViewModel, record.Sender);
            Assert.Equal(PageNavigationEvent.Destroy, record.Event);
        }
    }
}
