using System;
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
using Prism.Common;
using Prism.DI.Forms.Tests;
using Prism.DI.Forms.Tests.Fixtures;
using Prism.DI.Forms.Tests.Mocks;
using Prism.DI.Forms.Tests.Mocks.Modules;
using Prism.DI.Forms.Tests.Mocks.Services;
using Prism.DI.Forms.Tests.Mocks.ViewModels;
using Prism.DI.Forms.Tests.Mocks.Views;
using Prism.DI.Forms.Tests.Navigation;
using Prism.Forms.Tests.Mocks.Logging;
using Prism.Ioc;
using Prism.Logging;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Xamarin.Forms;
using Xamarin.Forms.Mocks;
using Xunit;
using Xunit.Abstractions;

#if DryIoc
namespace Prism.DryIoc.Forms.Tests.Fixtures
#elif Unity
namespace Prism.Unity.Forms.Tests.Fixtures
#endif
{
    public class PrismApplicationFixture : FixtureBase
    {
        public PrismApplicationFixture(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }

        [Fact]
        public void OnInitialized()
        {
            var app = CreateMockApplication();
            Assert.True(app.Initialized);
        }

        [Fact]
        public void PlatformTypesRegistered()
        {
            var app = CreateMockApplication();
            Assert.IsAssignableFrom<ITestOutputHelper>(app.Container.Resolve<ITestOutputHelper>());
        }

        [Fact]
        public void ResolvesCustom_ILoggerFacade()
        {
            var app = CreateMockApplication();
            var logger = app.Container.Resolve<ILoggerFacade>();
            logger.Log("Test Message", Category.Debug, Priority.None);
            Assert.IsType<XunitLogger>(logger);
        }

        [Fact]
        public void OnInitialized_SetPage()
        {
            var view = new ViewMock();
            var app = CreateMockApplication(view);
            Assert.True(app.Initialized);
            Assert.NotNull(Application.Current.MainPage);
            Assert.Same(view, Application.Current.MainPage);
        }

        [Fact]
        public void ResolveTypeRegisteredWithContainer()
        {
            var app = CreateMockApplication();
            var service = app.Container.Resolve<IServiceMock>();
            Assert.NotNull(service);
            Assert.IsType<ServiceMock>(service);
        }

        [Fact]
        public void ResolveConcreteTypeNotRegisteredWithContainer()
        {
            var app = CreateMockApplication();
            Assert.True(app.Initialized);
            var concreteType = app.Container.Resolve<ConcreteTypeMock>();
            Assert.NotNull(concreteType);
            Assert.IsType<ConcreteTypeMock>(concreteType);
        }

        [Fact]
        public void ResolveTypeRegisteredWithDependencyService()
        {
            var app = CreateMockApplication();
            var dependencyService = app.Container.Resolve<IDependencyService>();
            Assert.NotNull(dependencyService);
            var service = dependencyService.Get<IDependencyServiceMock>();
            Assert.NotNull(service);
            Assert.IsType<DependencyServiceMock>(service);
        }

        [Fact]
        public void Container_ResolveNavigationService()
        {
            var app = CreateMockApplication();
            var navigationService = app.NavigationService;
            Assert.NotNull(navigationService);
            Assert.IsType<PageNavigationService>(navigationService);
        }

        [Fact]
        public async Task Navigate_With_NavigationPage()
        {
            var app = CreateMockApplication();
            var ex = await Record.ExceptionAsync(() => app.NavigationService.NavigateAsync("NavigationPage/ViewAMock"));
            Assert.Null(ex);
        }

        [Fact]
        public async Task Navigate_UnregisteredView_ThrowException()
        {
            var app = CreateMockApplication();
            var navigationService = ResolveAndSetRootPage(app);

            var result = await navigationService.NavigateAsync("missing");

            Assert.False(result.Success);
            Assert.NotNull(result.Exception);
#if Autofac
            Assert.IsType<ComponentNotRegisteredException>(result.Exception);
#elif DryIoc
            Assert.IsType<ContainerException>(result.Exception);
#elif Ninject
            Assert.IsType<ActivationException>(result.Exception);
#elif Unity
            Assert.IsType<NullReferenceException>(result.Exception);
#endif
            Assert.Contains("missing", result.Exception.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        [Theory]
        [InlineData(typeof(AutowireView), TargetIdiom.Unsupported)]
        [InlineData(typeof(AutowireViewTablet), TargetIdiom.Tablet)]
        public async Task NavigationUses_IdiomSpecificView(Type viewType, TargetIdiom idiom)
        {
            Device.SetIdiom(idiom);
            var initializer = new XunitPlatformInitializer(_testOutputHelper);
            var app = new PrismApplicationMockPlatformAware(initializer);

            Assert.True(app.Initialized);
            await app.NavigationService.NavigateAsync("AutowireView");
            Assert.IsType(viewType, app.MainPage);
            Assert.IsType<AutowireViewModel>(app.MainPage.BindingContext);

            Device.SetIdiom(TargetIdiom.Unsupported);
        }

        [Theory]
        [InlineData(typeof(ViewAMock), "Test")]
        [InlineData(typeof(ViewAMockAndroid), Device.Android)]
        public async Task NavigationUses_PlatformSpecificView(Type viewType, string runtimePlatform)
        {
            MockForms.UpdateRuntimePlatform(runtimePlatform);
            var initializer = new XunitPlatformInitializer(_testOutputHelper);
            var app = new PrismApplicationMockPlatformAware(initializer);

            Assert.True(app.Initialized);
            await app.NavigationService.NavigateAsync("ViewAMock");
            Assert.IsType(viewType, app.MainPage);
            Assert.IsType<ViewModelAMock>(app.MainPage.BindingContext);

            MockForms.UpdateRuntimePlatform("Test");
        }

        [Fact]
        public async Task Navigate_Key()
        {
            var app = CreateMockApplication();
            var navigationService = ResolveAndSetRootPage(app);
            await navigationService.NavigateAsync("view");
            var rootPage = ((IPageAware)navigationService).Page;
            Assert.True(rootPage.Navigation.ModalStack.Count == 1);
            Assert.IsType<ViewMock>(rootPage.Navigation.ModalStack[0]);
        }

        [Fact]
        public void Container_ResolveByType()
        {
            var app = CreateMockApplication();
            var viewModel = app.Container.Resolve<ViewModelAMock>();
            Assert.NotNull(viewModel);
            Assert.IsType<ViewModelAMock>(viewModel);
        }

        [Fact]
        public void Container_ResolveByKey()
        {
            var app = CreateMockApplication();
            var viewModel = app.Container.Resolve<ViewModelBMock>(ViewModelBMock.Key);
            Assert.NotNull(viewModel);
            Assert.IsType<ViewModelBMock>(viewModel);
        }

        [Fact]
        public void Module_Initialize()
        {
            var app = CreateMockApplication();
            var module = app.Container.Resolve<ModuleMock>();
            Assert.NotNull(module);
            Assert.True(module.Initialized);
        }

        [Fact]
        public void CustomNavigation_Resolved_In_PrismApplication()
        {
            var app = new PrismApplicationCustomNavMock(new XunitPlatformInitializer(_testOutputHelper));
            var navService = app.GetNavigationService();
            Assert.NotNull(navService);
            Assert.IsType<CustomNavigationServiceMock>(navService);
        }

        [Fact]
        public void CustomNavigation_Resolved_In_ViewModel()
        {
            var app = new PrismApplicationCustomNavMock(new XunitPlatformInitializer(_testOutputHelper))
            {
                MainPage = new AutowireView()
            };
            var vm = app.MainPage.BindingContext as AutowireViewModel;

            Assert.NotNull(vm);
            Assert.NotNull(vm.NavigationService);
            Assert.IsType<CustomNavigationServiceMock>(vm.NavigationService);
        }

        [Fact]
        public void CustomNamedNavigationService_Resolved_In_ViewModel()
        {
            var app = CreateMockApplication(new CustomNamedNavService());
            var vm = app.MainPage.BindingContext as CustomNamedNavServiceViewModel;

            Assert.NotNull(vm);
            Assert.NotNull(vm.NavigationService);
        }

        [Fact]
        public async Task XamlNavigation_NaviateTo()
        {
            var app = CreateMockApplication();
            await app.NavigationService.NavigateAsync("NavigationPage/XamlViewMockA");
            var navigationPage = (NavigationPage)app.MainPage ;

            Assert.IsType<XamlViewMockA>(navigationPage.CurrentPage);
            var mockA =  navigationPage.CurrentPage as XamlViewMockA;
            mockA.TestButton.SendClicked();
            
            Assert.IsType<XamlViewMockA>(navigationPage.RootPage);
            Assert.IsType<XamlViewMockB>(navigationPage.CurrentPage);
            Assert.True(navigationPage.Pages.Count()==2);
        }

        [Fact]
        public async Task XamlNavigation_GoBack()
        {
            var app = CreateMockApplication();
            await app.NavigationService.NavigateAsync("NavigationPage/XamlViewMockA/XamlViewMockB");
            var navigationPage = (NavigationPage)app.MainPage ;

            Assert.IsType<XamlViewMockB>(navigationPage.CurrentPage);
            var mockB =  navigationPage.CurrentPage as XamlViewMockB;
            mockB.TestButton.SendClicked();
            
            Assert.IsType<XamlViewMockA>(navigationPage.RootPage);
            Assert.IsType<XamlViewMockA>(navigationPage.CurrentPage);
            Assert.True(navigationPage.Pages.Count()==1);
        }

        [Fact]
        public async Task XamlNavigation_BasicNavigation()
        {
            var app = CreateMockApplication();
            await app.NavigationService.NavigateAsync("NavigationPage/XamlViewMockA");
            var navigationPage = (NavigationPage)app.MainPage ;

            Assert.IsType<XamlViewMockA>(navigationPage.CurrentPage);
            var mockA =  navigationPage.CurrentPage as XamlViewMockA;
            mockA.TestButton.SendClicked();
            
            Assert.IsType<XamlViewMockA>(navigationPage.RootPage);
            Assert.IsType<XamlViewMockB>(navigationPage.CurrentPage);
            Assert.True(navigationPage.Pages.Count()==2);

            var mockB =  navigationPage.CurrentPage as XamlViewMockB;
            mockB.TestButton.SendClicked();
            
            Assert.IsType<XamlViewMockA>(navigationPage.RootPage);
            Assert.IsType<XamlViewMockA>(navigationPage.CurrentPage);
        }

        private static INavigationService ResolveAndSetRootPage(PrismApplicationMock app)
        {
            var navigationService = app.NavigationService;
            ((IPageAware)navigationService).Page = new ContentPage();
            return navigationService;
        }
    }
}
