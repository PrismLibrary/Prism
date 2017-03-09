using System.Collections.Generic;
using Prism.Common;
using Prism.Forms.Tests.Mocks;
using Prism.Forms.Tests.Mocks.ViewModels;
using Prism.Forms.Tests.Mocks.Views;
using Xamarin.Forms;
using Xunit;

namespace Prism.Forms.Tests.Common
{
    public class PageUtilitiesFixture
    {
        [Fact]
        public void DestroyContentPage()
        {
            var recorder = new PageNavigationEventRecorder();
            var page = new ContentPageMock(recorder);
            var viewModel = (ViewModelBase)page.BindingContext;

            PageUtilities.DestroyPage(page);

            Assert.Equal(2, recorder.Records.Count);

            Assert.Equal(page, recorder.Records[0].Sender);
            Assert.Null(page.BindingContext);
            Assert.Equal(0, page.Behaviors.Count);
            Assert.Equal(PageNavigationEvent.Destroy, recorder.Records[0].Event);

            Assert.Equal(viewModel, recorder.Records[1].Sender);
            Assert.Equal(PageNavigationEvent.Destroy, recorder.Records[1].Event);
        }

        [Fact]
        public void DestroyNavigationPage()
        {
            var recorder = new PageNavigationEventRecorder();
            var navigationPage = new NavigationPageMock(recorder);
            var navigationPageViewModel = navigationPage.BindingContext;
            var contentPage1 = navigationPage.CurrentPage;
            var contentPage1ViewModel = contentPage1.BindingContext;
            var contentPage2 = new ContentPageMock(recorder);
            var contentPage2ViewModel = contentPage2.BindingContext;
            contentPage1.Navigation.PushAsync(contentPage2);

            PageUtilities.DestroyPage(navigationPage);

            Assert.Equal(6, recorder.Records.Count);

            // contentPage2
            Assert.Equal(contentPage2, recorder.Records[0].Sender);
            Assert.Null(contentPage2.BindingContext);
            Assert.Equal(0, contentPage2.Behaviors.Count);
            Assert.Equal(PageNavigationEvent.Destroy, recorder.Records[0].Event);

            Assert.Equal(contentPage2ViewModel, recorder.Records[1].Sender);
            Assert.Equal(PageNavigationEvent.Destroy, recorder.Records[1].Event);

            // contentPage1
            Assert.Equal(contentPage1, recorder.Records[2].Sender);
            Assert.Null(contentPage1.BindingContext);
            Assert.Equal(0, contentPage1.Behaviors.Count);
            Assert.Equal(PageNavigationEvent.Destroy, recorder.Records[2].Event);

            Assert.Equal(contentPage1ViewModel, recorder.Records[3].Sender);
            Assert.Equal(PageNavigationEvent.Destroy, recorder.Records[3].Event);

            // navigationPage
            Assert.Equal(navigationPage, recorder.Records[4].Sender);
            Assert.Null(navigationPage.BindingContext);
            Assert.Equal(0, navigationPage.Behaviors.Count);
            Assert.Equal(PageNavigationEvent.Destroy, recorder.Records[4].Event);

            Assert.Equal(navigationPageViewModel, recorder.Records[5].Sender);
            Assert.Equal(PageNavigationEvent.Destroy, recorder.Records[5].Event);
        }

        [Fact]
        public void DestoryMasterDetailPage()
        {
            var recorder = new PageNavigationEventRecorder();
            var masterDetailPage = new MasterDetailPageMock(recorder);
            var masterDetailPageViewModel = masterDetailPage.BindingContext;
            var detailPage = masterDetailPage.Detail;
            var detailPageViewModel = detailPage.BindingContext;

            PageUtilities.DestroyPage(masterDetailPage);

            // masterDetailPage
            Assert.Equal(detailPage, recorder.Records[0].Sender);
            Assert.Null(detailPage.BindingContext);
            Assert.Equal(0, detailPage.Behaviors.Count);
            Assert.Equal(PageNavigationEvent.Destroy, recorder.Records[0].Event);

            Assert.Equal(detailPageViewModel, recorder.Records[1].Sender);
            Assert.Equal(PageNavigationEvent.Destroy, recorder.Records[1].Event);

            // detailPage
            Assert.Equal(masterDetailPage, recorder.Records[2].Sender);
            Assert.Null(masterDetailPage.BindingContext);
            Assert.Equal(0, masterDetailPage.Behaviors.Count);
            Assert.Equal(PageNavigationEvent.Destroy, recorder.Records[2].Event);

            Assert.Equal(masterDetailPageViewModel, recorder.Records[3].Sender);
            Assert.Equal(PageNavigationEvent.Destroy, recorder.Records[3].Event);

            Assert.Equal(4, recorder.Records.Count);
        }

