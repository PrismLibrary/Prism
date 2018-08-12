using System.Collections.Generic;
using Prism.Common;
using Prism.Forms.Tests.Mocks;
using Prism.Forms.Tests.Mocks.ViewModels;
using Prism.Forms.Tests.Mocks.Views;
using Xamarin.Forms;
using Xunit;
using Xunit.Abstractions;

namespace Prism.Forms.Tests.Common
{
    public class PageUtilitiesFixture
    {
        private readonly ITestOutputHelper _output;

        public PageUtilitiesFixture(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void DestroyContentPage()
        {
            var recorder = new PageNavigationEventRecorder();
            var page = new ContentPageMock(recorder);

            if (page.BindingContext != null)
                _output.WriteLine(page.BindingContext.ToString());

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
            var masterPage = new ContentPageMock(recorder) { Title = "Master" };
            var masterPageViewModel = masterPage.BindingContext;
            var detailPage = new ContentPageMock(recorder) { Title = "Detail" };
            var detailPageViewModel = detailPage.BindingContext;
            var masterDetailPage = new MasterDetailPageMock(recorder, masterPage, detailPage);
            var masterDetailPageViewModel = masterDetailPage.BindingContext;

            PageUtilities.DestroyPage(masterDetailPage);

            // MasterPage
            var record = recorder.TakeFirst();
            Assert.Equal(masterPage, record.Sender);
            Assert.Null(masterPage.BindingContext);
            Assert.Equal(0, masterPage.Behaviors.Count);
            Assert.Equal(PageNavigationEvent.Destroy, record.Event);

            record = recorder.TakeFirst();
            Assert.Equal(masterPageViewModel, record.Sender);
            Assert.Equal(PageNavigationEvent.Destroy, record.Event);

            // DetailPage
            record = recorder.TakeFirst();
            Assert.Equal(detailPage, record.Sender);
            Assert.Null(detailPage.BindingContext);
            Assert.Equal(0, detailPage.Behaviors.Count);
            Assert.Equal(PageNavigationEvent.Destroy, record.Event);

            record = recorder.TakeFirst();
            Assert.Equal(detailPageViewModel, record.Sender);
            Assert.Equal(PageNavigationEvent.Destroy, record.Event);

            // MasterDetailPage
            record = recorder.TakeFirst();
            Assert.Equal(masterDetailPage, record.Sender);
            Assert.Null(masterDetailPage.BindingContext);
            Assert.Equal(0, masterDetailPage.Behaviors.Count);
            Assert.Equal(PageNavigationEvent.Destroy, record.Event);

            record = recorder.TakeFirst();
            Assert.Equal(masterDetailPageViewModel, record.Sender);
            Assert.Equal(PageNavigationEvent.Destroy, record.Event);

            Assert.True(recorder.IsEmpty);
        }

        [Fact]
        public void DestroyTabbedPage()
        {
            var recorder = new PageNavigationEventRecorder();
            var tabbedPage = new TabbedPageMock(recorder);
            var tabbedPageViewModel = tabbedPage.BindingContext;
            var tab1 = tabbedPage.Children[0];
            var tab1ViewModel = tab1.BindingContext;
            var tab2 = tabbedPage.Children[1];
            var tab2ViewModel = tab2.BindingContext;
            var tab3 = tabbedPage.Children[2];
            var tab3ViewModel = tab3.BindingContext;
            var tab4 = tabbedPage.Children[3];
            var tab4Child = ((NavigationPage)tab4).CurrentPage;
            var tab4ViewModel = tab4.BindingContext;
            var tab4ChildViewModel = tab4Child.BindingContext;
            var tab5 = tabbedPage.Children[4];
            var tab5Child = ((NavigationPage)tab5).CurrentPage;
            var tab5ViewModel = tab5.BindingContext;
            var tab5ChildViewModel = tab5Child.BindingContext;


            PageUtilities.DestroyPage(tabbedPage);

            Assert.Equal(13, recorder.Records.Count);

            //tab 5
            Assert.Equal(tab5ViewModel, recorder.Records[0].Sender);
            Assert.Equal(PageNavigationEvent.Destroy, recorder.Records[0].Event);

            Assert.Equal(tab5, recorder.Records[1].Sender);
            Assert.Equal(0, tab5.Behaviors.Count);
            Assert.Equal(PageNavigationEvent.Destroy, recorder.Records[1].Event);
            
            Assert.Equal(tab5ChildViewModel, recorder.Records[2].Sender);
            Assert.Equal(PageNavigationEvent.Destroy, recorder.Records[2].Event);

            //tab 4
            Assert.Equal(tab4Child, recorder.Records[3].Sender);
            Assert.Equal(0, tab4Child.Behaviors.Count);
            Assert.Equal(PageNavigationEvent.Destroy, recorder.Records[3].Event);

            Assert.Equal(tab4ChildViewModel, recorder.Records[4].Sender);
            Assert.Equal(PageNavigationEvent.Destroy, recorder.Records[4].Event);

            Assert.Equal(tab4, recorder.Records[5].Sender);
            Assert.Equal(0, tab4.Behaviors.Count);
            Assert.Equal(PageNavigationEvent.Destroy, recorder.Records[5].Event);

            Assert.Equal(tab4ViewModel, recorder.Records[6].Sender);
            Assert.Equal(PageNavigationEvent.Destroy, recorder.Records[6].Event);

            //tab 3
            Assert.Equal(tab3, recorder.Records[7].Sender);
            Assert.Null(tab3.BindingContext);
            Assert.Equal(0, tab3.Behaviors.Count);
            Assert.Equal(PageNavigationEvent.Destroy, recorder.Records[7].Event);

            Assert.Equal(tab3ViewModel, recorder.Records[8].Sender);
            Assert.Equal(PageNavigationEvent.Destroy, recorder.Records[8].Event);

            //tab 2 : PageMock has no binding context so it has no entries.

            //tab 1
            Assert.Equal(tab1, recorder.Records[9].Sender);
            Assert.Null(tab1.BindingContext);
            Assert.Equal(0, tab1.Behaviors.Count);
            Assert.Equal(PageNavigationEvent.Destroy, recorder.Records[9].Event);

            Assert.Equal(tab1ViewModel, recorder.Records[10].Sender);
            Assert.Equal(PageNavigationEvent.Destroy, recorder.Records[10].Event);

            //TabbedPage
            Assert.Equal(tabbedPage, recorder.Records[11].Sender);
            Assert.Null(tabbedPage.BindingContext);
            Assert.Equal(0, tabbedPage.Behaviors.Count);
            Assert.Equal(PageNavigationEvent.Destroy, recorder.Records[11].Event);

            Assert.Equal(tabbedPageViewModel, recorder.Records[12].Sender);
            Assert.Equal(PageNavigationEvent.Destroy, recorder.Records[12].Event);
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
            var modalStack = new List<Page> { contentPage2, contentPage3 };

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
