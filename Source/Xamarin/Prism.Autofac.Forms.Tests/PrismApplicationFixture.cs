﻿using Autofac;
using Autofac.Core.Registration;
using Prism.Autofac.Forms.Tests.Mocks;
using Prism.Common;
using Prism.DI.Forms.Tests;
using Prism.DI.Forms.Tests.Mocks.Services;
using Prism.DI.Forms.Tests.Mocks.ViewModels;
using Prism.DI.Forms.Tests.Mocks.Views;
using Prism.Ioc;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xunit;

namespace Prism.Autofac.Forms.Tests
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
            var dependencyService = app.Container.Resolve<IDependencyService>();
            Assert.NotNull(dependencyService);
            var service = dependencyService.Get<IDependencyServiceMock>();
            Assert.NotNull(service);
            Assert.IsType<DependencyServiceMock>(service);
        }

        [Fact]
        public void Container_ResolveNavigationService()
        {
            var app = new PrismApplicationMock();
            var navigationService = app.NavigationService;
            Assert.NotNull(navigationService);
            Assert.IsType<PageNavigationService>(navigationService);
        }

        [Fact]
        public async Task Navigate_UnregisteredView_ThrowNullReferenceException()
        {
            var app = new PrismApplicationMock();
            var navigationService = ResolveAndSetRootPage(app);
            var exception = await Assert.ThrowsAsync<ComponentNotRegisteredException>(async () => await navigationService.NavigateAsync("missing"));
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
            Assert.IsType<ViewMock>(rootPage.Navigation.ModalStack[0]);
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
            var viewModel = app.Container.Resolve<ViewModelBMock>(ViewModelBMock.Key);
            Assert.NotNull(viewModel);
            Assert.IsType<ViewModelBMock>(viewModel);
        }

        [Fact]
        public void Container_DoesNotSupportModules()
        {
            Assert.Throws<NotSupportedException>(() =>
            {
                var app = new PrismApplicationModulesMock();
            });
        }

        private static INavigationService ResolveAndSetRootPage(PrismApplicationMock app)
        {
            var navigationService = app.NavigationService;
            ((IPageAware)navigationService).Page = new ContentPage();
            return navigationService;
        }
    }
}