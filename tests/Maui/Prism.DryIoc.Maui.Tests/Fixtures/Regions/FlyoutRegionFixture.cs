using Microsoft.Extensions.DependencyInjection;
using Prism.Common;
using Prism.Navigation.Regions;
using Prism.Navigation.Xaml;
using PageNavigation = Prism.Navigation.Xaml.Navigation;
using RegionManager = Prism.Navigation.Regions.Xaml.RegionManager;
#if NET10_0
using Microsoft.Maui.Controls.Compatibility.Hosting;
using LegacyStackLayout = Microsoft.Maui.Controls.Compatibility.StackLayout;
#endif

namespace Prism.DryIoc.Maui.Tests.Fixtures.Regions;

public class FlyoutRegionFixture : TestBase
{
    public FlyoutRegionFixture(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
    }

    [Theory]
    [InlineData("MockViewA", nameof(RegionFlyoutPage))]
    [InlineData(nameof(RegionDetailPage), nameof(RegionFlyoutPage))]
#if NET10_0
    // The original repro uses the compatibility adapter, which is not supported
    // by the pinned MAUI 11 RC. The modern adapter is covered on both frameworks.
    [InlineData("MockViewA", nameof(LegacyRegionFlyoutPage))]
    [InlineData(nameof(RegionDetailPage), nameof(LegacyRegionFlyoutPage))]
#endif
    public void ViewDiscovery_PopulatesFlyoutWithOrWithoutDetailRegions(string detailPage, string flyoutPage)
    {
        var mauiApp = CreateFlyoutApp(detailPage, flyoutPage);
        var window = GetWindow(mauiApp);
        var flyout = Assert.IsAssignableFrom<RegionFlyoutPage>(window.Page);

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
        // Layout discovery renders the view without activating it. Child regions
        // enumerate active views, so only the ContentView region contributes here.
        Assert.Same(views[0], Assert.Single(flyout.Flyout.GetChildRegions()));
        Assert.Same(views[1], Assert.Single(RegionManager.GetObservableRegion(flyout.LayoutRegion).Value.Views));
    }

    [Theory]
    [InlineData(nameof(RegionFlyoutPage))]
#if NET10_0
    [InlineData(nameof(LegacyRegionFlyoutPage))]
#endif
    public async Task FlyoutRegions_SurviveDetailNavigation_AndAreDestroyedAndRecreatedWithFlyout(string flyoutPage)
    {
        var mauiApp = CreateFlyoutApp("MockViewA", flyoutPage);
        var window = GetWindow(mauiApp);
        var regionManager = mauiApp.Services.GetRequiredService<IRegionManager>();
        IContainerProvider previousScope = null;
        RegionFlyoutPage previousFlyout = null;

        for (var creation = 0; creation < 3; creation++)
        {
            var flyout = Assert.IsAssignableFrom<RegionFlyoutPage>(window.Page);
            var views = AssertMenuPopulated(flyout);
            var scope = flyout.GetContainerProvider();
            Assert.NotSame(previousFlyout, flyout);
            Assert.NotSame(previousScope, scope);
            RegionManager.GetObservableRegion(flyout.LayoutRegion).Value.Activate(views[1]);
            Assert.Equal(2, flyout.Flyout.GetChildRegions().Count());
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
                    .NavigateAsync($"/{flyoutPage}/NavigationPage/MockViewA");
                Assert.True(reopen.Success, reopen.Exception?.ToString());
            }
        }
    }

    [Fact]
    public async Task PageHostedFlyoutRegion_InheritsScopeWithoutOwningIt_AndIsDestroyedWithFlyout()
    {
        var mauiApp = CreateFlyoutApp("MockViewA", nameof(PageRegionFlyoutPage));
        var window = GetWindow(mauiApp);
        var flyout = Assert.IsType<PageRegionFlyoutPage>(window.Page);
        var menu = Assert.IsType<ContentPage>(flyout.Flyout);
        var view = Assert.IsType<TrackingView>(menu.Content);
        var viewModel = Assert.IsType<TrackingViewModel>(view.BindingContext);
        var scope = flyout.GetContainerProvider();

        Assert.Same(scope, menu.GetContainerProvider());
        Assert.Same(flyout, scope.Resolve<IPageAccessor>().Page);
        Assert.Same(flyout, viewModel.PageAccessor.Page);
        Assert.Same(PageNavigation.GetNavigationService(flyout), viewModel.NavigationService);
        Assert.NotSame(scope, window.CurrentPage.GetContainerProvider());
        Assert.Single(menu.GetChildRegions());
        var regionManager = mauiApp.Services.GetRequiredService<IRegionManager>();
        var region = Assert.Single(regionManager.Regions);
        Assert.Same(menu, Assert.IsAssignableFrom<ITargetAwareRegion>(region).TargetElement);

        var leaveFlyout = await viewModel.NavigationService.NavigateAsync("/MockViewA");

        Assert.True(leaveFlyout.Success, leaveFlyout.Exception?.ToString());
        Assert.Empty(regionManager.Regions);
        Assert.Null(menu.GetChildRegions());
        Assert.Equal(1, view.DestroyCount);
        Assert.Equal(1, viewModel.DestroyCount);
    }

    private MauiApp CreateFlyoutApp(string detailPage, string flyoutPage = nameof(RegionFlyoutPage)) =>
        CreateBuilder(prism => prism
            .RegisterTypes(container => container
                .RegisterForNavigation<RegionFlyoutPage>()
#if NET10_0
                .RegisterForNavigation<LegacyRegionFlyoutPage>()
#endif
                .RegisterForNavigation<PageRegionFlyoutPage>()
                .RegisterForNavigation<RegionDetailPage>()
                .RegisterForRegionNavigation<TrackingView, TrackingViewModel>())
            .OnInitialized(container => container.Resolve<IRegionManager>()
                .RegisterViewWithRegion<TrackingView>("LayoutRegionFlyout")
                .RegisterViewWithRegion<TrackingView>("ContentViewRegionFlyout")
                .RegisterViewWithRegion<TrackingView>("RegionA"))
            .CreateWindow($"{flyoutPage}/NavigationPage/{detailPage}"))
