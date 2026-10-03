using Microsoft.Extensions.DependencyInjection;
using Prism.Common;
using Prism.Navigation.Regions;
using Prism.Navigation.Xaml;
using PageNavigation = Prism.Navigation.Xaml.Navigation;
using RegionManager = Prism.Navigation.Regions.Xaml.RegionManager;
using StackLayout = Microsoft.Maui.Controls.Compatibility.StackLayout;

namespace Prism.DryIoc.Maui.Tests.Fixtures.Regions;

public class FlyoutRegionFixture : TestBase
{
    public FlyoutRegionFixture(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
    }

    [Theory]
    [InlineData("MockViewA")]
    [InlineData(nameof(RegionDetailPage))]
    public void ViewDiscovery_PopulatesFlyoutWithOrWithoutDetailRegions(string detailPage)
    {
        var mauiApp = CreateFlyoutApp(detailPage);
        var window = GetWindow(mauiApp);
        var flyout = Assert.IsType<RegionFlyoutPage>(window.Page);

        // Inspect the controls first: enumerating IRegionManager.Regions would mask #3161.
        var views = AssertMenuPopulated(flyout);
        var scope = flyout.GetContainerProvider();
        Assert.Same(scope, flyout.Flyout.GetContainerProvider());
        Assert.NotSame(scope, flyout.Detail.GetContainerProvider());
        Assert.NotSame(scope, window.CurrentPage.GetContainerProvider());
        Assert.Same(flyout, scope.Resolve<IPageAccessor>().Page);
        Assert.All(views, view =>
        {
            var viewModel = Assert.IsType<TrackingViewModel>(view.BindingContext);
            Assert.Same(flyout, viewModel.PageAccessor.Page);
            Assert.Same(PageNavigation.GetNavigationService(flyout), viewModel.NavigationService);
        });

        var regionManager = mauiApp.Services.GetRequiredService<IRegionManager>();
        Assert.Equal(detailPage == nameof(RegionDetailPage) ? 3 : 2, regionManager.Regions.Count());
        Assert.Equal(2, flyout.Flyout.GetChildRegions().Count());
    }

    [Fact]
    public async Task FlyoutRegions_SurviveDetailNavigation_AndAreDestroyedAndRecreatedWithFlyout()
    {
        var mauiApp = CreateFlyoutApp("MockViewA");
        var window = GetWindow(mauiApp);
        var regionManager = mauiApp.Services.GetRequiredService<IRegionManager>();
        IContainerProvider previousScope = null;
        RegionFlyoutPage previousFlyout = null;

        for (var creation = 0; creation < 3; creation++)
        {
            var flyout = Assert.IsType<RegionFlyoutPage>(window.Page);
            var views = AssertMenuPopulated(flyout);
            var scope = flyout.GetContainerProvider();
            Assert.NotSame(previousFlyout, flyout);
            Assert.NotSame(previousScope, scope);
            var viewModel = Assert.IsType<TrackingViewModel>(views[0].BindingContext);
            var originalDetail = flyout.Detail;

            var changeDetail = await viewModel.NavigationService.NavigateAsync("NavigationPage/MockViewB");

            Assert.True(changeDetail.Success, changeDetail.Exception?.ToString());
            Assert.Same(flyout, window.Page);
            Assert.NotSame(originalDetail, flyout.Detail);
            Assert.Equal(views, AssertMenuPopulated(flyout));
            Assert.All(views, view => Assert.Equal(0, view.DestroyCount));
            Assert.Equal(2, regionManager.Regions.Count());

            var leaveFlyout = await viewModel.NavigationService.NavigateAsync("/MockViewA");

            Assert.True(leaveFlyout.Success, leaveFlyout.Exception?.ToString());
            Assert.Empty(regionManager.Regions);
            Assert.Null(flyout.Flyout.GetChildRegions());
            Assert.All(views, view =>
            {
                Assert.Equal(1, view.DestroyCount);
                Assert.Equal(1, Assert.IsType<TrackingViewModel>(view.BindingContext).DestroyCount);
            });

            previousScope = scope;
            previousFlyout = flyout;
            if (creation < 2)
            {
                var reopen = await PageNavigation.GetNavigationService(window.Page)
                    .NavigateAsync($"/{nameof(RegionFlyoutPage)}/NavigationPage/MockViewA");
                Assert.True(reopen.Success, reopen.Exception?.ToString());
            }
        }
    }

    private MauiApp CreateFlyoutApp(string detailPage) =>
        CreateBuilder(prism => prism
            .RegisterTypes(container => container
                .RegisterForNavigation<RegionFlyoutPage>()
                .RegisterForNavigation<RegionDetailPage>()
                .RegisterForRegionNavigation<TrackingView, TrackingViewModel>())
            .OnInitialized(container => container.Resolve<IRegionManager>()
                .RegisterViewWithRegion<TrackingView>("LayoutRegionFlyout")
                .RegisterViewWithRegion<TrackingView>("ContentViewRegionFlyout")
                .RegisterViewWithRegion<TrackingView>("RegionA"))
            .CreateWindow($"{nameof(RegionFlyoutPage)}/NavigationPage/{detailPage}"))
            .Build();

    private static TrackingView[] AssertMenuPopulated(RegionFlyoutPage flyout)
    {
        var contentView = Assert.IsType<TrackingView>(flyout.ContentRegion.Content);
        var layoutItem = Assert.IsType<ContentView>(Assert.Single(flyout.LayoutRegion.Children));
        var layoutView = Assert.IsType<TrackingView>(layoutItem.Content);
        return [contentView, layoutView];
    }

    public sealed class RegionFlyoutPage : FlyoutPage
    {
        public StackLayout LayoutRegion { get; } = new();

        public ContentView ContentRegion { get; } = new();

        public RegionFlyoutPage()
        {
            RegionManager.SetRegionName(LayoutRegion, "LayoutRegionFlyout");
            RegionManager.SetRegionName(ContentRegion, "ContentViewRegionFlyout");
            Flyout = new ContentPage
            {
                Title = "Menu",
                Content = new VerticalStackLayout { Children = { LayoutRegion, ContentRegion } }
            };
        }
    }

    public sealed class RegionDetailPage : ContentPage
    {
        public RegionDetailPage()
        {
            var region = new ContentView();
            RegionManager.SetRegionName(region, "RegionA");
            Content = region;
        }
    }

    public sealed class TrackingView : ContentView, IDestructible
    {
        public int DestroyCount { get; private set; }

        public void Destroy() => DestroyCount++;
    }

    public sealed class TrackingViewModel : IDestructible
    {
        public TrackingViewModel(IPageAccessor pageAccessor, INavigationService navigationService)
        {
            PageAccessor = pageAccessor;
            NavigationService = navigationService;
        }

        public IPageAccessor PageAccessor { get; }

        public INavigationService NavigationService { get; }

        public int DestroyCount { get; private set; }

        public void Destroy() => DestroyCount++;
    }
}
