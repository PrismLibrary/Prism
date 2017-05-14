using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Prism.Common;
using Prism.Autofac.Forms.Tests.Mocks;
using Prism.DI.Forms.Tests.Mocks.Modules;
using Prism.DI.Forms.Tests.Mocks.Services;
using Prism.DI.Forms.Tests.Mocks.ViewModels;
using Prism.DI.Forms.Tests.Mocks.Views;
using Prism.Autofac.Navigation;
using Prism.Navigation;
using Xamarin.Forms;
using Xunit;
using Autofac;
using Prism.DI.Forms.Tests;
#if TEST
using Application = Prism.FormsApplication;
#endif

namespace Prism.Autofac.Forms.Tests
{
    public class PrismApplicationFixture
    {
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
        public void ResolveTypeRegisteredWithContainer()
        {
            var app = new PrismApplicationMock();
            var service = app.Container.Resolve<IServiceMock>();
            Assert.NotNull(service);
            Assert.IsType<ServiceMock>(service);
        }

        [Fact]
        public void ResolveConcreteTypeNotRegisteredWithContainer()
        {
            var app = new PrismApplicationMock();
            Assert.True(app.Initialized);
            var concreteType = app.Container.Resolve<ConcreteTypeMock>();
            Assert.NotNull(concreteType);
            Assert.IsType<ConcreteTypeMock>(concreteType);
        }

        [Fact]
        public void ResolveTypeRegisteredWithDependencyService()
        {
            var app = new PrismApplicationMock();

            //Was: Autofac needs to be updated to support resolving unknown interfaces by using the DependencyService
            //Assert.Throws<ComponentNotRegisteredException>(() => app.Container.Resolve<IDependencyServiceMock>());

            //  Update 2017-04-03: Probably don't need this support, since use of DependencyService is being deprecated.
            //  So added "support" for it by secondarily registering the service directly in the Autofac container.
            var service = app.Container.Resolve<IDependencyServiceMock>();
            Assert.NotNull(service);
            Assert.IsType<DependencyServiceMock>(service);
        }

        [Fact]
        public void Container_ResolveNavigationService()
        {
            var app = new PrismApplicationMock();
            var navigationService = app.NavigationService;
            Assert.NotNull(navigationService);
            Assert.IsType<AutofacPageNavigationService>(navigationService);
        }

        [Fact]
        public void Module_Initialize()
        {
            var app = new PrismApplicationMock();
            var module = app.Container.Resolve<ModuleMock>();
            Assert.NotNull(module);
            Assert.True(module.Initialized);
        }

        [Fact]
        public async Task Navigate_UnregisteredView_ThrowNullReferenceException()
        {
            var app = new PrismApplicationMock();
            var navigationService = ResolveAndSetRootPage(app);
            var exception = await Assert.ThrowsAsync<NullReferenceException>(async () => await navigationService.NavigateAsync("missing"));
            Assert.Contains("missing", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task Navigate_Key()
        {
            var app = new PrismApplicationMock();
            var navigationService = ResolveAndSetRootPage(app);
            await navigationService.NavigateAsync("view");
            var rootPage = ((IPageAware)navigationService).Page;
            Assert.True(rootPage.Navigation.ModalStack.Count == 1);
            Assert.IsType(typeof(ViewMock), rootPage.Navigation.ModalStack[0]);
        }

        [Fact]
        public void Container_ResolveByType()
        {
            var app = new PrismApplicationMock();
            var viewModel = app.Container.Resolve<ViewModelAMock>();
            Assert.NotNull(viewModel);
            Assert.IsType<ViewModelAMock>(viewModel);
        }

        [Fact]
        public void Container_ResolveByKey()
        {
            var app = new PrismApplicationMock();
            var viewModel = app.Container.ResolveNamed<ViewModelBMock>(ViewModelBMock.Key);
            Assert.NotNull(viewModel);
            Assert.IsType<ViewModelBMock>(viewModel);
        }

        private static INavigationService ResolveAndSetRootPage(PrismApplicationMock app)
        {
            var navigationService = app.NavigationService;
            ((IPageAware)navigationService).Page = new ContentPage();
            return navigationService;
        }

        [Fact]
        public void Container_IsImmutableAfterBuild()
        {
            //Arrange
            var app = new PrismApplicationMock();

            //Act
            //Referencing the Container's ComponentRegistry causes the Container to be built
            //  and no further type registration should be possible.
            var componentRegistry = app.Container.ComponentRegistry;

            //Assert
            Assert.True(app.Container.IsContainerBuilt);
            Assert.Throws<InvalidOperationException>(() => app.Container.Register(ctx => new List<string>())
                .As<IList<string>>());
            //But type resolution will work properly
            Assert.NotNull(app.Container.Resolve<ViewModelAMock>());
        }
    }
}