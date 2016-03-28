using System.Reflection;
using System.Threading.Tasks;
using DryIoc;
using Prism.Common;
using Prism.DryIoc.Forms.Tests.Mocks;
using Prism.DryIoc.Forms.Tests.Mocks.ViewModels;
using Prism.DryIoc.Forms.Tests.Services;
using Prism.DryIoc.Navigation;
using Prism.Navigation;
using Xamarin.Forms;
using Xunit;

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
            await navigationService.Navigate<ViewModelAMock>();
            var page = ((IPageAware) navigationService).Page;
            Assert.NotNull(page);
            Assert.IsType<ContentPage>(page);
        }

        [Fact]
        public async Task Navigate_Key()
        {
            var app = new PrismApplicationMock();
            var navigationService = ResolveAndSetRootPage(app);
            await navigationService.Navigate("view");
            var page = ((IPageAware) navigationService).Page;
            Assert.NotNull(page);
            Assert.IsType<ContentPage>(page);
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

        private INavigationService ResolveAndSetRootPage(PrismApplicationMock app)
        {
            var navigationService = app.Container.Resolve<INavigationService>();
            ((IPageAware) navigationService).Page = new ContentPage();
            return navigationService;
        }
    }
}