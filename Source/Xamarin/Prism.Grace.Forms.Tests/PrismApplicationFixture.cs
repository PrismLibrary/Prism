﻿using Prism.Common;
using Prism.DI.Forms.Tests;
using Prism.DI.Forms.Tests.Mocks.Modules;
using Prism.DI.Forms.Tests.Mocks.Services;
using Prism.DI.Forms.Tests.Mocks.ViewModels;
using Prism.DI.Forms.Tests.Mocks.Views;
using Prism.Grace.Forms.Tests.Mocks;
using Prism.Grace.Navigation;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xunit;

namespace Prism.Grace.Forms.Tests
{
    namespace Prism.Grace.Forms.Tests
    {
        public class PrismApplicationFixture
        {
            public PrismApplicationFixture()
            {
                Xamarin.Forms.Mocks.MockForms.Init();
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
            public void ResolveTypeRegisteredWithContainer()
            {
                var app = new PrismApplicationMock();
                var service = app.Container.Locate<IServiceMock>();
                Assert.NotNull(service);
                Assert.IsType<ServiceMock>(service);
            }

            [Fact]
            public void ResolveConcreteTypeNotRegisteredWithContainer()
            {
                var app = new PrismApplicationMock();
                Assert.True(app.Initialized);
                var concreteType = app.Container.Locate<ConcreteTypeMock>();
                Assert.NotNull(concreteType);
                Assert.IsType<ConcreteTypeMock>(concreteType);
            }

            [Fact]
            public void ResolveTypeRegisteredWithDependencyService()
            {
                var app = new PrismApplicationMock();
                var dependencyService = app.Container.Locate<IDependencyService>();
                Assert.NotNull(dependencyService);
                var service = dependencyService.Get<IDependencyServiceMock>();
                Assert.NotNull(service);
                Assert.IsType<DependencyServiceMock>(service);
            }

            [Fact]
            public void IsSingeltonAcrossScopesAndLifestyles()
            {
                var app = new PrismApplicationMock();
                var dependencyManagerA = app.Container.Locate<ModuleMock>();
                var dependencyManagerB = app.Container.Locate<ModuleMock>();
                Assert.NotNull(dependencyManagerA);
                Assert.NotNull(dependencyManagerB);
                Assert.Same(dependencyManagerA, dependencyManagerB);
            }

            [Fact]
            public void Container_ResolveNavigationService()
            {
                var app = new PrismApplicationMock();
                var navigationService = app.NavigationService;
                Assert.NotNull(navigationService);
                Assert.IsType<GracePageNavigationService>(navigationService);
            }

            [Fact]
            public void Module_Initialize()
            {
                var app = new PrismApplicationMock();
                var module = app.Container.Locate<ModuleMock>();
                Assert.NotNull(module);
                Assert.True(module.Initialized);
            }

            [Fact]
            public async Task Navigate_UnregisteredView_ThrowContainerException()
            {
                var app = new PrismApplicationMock();
                var navigationService = ResolveAndSetRootPage(app);
                var exception = await Assert.ThrowsAsync<NullReferenceException>(async () => await navigationService.NavigateAsync("missing"));
                //Assert.Contains("missing", exception.Message, StringComparison.OrdinalIgnoreCase);
            }

            [Fact]
            public async Task Navigate_Key()
            {
                var app = new PrismApplicationMock();
                var navigationService = ResolveAndSetRootPage(app);
                await navigationService.NavigateAsync("view");
                var rootPage = ((IPageAware)navigationService).Page;
                Assert.True(rootPage.Navigation.ModalStack.Count == 1);
                Assert.IsType<ViewMock>(rootPage.Navigation.ModalStack[0]);
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
            public void Container_ResolveByType()
            {
                var app = new PrismApplicationMock();
                var viewModel = app.Container.Locate<ViewModelAMock>();
                Assert.NotNull(viewModel);
                Assert.IsType<ViewModelAMock>(viewModel);
            }

            [Fact]
            public void Container_ResolveByKey()
            {
                var app = new PrismApplicationMock();
                var viewModel = app.Container.Locate<ViewModelBMock>(withKey: ViewModelBMock.Key);
                Assert.NotNull(viewModel);
                Assert.IsType<ViewModelBMock>(viewModel);
            }

            private static INavigationService ResolveAndSetRootPage(PrismApplicationMock app)
            {
                var navigationService = app.NavigationService;
                ((IPageAware)navigationService).Page = new ContentPage();
                return navigationService;
            }
        }
    }
}