        [Fact]
        public void DestroyTabbedPage()
        {
            var recorder = new PageNavigationEventRecorder();
            var tabbedPage = new TabbedPageMock(recorder);
            var tabbedPageViewModel = tabbedPage.BindingContext;
            var childPage1 = tabbedPage.Children[0];
            var childPage1ViewModel = childPage1.BindingContext;
            var childPage2 = tabbedPage.Children[1];
            var childPage2ViewModel = childPage2.BindingContext;
            var childPage3 = tabbedPage.Children[2];
            var childPage3ViewModel = childPage3.BindingContext;

            PageUtilities.DestroyPage(tabbedPage);

            Assert.Equal(7, recorder.Records.Count);

            // childPage3
            Assert.Equal(childPage3, recorder.Records[0].Sender);
            Assert.Null(childPage3.BindingContext);
            Assert.Equal(0, childPage3.Behaviors.Count);
            Assert.Equal(PageNavigationEvent.Destroy, recorder.Records[0].Event);

            Assert.Equal(childPage3ViewModel, recorder.Records[1].Sender);
            Assert.Equal(PageNavigationEvent.Destroy, recorder.Records[1].Event);

            // childPage2 : This page is PageMock.
            Assert.Equal(childPage2ViewModel, recorder.Records[2].Sender);
            Assert.Equal(PageNavigationEvent.Destroy, recorder.Records[2].Event);

            // childPage1
            Assert.Equal(childPage1, recorder.Records[3].Sender);
            Assert.Null(childPage1.BindingContext);
            Assert.Equal(0, childPage1.Behaviors.Count);
            Assert.Equal(PageNavigationEvent.Destroy, recorder.Records[3].Event);

            Assert.Equal(childPage1ViewModel, recorder.Records[4].Sender);
            Assert.Equal(PageNavigationEvent.Destroy, recorder.Records[4].Event);

            // tabbedPage
            Assert.Equal(tabbedPage, recorder.Records[5].Sender);
            Assert.Null(tabbedPage.BindingContext);
            Assert.Equal(0, tabbedPage.Behaviors.Count);
            Assert.Equal(PageNavigationEvent.Destroy, recorder.Records[5].Event);

            Assert.Equal(tabbedPageViewModel, recorder.Records[6].Sender);
            Assert.Equal(PageNavigationEvent.Destroy, recorder.Records[6].Event);
        }

        [Fact]
        public void DestroyCarouselPage()
        {
            var recorder = new PageNavigationEventRecorder();
            var carouselPage = new CarouselPageMock(recorder);
            var carouselPageViewModel = carouselPage.BindingContext;
            var childPage2 = carouselPage.Children[1];
            var childPage2ViewModel = childPage2.BindingContext;

            PageUtilities.DestroyPage(carouselPage);

            Assert.Equal(6, recorder.Records.Count);

            // childPage3 : This page is ContentPage.
            Assert.Equal(carouselPageViewModel, recorder.Records[0].Sender);
            Assert.Equal(PageNavigationEvent.Destroy, recorder.Records[0].Event);

            // childPage2
            Assert.Equal(childPage2, recorder.Records[1].Sender);
            Assert.Null(childPage2.BindingContext);
            Assert.Equal(0, childPage2.Behaviors.Count);
            Assert.Equal(PageNavigationEvent.Destroy, recorder.Records[1].Event);

            Assert.Equal(childPage2ViewModel, recorder.Records[2].Sender);
            Assert.Equal(PageNavigationEvent.Destroy, recorder.Records[2].Event);

            // childPage1 : This page is ContentPage.
            Assert.Equal(carouselPageViewModel, recorder.Records[3].Sender);
            Assert.Equal(PageNavigationEvent.Destroy, recorder.Records[3].Event);

            // tabbedPage
            Assert.Equal(carouselPage, recorder.Records[4].Sender);
            Assert.Null(carouselPage.BindingContext);
            Assert.Equal(0, carouselPage.Behaviors.Count);
            Assert.Equal(PageNavigationEvent.Destroy, recorder.Records[4].Event);

            Assert.Equal(carouselPageViewModel, recorder.Records[5].Sender);
            Assert.Equal(PageNavigationEvent.Destroy, recorder.Records[5].Event);
        }

        [Fact]
        public void DestroyWithModalStack()
        {
            var recorder = new PageNavigationEventRecorder();
            var contentPage1 = new ContentPageMock(recorder);
            var viewModel1 = contentPage1.BindingContext;
            var contentPage2 = new ContentPageMock(recorder);
            var viewModel2 = contentPage2.BindingContext;
            var contentPage3 = new ContentPageMock(recorder);
            var viewModel3 = contentPage3.BindingContext;
            var modalStack = new List<Page> {contentPage2, contentPage3};

            PageUtilities.DestroyWithModalStack(contentPage1, modalStack);

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
