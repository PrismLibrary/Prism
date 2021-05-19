using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Prism.DI.Forms.Tests.Mocks.ViewModels;
using Prism.DI.Forms.Tests.Mocks.Views;
using Prism.Ioc;
using Xunit;
using Xunit.Abstractions;

namespace Prism.DI.Forms.Tests.Fixtures.Regions
{
    public class RegionFixture : FixtureBase, IPlatformInitializer
    {
        private PrismApplicationMock _app;

        public RegionFixture(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
            _app = new PrismApplicationMock(this);
        }

        [Fact]
        public async Task RegionWorksWhenContentViewIsTopChild()
        {
            var result = await _app.NavigationService.NavigateAsync("Issue2415Page");
            Assert.Null(result.Exception);
            Assert.NotNull(_app.MainPage);
            Assert.IsType<Issue2415Page>(_app.MainPage);

            var vm = _app.MainPage.BindingContext as Issue2415PageViewModel;

            Assert.NotNull(vm.Result);
            Assert.True(vm.Result.Result);
        }

        void IPlatformInitializer.RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterInstance(_testOutputHelper);
            containerRegistry.RegisterRegionServices();
            containerRegistry.RegisterForNavigation<Issue2415Page, Issue2415PageViewModel>();
            containerRegistry.RegisterForRegionNavigation<Issue2415RegionView, Issue2415RegionViewModel>();
        }
    }
}
