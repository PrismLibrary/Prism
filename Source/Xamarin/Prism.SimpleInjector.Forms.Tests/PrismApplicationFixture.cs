using System;
using System.Reflection;
using System.Threading.Tasks;
using SimpleInjector;
using Prism.Common;
using Prism.SimpleInjector.Forms.Tests.Mocks;
using Prism.DI.Forms.Tests.Mocks.Modules;
using Prism.DI.Forms.Tests.Mocks.Services;
using Prism.DI.Forms.Tests.Mocks.ViewModels;
using Prism.DI.Forms.Tests.Mocks.Views;
using Prism.SimpleInjector.Navigation;
using Prism.Navigation;
using Xamarin.Forms;
using Xunit;
using Prism.DI.Forms.Tests;
#if TEST
using Application = Prism.FormsApplication;
#endif

namespace Prism.SimpleInjector.Forms.Tests
{
    public class PrismApplicationFixture
    {
        public PrismApplicationFixture()
        {
            
        }

        [Fact]
        public void OnInitialized()
        {
            var app = new PrismApplicationMock();
            Assert.True(app.Initialized);
        }

        [Fact]
        public void OnInitialized_SetPage()
        {
            var view = new ViewMock();
            var app = new PrismApplicationMock(view);
            Assert.True(app.Initialized);
            Assert.NotNull(Application.Current.MainPage);
            Assert.Same(view, Application.Current.MainPage);
        }

        [Fact]
        public void GetInstanceTypeRegisteredWithContainer()
        {
            var app = new PrismApplicationMock();
            var service = app.Container.GetInstance<IServiceMock>();
            Assert.NotNull(service);
            Assert.IsType<ServiceMock>(service);
        }

        [Fact]
        public void GetInstanceConcreteTypeNotRegisteredWithContainer()
        {
            var app = new PrismApplicationMock();
            Assert.True(app.Initialized);
            var concreteType = app.Container.GetInstance<ConcreteTypeMock>();
            Assert.NotNull(concreteType);
            Assert.IsType<ConcreteTypeMock>(concreteType);
        }

        [Fact]
        public void GetInstanceTypeRegisteredWithDependencyService()
        {
            var app = new PrismApplicationMock();
            var service = app.Container.GetInstance<IDependencyServiceMock>();
            Assert.NotNull(service);
            Assert.IsType<DependencyServiceMock>(service);
        }

        [Fact]
        public void Container_GetInstanceNavigationService()
        {
            var app = new PrismApplicationMock();
            var navigationService = app.NavigationService;
            Assert.NotNull(navigationService);
            Assert.IsType<SimpleInjectorPageNavigationService>(navigationService);
        }

        [Fact]
        public void Module_Initialize()
        {
            var app = new PrismApplicationMock();
            var module = app.Container.GetInstance<ModuleMock>();
            Assert.NotNull(module);
            Assert.True(module.Initialized);
        }

        [Fact]
        public async Task Navigate_UnregisteredView_ThrowContainerException()
        {
            var app = new PrismApplicationMock();
            var navigationService = GetInstanceAndSetRootPage(app);
            var exception = await Assert.ThrowsAsync<SimpleInjectorPageNavigationException>(async () => await navigationService.NavigateAsync("missing"));
            Assert.Contains("missing", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task Navigate_Key()
        {
            var app = new PrismApplicationMock();
            var navigationService = GetInstanceAndSetRootPage(app);
            await navigationService.NavigateAsync("view");
            var rootPage = ((IPageAware)navigationService).Page;
            Assert.True(rootPage.Navigation.ModalStack.Count == 1);
            Assert.IsType(typeof(ViewMock), rootPage.Navigation.ModalStack[0]);
        }

        [Fact]
        public void Navigate_ViewModelFactory_PageAware()
        {
            var app = new PrismApplicationMock();
            var view = new AutowireView();
            var viewModel = (AutowireViewModel)view.BindingContext;
            var pageAware = (IPageAware)viewModel.NavigationService;
            var navigationServicePage = app.CreateNavigationServiceForPage(view);
            Assert.IsType<AutowireView>(pageAware.Page);
            var navigatedPage = ((IPageAware)navigationServicePage).Page;
            Assert.IsType<AutowireView>(navigatedPage);
            Assert.Same(view, pageAware.Page);
            Assert.Same(pageAware.Page, navigatedPage);
        }

        [Fact]
        public void Container_GetInstanceByType()
        {
            var app = new PrismApplicationMock();
            var viewModel = app.Container.GetInstance<ViewModelAMock>();
            Assert.NotNull(viewModel);
            Assert.IsType<ViewModelAMock>(viewModel);
        }

        private static INavigationService GetInstanceAndSetRootPage(PrismApplicationMock app)
        {
            var navigationService = app.NavigationService;
            ((IPageAware)navigationService).Page = new ContentPage();
            return navigationService;
        }
    }
}