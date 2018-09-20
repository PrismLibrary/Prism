using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#if Autofac
using Autofac.Core.Registration;
#elif DryIoc
using DryIoc;
#elif Ninject
using Ninject;
#endif
using Prism.DI.Forms.Tests.Fixtures;
using Prism.DI.Forms.Tests.Mocks.ViewModels;
using Prism.DI.Forms.Tests.Mocks.Views;
using Prism.Mvvm;
using Xamarin.Forms;
using Xunit;
using Xunit.Abstractions;

#if Autofac
namespace Prism.Autofac.Forms.Tests.Fixtures
#elif DryIoc
namespace Prism.DryIoc.Forms.Tests.Fixtures
#elif Ninject
namespace Prism.Ninject.Forms.Tests.Fixtures
#elif Unity
namespace Prism.Unity.Forms.Tests.Fixtures
#endif
{
    public class PartialViewFixture : FixtureBase
    {
        public PartialViewFixture(ITestOutputHelper testOutputHelper) 
            : base(testOutputHelper)
        {
        }

        [Fact]
        public async Task PartialView_LocatesViewModel()
        {
            var app = CreateMockApplication();
            await app.NavigationService.NavigateAsync("/XamlViewMock?text=Test");
            Assert.NotNull(app.MainPage);
            Assert.IsType<XamlViewMock>(app.MainPage);
            var page = (XamlViewMock)app.MainPage;
            var layout = (StackLayout)page.Content;
            Assert.NotNull(layout);
            var partialView = (PartialView)layout.Children.FirstOrDefault(c => c is PartialView);
            Assert.NotNull(partialView);
            Assert.NotNull(partialView.BindingContext);
            Assert.True(ViewModelLocator.GetAutowireViewModel(partialView));
            Assert.IsType<PartialViewModel>(partialView.BindingContext);
        }

        [Fact]
        public async Task PartialView_AddedToPageRegistry()
        {
            var app = CreateMockApplication();
            await app.NavigationService.NavigateAsync("/XamlViewMock?text=Test");
            var page = (XamlViewMock)app.MainPage;
            var partialViews = (List<BindableObject>)page.GetValue(ViewModelLocator.PartialViewsProperty);

            Assert.Single(partialViews);
        }

        [Fact]
        public async Task PartialViewSupportsINavigationAwareInterfaces()
        {
            var app = CreateMockApplication();
            await app.NavigationService.NavigateAsync("/XamlViewMock?text=Test");
            var page = (XamlViewMock)app.MainPage;
            var layout = (StackLayout)page.Content;
            var partialView = (PartialView)layout.Children.FirstOrDefault(c => c is PartialView);
            var vm = (PartialViewModel)partialView.BindingContext;
            Assert.Equal(1, vm.OnNavigatingToCalled);
            Assert.Equal(1, vm.OnNavigatedToCalled);
            Assert.Equal(0, vm.OnNavigatedFromCalled);
            Assert.Equal("Test", vm.SomeText);

            var partialViews = (List<BindableObject>)page.GetValue(ViewModelLocator.PartialViewsProperty);
            Assert.Single(partialViews);

            partialView.Navigate();

            Assert.Equal(1, vm.OnNavigatedFromCalled);
        }

        [Fact]
        public async Task PartialViewModel_InjectsINavigationService()
        {
            var app = CreateMockApplication();
            await app.NavigationService.NavigateAsync("/XamlViewMock?text=Test");
            var page = (XamlViewMock)app.MainPage;
            var layout = (StackLayout)page.Content;
            var partialView = (PartialView)layout.Children.FirstOrDefault(c => c is PartialView);
            var vm = (PartialViewModel)partialView.BindingContext;

            partialView.Navigate();
            Assert.IsType<AutowireView>(app.MainPage);
        }

        [Fact]
        public async Task PartialView_DoesNotResultInMemoryLeak()
        {
            var app = CreateMockApplication();
            await app.NavigationService.NavigateAsync("NavigationPage/XamlViewMock?text=Test");
            var navPage = (NavigationPage)app.MainPage;
            Assert.Single((List<BindableObject>)navPage.CurrentPage.GetValue(ViewModelLocator.PartialViewsProperty));

            await app.NavigationService.NavigateAsync("/AutowireView");

            Assert.Null(app.MainPage.GetValue(ViewModelLocator.PartialViewsProperty));

            await app.NavigationService.NavigateAsync("/XamlViewMock");
            Assert.Single((List<BindableObject>)app.MainPage.GetValue(ViewModelLocator.PartialViewsProperty));
        }
    }
}