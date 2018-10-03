using Prism.Common;
using Prism.Forms.Tests.Mocks;
using Prism.Forms.Tests.Mocks.Views;
using Prism.Forms.Tests.Navigation.Mocks.Views;
using Prism.Navigation;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xunit;

namespace Prism.Forms.Tests.Navigation
{
    [Collection("PageNavigationRegistry")]
    public class INavigationServiceExtensionsFixture
    {
        public INavigationServiceExtensionsFixture()
        {
            PageNavigationRegistry.ClearRegistrationCache();

            PageNavigationRegistry.Register("NavigationPage", typeof(NavigationPage));
            PageNavigationRegistry.Register("Page1", typeof(NavigationPathPageMock));
            PageNavigationRegistry.Register("Page2", typeof(NavigationPathPageMock2));
            PageNavigationRegistry.Register("Page3", typeof(NavigationPathPageMock3));
            PageNavigationRegistry.Register("Page4", typeof(NavigationPathPageMock4));
            PageNavigationRegistry.Register("TabbedPage1", typeof(NavigationPathTabbedPageMock));
            PageNavigationRegistry.Register("MasterDetailPage", typeof(MasterDetailPage));
        }

        [Fact]
        public async Task GoBackToRootAsync_PopsToRoot()
        {
            var navigationService = new PageNavigationServiceMock(null, null, null);
            var rootPage = new NavigationPage();
            ((IPageAware)navigationService).Page = rootPage;

            var page1 = new ContentPageMock() { Title = "Page 1" };
            await rootPage.Navigation.PushAsync(page1);

            await rootPage.Navigation.PushAsync(new ContentPageMock() { Title = "Page 2" });
            await rootPage.Navigation.PushAsync(new ContentPageMock() { Title = "Page 3" });
            await rootPage.Navigation.PushAsync(new ContentPageMock() { Title = "Page 4" });

            Assert.True(rootPage.Navigation.NavigationStack.Count == 4);

            await navigationService.GoBackToRootAsync();

            Assert.Equal(1, rootPage.Navigation.NavigationStack.Count);
            Assert.Equal(page1, rootPage.Navigation.NavigationStack[0]);
        }

        [Fact]
        public async Task GoBackToRootAsync_PopsToRoot_INavigationAware_Destroy()
        {
            var recorder = new PageNavigationEventRecorder();
            var navigationService = new PageNavigationServiceMock(null, null, null);
            var rootPage = new NavigationPage();
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

            await navigationService.GoBackToRootAsync();

            Assert.Equal(1, rootPage.Navigation.NavigationStack.Count);
            Assert.Equal(page1, rootPage.Navigation.NavigationStack[0]);
            Assert.Equal(16, recorder.Records.Count);

            //root
            var record = recorder.TakeFirst();
            Assert.Equal(page1, record.Sender);
            Assert.Equal(PageNavigationEvent.OnNavigatingTo, record.Event);

            record = recorder.TakeFirst();
            Assert.Equal(page1.BindingContext, record.Sender);
            Assert.Equal(PageNavigationEvent.OnNavigatingTo, record.Event);

            //page 4
            record = recorder.TakeFirst();
            Assert.Equal(page4, record.Sender);
            Assert.Equal(PageNavigationEvent.OnNavigatedFrom, record.Event);

            record = recorder.TakeFirst();
            Assert.Equal(page4ViewModel, record.Sender);
            Assert.Equal(PageNavigationEvent.OnNavigatedFrom, record.Event);

            record = recorder.TakeFirst();
            Assert.Equal(page4, record.Sender);
            Assert.Equal(PageNavigationEvent.Destroy, record.Event);

            record = recorder.TakeFirst();
            Assert.Equal(page4ViewModel, record.Sender);
            Assert.Equal(PageNavigationEvent.Destroy, record.Event);

            //page 3
            record = recorder.TakeFirst();
            Assert.Equal(page3, record.Sender);
            Assert.Equal(PageNavigationEvent.OnNavigatedFrom, record.Event);

            record = recorder.TakeFirst();
            Assert.Equal(page3ViewModel, record.Sender);
            Assert.Equal(PageNavigationEvent.OnNavigatedFrom, record.Event);

            record = recorder.TakeFirst();
            Assert.Equal(page3, record.Sender);
            Assert.Equal(PageNavigationEvent.Destroy, record.Event);

            record = recorder.TakeFirst();
            Assert.Equal(page3ViewModel, record.Sender);
            Assert.Equal(PageNavigationEvent.Destroy, record.Event);

            //page 2
            record = recorder.TakeFirst();
            Assert.Equal(page2, record.Sender);
            Assert.Equal(PageNavigationEvent.OnNavigatedFrom, record.Event);

            record = recorder.TakeFirst();
            Assert.Equal(page2ViewModel, record.Sender);
            Assert.Equal(PageNavigationEvent.OnNavigatedFrom, record.Event);

            record = recorder.TakeFirst();
            Assert.Equal(page2, record.Sender);
            Assert.Equal(PageNavigationEvent.Destroy, record.Event);

            record = recorder.TakeFirst();
            Assert.Equal(page2ViewModel, record.Sender);
            Assert.Equal(PageNavigationEvent.Destroy, record.Event);

            //root
            record = recorder.TakeFirst();
            Assert.Equal(page1, record.Sender);
            Assert.Equal(PageNavigationEvent.OnNavigatedTo, record.Event);

            record = recorder.TakeFirst();
            Assert.Equal(page1.BindingContext, record.Sender);
            Assert.Equal(PageNavigationEvent.OnNavigatedTo, record.Event);
        }