#if NET10_0
            .UseMauiCompatibility()
#endif
            .Build();

    private static TrackingView[] AssertMenuPopulated(RegionFlyoutPage flyout)
    {
        var contentView = Assert.IsType<TrackingView>(flyout.ContentRegion.Content);
        var layoutChildren = flyout.LayoutRegion switch
        {
            Layout layout => layout.Children.Cast<View>(),
#if NET10_0
            LegacyStackLayout layout => layout.Children,
#endif
            _ => throw new NotSupportedException()
        };
        var layoutItem = Assert.IsType<ContentView>(Assert.Single(layoutChildren));
        var layoutView = Assert.IsType<TrackingView>(layoutItem.Content);
        return [contentView, layoutView];
    }

    public class RegionFlyoutPage : FlyoutPage
    {
        public View LayoutRegion { get; }

        public ContentView ContentRegion { get; } = new();

        public RegionFlyoutPage()
            : this(new Microsoft.Maui.Controls.StackLayout())
        {
        }

        protected RegionFlyoutPage(View layoutRegion)
        {
            LayoutRegion = layoutRegion;
            RegionManager.SetRegionName(LayoutRegion, "LayoutRegionFlyout");
            RegionManager.SetRegionName(ContentRegion, "ContentViewRegionFlyout");
            Flyout = new ContentPage
            {
                Title = "Menu",
                Content = new VerticalStackLayout { Children = { LayoutRegion, ContentRegion } }
            };
        }
    }

#if NET10_0
    public sealed class LegacyRegionFlyoutPage : RegionFlyoutPage
    {
        public LegacyRegionFlyoutPage()
            : base(new LegacyStackLayout())
        {
        }
    }
#endif

    public sealed class RegionDetailPage : ContentPage
    {
        public RegionDetailPage()
        {
            var region = new ContentView();
            RegionManager.SetRegionName(region, "RegionA");
            Content = region;
        }
    }

    public sealed class PageRegionFlyoutPage : FlyoutPage
    {
        public PageRegionFlyoutPage()
        {
            var menu = new ContentPage { Title = "Menu" };
            RegionManager.SetRegionName(menu, "ContentViewRegionFlyout");
            Flyout = menu;
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
