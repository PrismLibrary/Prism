using Prism.Common;
using Prism.DI.Forms.Tests.Mocks.ViewModels;
using Prism.DI.Forms.Tests.Mocks.Views;
using Xamarin.Forms;
using Xunit;
using Xunit.Abstractions;

namespace Prism.DI.Forms.Tests.Fixtures.Navigation
{
    public class NavigationServiceFixture : FixtureBase
    {
        public NavigationServiceFixture(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }

        [Fact]
        public void ContentPage_GetsCorrectNavService()
        {
            var app = CreateMockApplication();
            app.NavigationService.NavigateAsync("XamlViewMockA");

            var mainPage = app.MainPage;
            var vm = mainPage.BindingContext as XamlViewMockAViewModel;

            Assert.IsType<XamlViewMockA>(mainPage);
            Assert.NotNull(vm);

            var page = ((IPageAware)vm.NavigationService).Page;
            Assert.IsType<XamlViewMockA>(mainPage);
            Assert.Same(mainPage, page);
        }

        [Fact]
        public void ContentPage_InNavigationPage_GetsCorrectNavService()
        {
            var app = CreateMockApplication();
            app.NavigationService.NavigateAsync("NavigationPage/XamlViewMockA");

            var mainPage = app.MainPage;
            Assert.IsType<NavigationPage>(mainPage);

            var view = mainPage.Navigation.NavigationStack[0] as XamlViewMockA;
            Assert.NotNull(view);

            var vm = view.BindingContext as XamlViewMockAViewModel;
            Assert.NotNull(vm);

            var page = ((IPageAware)vm.NavigationService).Page;
            Assert.IsType<XamlViewMockA>(page);
            Assert.Same(view, page);
        }

        [Fact]
        public void TabbedPage_GetsCorrectNavService()
        {
            var app = CreateMockApplication();
            app.NavigationService.NavigateAsync("XamlTabbedViewMock");

            var tp = app.MainPage as TabbedPage;
            Assert.NotNull(tp);

            var tpVm = tp.BindingContext as XamlTabbedViewMockViewModel;
            Assert.NotNull(tpVm);

            var page = ((IPageAware)tpVm.NavigationService).Page;
            Assert.IsType<XamlTabbedViewMock>(page);
            Assert.Same(tp, page);
        }

        [Fact]
        public void TabbedPage_ContentPage_NestedNavigationPage_GetsCorrectNavService()
        {
            var app = CreateMockApplication();
            app.NavigationService.NavigateAsync("XamlTabbedViewMock");

            var tp = app.MainPage as TabbedPage;
            Assert.NotNull(tp);

            var tab = tp.Children[0];
            Assert.NotNull(tab);

            var navPage = tab as NavigationPage;
            Assert.NotNull(navPage);

            var view = navPage.CurrentPage;
            Assert.NotNull(view);

            var vm = view.BindingContext as XamlViewMockAViewModel;
            Assert.NotNull(vm);

            var page = ((IPageAware)vm.NavigationService).Page;
            Assert.IsType<XamlViewMockA>(page);
            Assert.Same(view, page);
        }

        [Fact]
        public void TabbedPage_ContentPage_GetsCorrectNavService()
        {
            var app = CreateMockApplication();
            app.NavigationService.NavigateAsync("XamlTabbedViewMock");

            var tp = app.MainPage as TabbedPage;
            Assert.NotNull(tp);

            var view = tp.Children[1];
            Assert.NotNull(view);

            var vm = view.BindingContext as XamlViewMockAViewModel;
            Assert.NotNull(vm);

            var page = ((IPageAware)vm.NavigationService).Page;
            Assert.IsType<XamlViewMockA>(page);
            Assert.Same(view, page);
        }

        [Fact]
        public void TabbedPage_InNavigationPage_GetsCorrectNavService()
        {
            var app = CreateMockApplication();
            app.NavigationService.NavigateAsync("NavigationPage/XamlTabbedViewMock");

            var mainPage = app.MainPage;
            Assert.IsType<NavigationPage>(mainPage);

            var tp = mainPage.Navigation.NavigationStack[0] as TabbedPage;
            Assert.NotNull(tp);

            var tpVm = tp.BindingContext as XamlTabbedViewMockViewModel;
            Assert.NotNull(tpVm);

            var page = ((IPageAware)tpVm.NavigationService).Page;
            Assert.IsType<XamlTabbedViewMock>(page);
            Assert.Same(tp, page);
        }

        [Fact]
        public void MasterDetail_GetsCorrectNavService()
        {
            var app = CreateMockApplication();
            app.NavigationService.NavigateAsync("XamlMasterDetailViewMock");

            var mainPage = app.MainPage;
            Assert.IsType<XamlMasterDetailViewMock>(mainPage);

            var vm = mainPage.BindingContext as XamlMasterDetailViewMockViewModel;
            Assert.NotNull(vm);

            var page = ((IPageAware)vm.NavigationService).Page;
            Assert.IsType<XamlMasterDetailViewMock>(page);
            Assert.Same(mainPage, page);
        }

        [Fact]
        public void MasterDetail_Detail_GetsCorrectNavService()
        {
            var app = CreateMockApplication();
            app.NavigationService.NavigateAsync("XamlMasterDetailViewMock");

            var mainPage = app.MainPage as XamlMasterDetailViewMock;
            Assert.NotNull(mainPage);

            var detail = mainPage.Detail as NavigationPage;
            Assert.NotNull(detail);

            var view = detail.CurrentPage as XamlViewMockA;
            Assert.NotNull(view);

            var vm = view.BindingContext as XamlViewMockAViewModel;
            Assert.NotNull(vm);

            var page = ((IPageAware)vm.NavigationService).Page;
            Assert.IsType<XamlViewMockA>(page);
            Assert.Same(view, page);
        }
    }
}
