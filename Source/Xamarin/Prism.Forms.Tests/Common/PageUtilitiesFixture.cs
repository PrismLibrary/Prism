using Prism.Common;
using Prism.Forms.Tests.Mocks;
using Prism.Forms.Tests.Mocks.Views;
using Xunit;

namespace Prism.Forms.Tests.Common
{
    public class PageUtilitiesFixture
    {
        [Fact]
        public void DestroyContentPage()
        {
            using (var recoder = PageNavigationEventRecoder.BeginRecord())
            {
                var page = new ContentPageMock();
                var viewModel = page.BindingContext;

                PageUtilities.DestroyPage(page);

                Assert.Equal(2, recoder.Records.Count);

                Assert.Equal(page, recoder.Records[0].Sender);
                Assert.Null(page.BindingContext);
                Assert.Equal(0, page.Behaviors.Count);
                Assert.Equal(PageNavigationEvent.Destroy, recoder.Records[0].Event);

                Assert.Equal(viewModel, recoder.Records[1].Sender);
                Assert.Equal(PageNavigationEvent.Destroy, recoder.Records[1].Event);
            }
        }

        [Fact]
        public void DestroyNavigationPage()
        {
            using (var recoder = PageNavigationEventRecoder.BeginRecord())
            {
                var navigationPage = new NavigationPageMock();
                var navigationPageViewModel = navigationPage.BindingContext;
                var contentPage1 = navigationPage.CurrentPage;
                var contentPage1ViewModel = contentPage1.BindingContext;
                var contentPage2 = new ContentPageMock();
                var contentPage2ViewModel = contentPage2.BindingContext;
                contentPage1.Navigation.PushAsync(contentPage2);

                PageUtilities.DestroyPage(navigationPage);

                Assert.Equal(6, recoder.Records.Count);

                // contentPage2
                Assert.Equal(contentPage2, recoder.Records[0].Sender);
                Assert.Null(contentPage2.BindingContext);
                Assert.Equal(0, contentPage2.Behaviors.Count);
                Assert.Equal(PageNavigationEvent.Destroy, recoder.Records[0].Event);

                Assert.Equal(contentPage2ViewModel, recoder.Records[1].Sender);
                Assert.Equal(PageNavigationEvent.Destroy, recoder.Records[1].Event);

                // contentPage1
                Assert.Equal(contentPage1, recoder.Records[2].Sender);
                Assert.Null(contentPage1.BindingContext);
                Assert.Equal(0, contentPage1.Behaviors.Count);
                Assert.Equal(PageNavigationEvent.Destroy, recoder.Records[2].Event);

                Assert.Equal(contentPage1ViewModel, recoder.Records[3].Sender);
                Assert.Equal(PageNavigationEvent.Destroy, recoder.Records[3].Event);

                // navigationPage
                Assert.Equal(navigationPage, recoder.Records[4].Sender);
                Assert.Null(navigationPage.BindingContext);
                Assert.Equal(0, navigationPage.Behaviors.Count);
                Assert.Equal(PageNavigationEvent.Destroy, recoder.Records[4].Event);

                Assert.Equal(navigationPageViewModel, recoder.Records[5].Sender);
                Assert.Equal(PageNavigationEvent.Destroy, recoder.Records[5].Event);
            }
        }

        [Fact]
        public void DestoryMasterDetailPage()
        {
            using (var recoder = PageNavigationEventRecoder.BeginRecord())
            {
                var masterDetailPage = new MasterDetailPageMock();
                var masterDetailPageViewModel = masterDetailPage.BindingContext;
                var detailPage = masterDetailPage.Detail;
                var detailPageViewModel = detailPage.BindingContext;

                PageUtilities.DestroyPage(masterDetailPage);

                // masterDetailPage
                Assert.Equal(detailPage, recoder.Records[0].Sender);
                Assert.Null(detailPage.BindingContext);
                Assert.Equal(0, detailPage.Behaviors.Count);
                Assert.Equal(PageNavigationEvent.Destroy, recoder.Records[0].Event);

                Assert.Equal(detailPageViewModel, recoder.Records[1].Sender);
                Assert.Equal(PageNavigationEvent.Destroy, recoder.Records[1].Event);

                // detailPage
                Assert.Equal(masterDetailPage, recoder.Records[2].Sender);
                Assert.Null(masterDetailPage.BindingContext);
                Assert.Equal(0, masterDetailPage.Behaviors.Count);
                Assert.Equal(PageNavigationEvent.Destroy, recoder.Records[2].Event);

                Assert.Equal(masterDetailPageViewModel, recoder.Records[3].Sender);
                Assert.Equal(PageNavigationEvent.Destroy, recoder.Records[3].Event);

                Assert.Equal(4, recoder.Records.Count);
            }
        }

