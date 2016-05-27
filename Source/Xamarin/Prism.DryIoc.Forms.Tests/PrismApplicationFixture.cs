using System;
using System.Reflection;
using System.Threading.Tasks;
using DryIoc;
using Prism.Common;
using Prism.DryIoc.Forms.Tests.Mocks;
using Prism.DryIoc.Forms.Tests.Mocks.Modules;
using Prism.DryIoc.Forms.Tests.Mocks.Services;
using Prism.DryIoc.Forms.Tests.Mocks.ViewModels;
using Prism.DryIoc.Forms.Tests.Mocks.Views;
using Prism.DryIoc.Navigation;
using Prism.Navigation;
using Xamarin.Forms;
using Xunit;
#if TEST
using Application = Prism.FormsApplication;
#endif

namespace Prism.DryIoc.Forms.Tests
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
            var service = app.Container.Resolve<IDryIocServiceMock>();
            Assert.NotNull(service);
            Assert.IsType<DryIocServiceMock>(service);
        }

        [Fact]
        public void ResolveTypeRegisteredWithDependencyService()
        {
            var app = new PrismApplicationMock();
            // TODO(joacar)
            // Since we must call Xamarin.Forms.Init() (and cannot do so from PCL)
            // to call Xamarin.Forms.DependencyService
            // we check that this throws an InvalidOperationException (for reason stated above).
            // This shows that a call to Xamarin.Forms.DependencyService was made and thus should return
            // service instance (if registered)
            Assert.Throws<TargetInvocationException>(
                () => app.Container.Resolve<IDependencyServiceMock>(IfUnresolved.ReturnDefault));
        }

        [Fact]
        public void Container_ResolveNavigationService()
        {
            var app = new PrismApplicationMock();
            var navigationService = app.Container.Resolve<INavigationService>();
            Assert.NotNull(navigationService);
            Assert.IsType<DryIocPageNavigationService>(navigationService);
        }

        [Fact]
        public async Task Navigate_ViewModel()
        {
            var app = new PrismApplicationMock();
            var navigationService = ResolveAndSetRootPage(app);
            await navigationService.NavigateAsync<ViewModelAMock>();
        }

        [Fact]
        public async Task Navigate_View_ConstructorArguments()
        {
            var app = new PrismApplicationMock();
            var navigationService = ResolveAndSetRootPage(app);
            await navigationService.NavigateAsync<ConstructorArgumentViewModel>();
            var page = ((IPageAware)navigationService).Page;
            Assert.NotNull(page);
            var view = (ConstructorArgumentView)page;
            Assert.NotNull(view.Service);
            var vm = (ConstructorArgumentViewModel)view.BindingContext;
            Assert.NotNull(vm.Service);
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
        public async Task Navigate_UnregisteredView_ThrowInvalidOperationException()
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
        }

        [Fact]
        public async Task Navigate_ViewModelFactory_PageAware()
        {
            var app = new PrismApplicationMock();
            var navigationService = ResolveAndSetRootPage(app);
            await navigationService.NavigateAsync<AutowireViewModel>();
            var page = ((IPageAware)navigationService).Page;
            Assert.NotNull(page);
            Assert.IsType<AutowireView>(page);
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

        private static INavigationService ResolveAndSetRootPage(PrismApplicationMock app)
        {
            var navigationService = app.Container.Resolve<INavigationService>();
            ((IPageAware)navigationService).Page = new ContentPage();
            return navigationService;
        }
    }
}