        [Fact]
        public async Task GetNavigationUriPath()
        {
            var rootPage = new Xamarin.Forms.NavigationPage();

            var page1 = new NavigationPathPageMock() { Title = "Page1" };
            await rootPage.Navigation.PushAsync(page1);

            var page2 = new NavigationPathPageMock2() { Title = "Page2" };
            await rootPage.Navigation.PushAsync(page2);

            var page3 = new NavigationPathPageMock3() { Title = "Page3" };
            await rootPage.Navigation.PushAsync(page3);

            var page4 = new NavigationPathPageMock4() { Title = "Page4" };
            await rootPage.Navigation.PushAsync(page4);

            var path = page3.ViewModel.NavigationService.GetNavigationUriPath();

            Assert.Equal("/NavigationPage/Page1/Page2/Page3", path);
        }

        [Fact]
        public async Task GetNavigationUriPath2()
        {
            var rootPage = new NavigationPage();

            var page1 = new NavigationPathTabbedPageMock() { Title = "TabbedPage1" };
            await rootPage.Navigation.PushAsync(page1);

            var path = ((NavigationPathPageMock2)page1.Children[1]).ViewModel.NavigationService.GetNavigationUriPath();

            Assert.Equal("/NavigationPage/TabbedPage1?selectedTab=Page2", path);
        }

        [Fact]
        public void GetNavigationUriPath3()
        {
            var rootPage = new MasterDetailPage();
            rootPage.Master = new ContentPage() { Title = "Master" };

            var page1 = new NavigationPathPageMock() { Title = "Page1" };
            rootPage.Detail = page1;

            var path = page1.ViewModel.NavigationService.GetNavigationUriPath();

            Assert.Equal("/MasterDetailPage/Page1", path);
        }

        [Fact]
        public void GetNavigationUriPath4()
        {
            var rootPage = new MasterDetailPage();
            rootPage.Master = new ContentPage() { Title = "Master" };

            var page1 = new NavigationPathPageMock() { Title = "Page1" };
            rootPage.Detail = new NavigationPage(page1);

            var path = page1.ViewModel.NavigationService.GetNavigationUriPath();

            Assert.Equal("/MasterDetailPage/NavigationPage/Page1", path);
        }

        [Fact]
        public void GetNavigationUriPath5()
        {
            var rootPage = new MasterDetailPage();
            rootPage.Master = new ContentPage() { Title = "Master" };

            var tabbedpage = new NavigationPathTabbedPageMock() { Title = "Page1" };

            var detail = new NavigationPage(tabbedpage); ;
            rootPage.Detail = detail;

            var page1 = new NavigationPathPageMock() { Title = "Page1" };
            rootPage.Detail.Navigation.PushAsync(page1);

            var path = page1.ViewModel.NavigationService.GetNavigationUriPath();
            Assert.Equal("/MasterDetailPage/NavigationPage/TabbedPage1/Page1", path);
            
            path = tabbedpage.ViewModel.NavigationService.GetNavigationUriPath();
            Assert.Equal("/MasterDetailPage/NavigationPage/TabbedPage1", path);

            path = ((NavigationPathPageMock)tabbedpage.Children[0]).ViewModel.NavigationService.GetNavigationUriPath();
            Assert.Equal("/MasterDetailPage/NavigationPage/TabbedPage1?selectedTab=Page1", path);
        }
    }
}