        [Fact]
        public void DestroyTabbedPage()
        {
            using (var recoder = PageNavigationEventRecoder.BeginRecord())
            {
                var tabbedPage = new TabbedPageMock();
                var tabbedPageViewModel = tabbedPage.BindingContext;
                var childPage1 = tabbedPage.Children[0];
                var childPage1ViewModel = childPage1.BindingContext;
                var childPage2 = tabbedPage.Children[1];
                var childPage2ViewModel = childPage2.BindingContext;
                var childPage3 = tabbedPage.Children[2];
                var childPage3ViewModel = childPage3.BindingContext;

                PageUtilities.DestroyPage(tabbedPage);

                Assert.Equal(7, recoder.Records.Count);

                // childPage3
                Assert.Equal(childPage3, recoder.Records[0].Sender);
                Assert.Null(childPage3.BindingContext);
                Assert.Equal(0, childPage3.Behaviors.Count);
                Assert.Equal(PageNavigationEvent.Destroy, recoder.Records[0].Event);

                Assert.Equal(childPage3ViewModel, recoder.Records[1].Sender);
                Assert.Equal(PageNavigationEvent.Destroy, recoder.Records[1].Event);

                // childPage2 : This page is PageMock.
                Assert.Equal(childPage2ViewModel, recoder.Records[2].Sender);
                Assert.Equal(PageNavigationEvent.Destroy, recoder.Records[2].Event);

                // childPage1
                Assert.Equal(childPage1, recoder.Records[3].Sender);
                Assert.Null(childPage1.BindingContext);
                Assert.Equal(0, childPage1.Behaviors.Count);
                Assert.Equal(PageNavigationEvent.Destroy, recoder.Records[3].Event);

                Assert.Equal(childPage1ViewModel, recoder.Records[4].Sender);
                Assert.Equal(PageNavigationEvent.Destroy, recoder.Records[4].Event);

                // tabbedPage
                Assert.Equal(tabbedPage, recoder.Records[5].Sender);
                Assert.Null(tabbedPage.BindingContext);
                Assert.Equal(0, tabbedPage.Behaviors.Count);
                Assert.Equal(PageNavigationEvent.Destroy, recoder.Records[5].Event);

                Assert.Equal(tabbedPageViewModel, recoder.Records[6].Sender);
                Assert.Equal(PageNavigationEvent.Destroy, recoder.Records[6].Event);

            }
        }

        [Fact]
        public void DestroyCarouselPage()
        {
            using (var recoder = PageNavigationEventRecoder.BeginRecord())
            {
                var carouselPage = new CarouselPageMock();
                var carouselPageViewModel = carouselPage.BindingContext;
                var childPage1 = carouselPage.Children[0];
                var childPage1ViewModel = childPage1.BindingContext;
                var childPage2 = carouselPage.Children[1];
                var childPage2ViewModel = childPage2.BindingContext;
                var childPage3 = carouselPage.Children[2];
                var childPage3ViewModel = childPage3.BindingContext;

                PageUtilities.DestroyPage(carouselPage);

                Assert.Equal(6, recoder.Records.Count);

                // childPage3 : This page is ContentPage.
                Assert.Equal(carouselPageViewModel, recoder.Records[0].Sender);
                Assert.Equal(PageNavigationEvent.Destroy, recoder.Records[0].Event);

                // childPage2
                Assert.Equal(childPage2, recoder.Records[1].Sender);
                Assert.Null(childPage2.BindingContext);
                Assert.Equal(0, childPage1.Behaviors.Count);
                Assert.Equal(PageNavigationEvent.Destroy, recoder.Records[1].Event);

                Assert.Equal(childPage2ViewModel, recoder.Records[2].Sender);
                Assert.Equal(PageNavigationEvent.Destroy, recoder.Records[2].Event);

                // childPage1 : This page is ContentPage.
                Assert.Equal(carouselPageViewModel, recoder.Records[3].Sender);
                Assert.Equal(PageNavigationEvent.Destroy, recoder.Records[3].Event);

                // tabbedPage
                Assert.Equal(carouselPage, recoder.Records[4].Sender);
                Assert.Null(carouselPage.BindingContext);
                Assert.Equal(0, carouselPage.Behaviors.Count);
                Assert.Equal(PageNavigationEvent.Destroy, recoder.Records[4].Event);

                Assert.Equal(carouselPageViewModel, recoder.Records[5].Sender);
                Assert.Equal(PageNavigationEvent.Destroy, recoder.Records[5].Event);

            }

        }

        [Fact]
        public void DestroyWithModalStack()
        {
            var contentPage1 = new ContentPageMock();
            var viewModel1 = contentPage1.BindingContext;
            var contentPage2 = new ContentPageMock();
            var viewModel2 = contentPage2.BindingContext;
            var contentPage3 = new ContentPageMock();
            var viewModel3 = contentPage3.BindingContext;
            contentPage1.Navigation.PushModalAsync(contentPage2);
            contentPage1.Navigation.PushModalAsync(contentPage3);

            using (var recorder = PageNavigationEventRecoder.BeginRecord())
            {
                PageUtilities.DestroyWithModalStack(contentPage1);

                var record = recorder.TakeFirst();
                Assert.Equal(contentPage3, record.Sender);
                Assert.Equal(PageNavigationEvent.Destroy, record.Event);

                record = recorder.TakeFirst();
                Assert.Equal(viewModel3, record.Sender);
                Assert.Equal(PageNavigationEvent.Destroy, record.Event);

                record = recorder.TakeFirst();
                Assert.Equal(contentPage2, record.Sender);
                Assert.Equal(PageNavigationEvent.Destroy, record.Event);

                record = recorder.TakeFirst();
                Assert.Equal(viewModel2, record.Sender);
                Assert.Equal(PageNavigationEvent.Destroy, record.Event);

                record = recorder.TakeFirst();
                Assert.Equal(contentPage1, record.Sender);
                Assert.Equal(PageNavigationEvent.Destroy, record.Event);

                record = recorder.TakeFirst();
                Assert.Equal(viewModel1, record.Sender);
                Assert.Equal(PageNavigationEvent.Destroy, record.Event);

                Assert.True(recorder.IsEmpty);
            }
        }
    }
}
