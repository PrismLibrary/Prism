using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Layouts;
using Prism.DryIoc.Maui.Tests.Mocks.ViewModels;
using Prism.DryIoc.Maui.Tests.Mocks.Views;
using Prism.Navigation.Regions;
using Prism.Navigation.Xaml;

namespace Prism.DryIoc.Maui.Tests.Fixtures.Regions;

public class RegionFixture : TestBase
{
    public RegionFixture(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
    }

    [Fact]
    public void ContentRegion_CreatedBy_RequestNavigate()
    {
        var mauiApp = CreateBuilder(prism => prism.RegisterTypes(container =>
        {
            container.RegisterForNavigation<MockContentRegionPage, MockContentRegionPageViewModel>();
            container.RegisterForRegionNavigation<MockRegionViewA, MockRegionViewAViewModel>();
        }).CreateWindow(nav => nav.NavigateAsync("MockContentRegionPage"))).Build();
        var window = GetWindow(mauiApp);

        Assert.IsType<MockContentRegionPage>(window.Page);
        var page = window.Page as MockContentRegionPage;
        Assert.NotNull(page.ContentRegion.Content);
        Assert.IsType<MockRegionViewA>(page.ContentRegion.Content);
        Assert.IsType<MockRegionViewAViewModel>(page.ContentRegion.Content.BindingContext);
    }

    [Fact]
    public void FrameRegion_DuplicateRegisterViewWithRegion_ContainsSingleView()
    {
        var mauiApp = CreateBuilder(prism =>
                prism.RegisterTypes(container =>
                {
                    container.RegisterForNavigation<MockContentRegionPage, MockContentRegionPageViewModel>();
                    container.RegisterForRegionNavigation<MockRegionViewA, MockRegionViewAViewModel>();
                })
                .OnInitialized(container =>
                {
                    var regionManager = container.Resolve<IRegionManager>();
                    regionManager.RegisterViewWithRegion("FrameRegion", "MockRegionViewA");
                    regionManager.RegisterViewWithRegion("FrameRegion", "MockRegionViewA");
                })
                .CreateWindow("MockContentRegionPage"))
            .Build();
        var window = GetWindow(mauiApp);

        Assert.IsType<MockContentRegionPage>(window.Page);
        var page = window.Page as MockContentRegionPage;
        Assert.NotNull(page.FrameRegion.Content);
        Assert.IsType<MockRegionViewA>(page.FrameRegion.Content);
    }

    [Fact]
    public void FrameRegion_CreatedBy_RegisterViewWithRegion()
    {
        var mauiApp = CreateBuilder(prism =>
                prism.RegisterTypes(container =>
                {
                    container.RegisterForNavigation<MockContentRegionPage, MockContentRegionPageViewModel>();
                    container.RegisterForRegionNavigation<MockRegionViewA, MockRegionViewAViewModel>();
                })
                .OnInitialized(container =>
                {
                    var regionManager = container.Resolve<IRegionManager>();
                    regionManager.RegisterViewWithRegion("FrameRegion", "MockRegionViewA");
                })
                .CreateWindow("MockContentRegionPage"))
            .Build();
        var window = GetWindow(mauiApp);

        Assert.IsType<MockContentRegionPage>(window.Page);
        var page = window.Page as MockContentRegionPage;
        Assert.NotNull(page.FrameRegion.Content);
        Assert.IsType<MockRegionViewA>(page.FrameRegion.Content);
        Assert.IsType<MockRegionViewAViewModel>(page.FrameRegion.Content.BindingContext);
    }

    [Fact]
    public void Issue3159_LayoutRegion_CreatedBy_RegisterViewWithRegion()
    {
        var mauiApp = CreateBuilder(prism =>
                prism.RegisterTypes(container =>
                {
                    container.RegisterForNavigation<MockContentRegionPage, MockContentRegionPageViewModel>();
                    container.RegisterForRegionNavigation<MockRegionViewA, MockRegionViewAViewModel>();
                })
                .OnInitialized(container =>
                {
                    var regionManager = container.Resolve<IRegionManager>();
                    regionManager.RegisterViewWithRegion("LayoutRegion", "MockRegionViewA");
                })
                .CreateWindow("MockContentRegionPage"))
            .Build();
        var window = GetWindow(mauiApp);

        Assert.IsType<MockContentRegionPage>(window.Page);
        var page = window.Page as MockContentRegionPage;

        Assert.NotNull(page.LayoutRegion.Children);
        Assert.NotEmpty(page.LayoutRegion.Children);
        Assert.IsType<ContentView>(page.LayoutRegion.Children.First());
        Assert.IsType<MockRegionViewAViewModel>(((ContentView)page.LayoutRegion.First()).Content.BindingContext);
    }

    [Fact]
    public void RegionsShareContainer_WithPage()
    {
        var mauiApp = CreateBuilder(prism =>
                prism.RegisterTypes(container =>
                {
                    container.RegisterForNavigation<MockContentRegionPage, MockContentRegionPageViewModel>();
                    container.RegisterForRegionNavigation<MockRegionViewA, MockRegionViewAViewModel>();
                })
                .OnInitialized(container =>
                {
                    var regionManager = container.Resolve<IRegionManager>();
                    regionManager.RegisterViewWithRegion("FrameRegion", "MockRegionViewA");
                })
                .CreateWindow("MockContentRegionPage"))
            .Build();
        var window = GetWindow(mauiApp);

        Assert.IsType<MockContentRegionPage>(window.Page);
        var page = window.Page as MockContentRegionPage;

        var regionManager = mauiApp.Services.GetRequiredService<IRegionManager>();
        var regions = regionManager.Regions.Cast<ITargetAwareRegion>();
        Assert.Equal(3, regions.Count());
        foreach (var region in regions)
        {
            Assert.Same(page.GetContainerProvider(), region.Container);
        }
    }

    [Fact]
    public void RegionViewModel_HasPageAccessor_WithCorrectPage()
    {
        // This validates that the NavigationService is using the correct Page to navigate from
        var mauiApp = CreateBuilder(prism =>
                prism.RegisterTypes(container =>
                {
                    container.RegisterForNavigation<MockContentRegionPage, MockContentRegionPageViewModel>();
                    container.RegisterForRegionNavigation<MockRegionViewA, MockRegionViewAViewModel>();
                })
                .CreateWindow("MockContentRegionPage"))
            .Build();
        var window = GetWindow(mauiApp);

        var regionManager = mauiApp.Services.GetRequiredService<IRegionManager>();
        var region = regionManager.Regions.First(x => x.Name == "ContentRegion");
        var activeView = region.ActiveViews.First();
        Assert.IsType<MockRegionViewA>(activeView);
        var activeViewAsMockRegionViewA = activeView as MockRegionViewA;
        var viewModel = activeViewAsMockRegionViewA.BindingContext as MockRegionViewAViewModel;
        Assert.NotNull(viewModel);

        Assert.NotNull(viewModel.Page);
        Assert.IsType<MockContentRegionPage>(viewModel.Page);
    }

    [Fact]
    public void RegionManager_HasRegionsAmount()
    {
        var mauiApp = CreateBuilder(prism =>
                prism.RegisterTypes(container =>
                {
                    container.RegisterForNavigation<MockContentRegionPage, MockContentRegionPageViewModel>();
                })
                .CreateWindow("MockContentRegionPage"))
            .Build();
        var window = GetWindow(mauiApp);

        var regionManager = mauiApp.Services.GetRequiredService<IRegionManager>();
        Assert.Equal(3, regionManager.Regions.Count());
    }

    [Fact]
    public void PageHas_2_ChildViews()
    {
        var mauiApp = CreateBuilder(prism =>
                prism.RegisterTypes(container =>
                {
                    container.RegisterForNavigation<MockContentRegionPage, MockContentRegionPageViewModel>();
                    container.RegisterForRegionNavigation<MockRegionViewA, MockRegionViewAViewModel>();
                })
                .OnInitialized(container =>
                {
                    var regionManager = container.Resolve<IRegionManager>();
                    regionManager.RegisterViewWithRegion("FrameRegion", "MockRegionViewA");
                })
                .CreateWindow("MockContentRegionPage"))
            .Build();
        var window = GetWindow(mauiApp);

        Assert.IsType<MockContentRegionPage>(window.Page);
        var page = window.Page as MockContentRegionPage;

        var children = page.GetChildRegions();
        Assert.NotNull(children);

        Assert.Equal(2, children.Count());
    }

    [Fact]
    public void RegionWithDefaultView_IsAutoPopulated()
    {
        var mauiApp = CreateBuilder(prism =>
                prism.RegisterTypes(container =>
                {
                    container.RegisterForNavigation<MockPageWithRegionAndDefaultView>("MainPage");
                    container.RegisterForRegionNavigation<MockRegionViewA, MockRegionViewAViewModel>();
                })
                .CreateWindow("MainPage", ex => Assert.Null(ex)))
            .Build();
        var window = GetWindow(mauiApp);

        Assert.IsType<MockPageWithRegionAndDefaultView>(window.Page);
        var page = window.Page as MockPageWithRegionAndDefaultView;

        var region = page.Content as ContentView;

        Assert.NotNull(region.Content);
        Assert.IsType<MockRegionViewA>(region.Content);
    }

    [Fact]
    public async Task Region_IsDestroyed_OnNavigatedAway()
    {
        var mauiApp = CreateBuilder(prism => prism
                .RegisterTypes(container =>
                {
                    container.RegisterForNavigation<MockPageWithRegionAndDefaultView>("MainPage");
                    container.RegisterForRegionNavigation<MockRegionViewA, MockRegionViewAViewModel>();
                })
                .CreateWindow("MainPage"))
            .Build();

        var window = GetWindow(mauiApp);

        var navigationService = Prism.Navigation.Xaml.Navigation.GetNavigationService(window.Page);
        var regionManager = mauiApp.Services.GetRequiredService<IRegionManager>();

        Assert.Single(regionManager.Regions);
        await navigationService.NavigateAsync("/MockViewA");
        Assert.Empty(regionManager.Regions);
    }

    [Fact]
    public async Task Region_IsDestroyed_OnNavigationGoBack()
    {
        var mauiApp = CreateBuilder(prism => prism
                .RegisterTypes(container =>
                {
                    container.RegisterForNavigation<MockPageWithRegionAndDefaultView>("RegionPage");
                    container.RegisterForRegionNavigation<MockRegionViewA, MockRegionViewAViewModel>();
                })
                .CreateWindow("NavigationPage/MockViewA"))
            .Build();

        var window = GetWindow(mauiApp);
        var navPage = window.Page as NavigationPage;

        var navigationService = Prism.Navigation.Xaml.Navigation.GetNavigationService(navPage.RootPage);
        var regionManager = mauiApp.Services.GetRequiredService<IRegionManager>();

        Assert.Empty(regionManager.Regions);
        await navigationService.NavigateAsync("RegionPage");
        Assert.Single(regionManager.Regions);

        await Prism.Navigation.Xaml.Navigation.GetNavigationService(navPage.CurrentPage).GoBackAsync();
        Assert.Empty(regionManager.Regions);

        var result = await navigationService.NavigateAsync("RegionPage");
        Assert.True(result.Success);
    }

    [Fact]
    public void Issue3328_WhenNavigatingToUnregisteredView_ShouldFailWithKeyNotFoundException()
    {
        // Arrange
        var mauiApp = CreateBuilder(prism => prism.RegisterTypes(container =>
        {
            container.RegisterForNavigation<MockContentRegionPage, MockContentRegionPageViewModel>();
            container.RegisterForRegionNavigation<MockRegionViewA, MockRegionViewAViewModel>();
        }).CreateWindow(nav => nav.NavigateAsync("MockContentRegionPage"))).Build();
        var window = GetWindow(mauiApp);

        Assert.IsType<MockContentRegionPage>(window.Page);
        var page = window.Page as MockContentRegionPage;
        Assert.NotNull(page.ContentRegion.Content);
        Assert.IsType<MockRegionViewA>(page.ContentRegion.Content);
        Assert.IsType<MockRegionViewAViewModel>(page.ContentRegion.Content.BindingContext);
        
        // Act
        var regionManager = mauiApp.Services.GetRequiredService<IRegionManager>();
        INavigationResult result = null;
        
        regionManager.RequestNavigate("ContentRegion", "UnregisteredRegion", navResult =>
        {
            result = navResult;
        });
        
        // Assert
        Assert.False(result.Success);
        var ex = Assert.IsType<KeyNotFoundException>(result.Exception);
        Assert.Equal("No view with the name 'UnregisteredRegion' has been registered", ex.Message);
        
        Assert.IsType<MockRegionViewA>(page.ContentRegion.Content);
        Assert.IsType<MockRegionViewAViewModel>(page.ContentRegion.Content.BindingContext);
    }

    [Fact]
    public void Issue3332_NestedContentRegion_InnerRegionReceivesGuest_AfterLayout()
    {
        var mauiApp = CreateBuilder(prism => prism
                .RegisterTypes(container =>
                {
                    container.RegisterForNavigation<MockNestedRegionPage, MockNestedRegionPageViewModel>();
                    container.RegisterForRegionNavigation<MockOuterGuestView, MockOuterGuestViewModel>();
                    container.RegisterForRegionNavigation<MockInnerGuestView, MockInnerGuestViewModel>();
                })
                .CreateWindow("MockNestedRegionPage"))
            .Build();

        var window = GetWindow(mauiApp);
        var page = Assert.IsType<MockNestedRegionPage>(window.Page);
        var regionManager = mauiApp.Services.GetRequiredService<IRegionManager>();

        // Same flow as e2e / community samples: RequestNavigate outer; outer guest RequestNavigate's inner in OnNavigatedTo.
        NavigationResult? outerResult = null;
        regionManager.RequestNavigate("OuterRegion", nameof(MockOuterGuestView), r => outerResult = r);
        Assert.NotNull(outerResult);
        Assert.True(outerResult!.Success, outerResult.Exception?.ToString());

        var outerGuest = Assert.IsType<MockOuterGuestView>(page.OuterRegion.Content);

        const double w = 400;
        const double h = 800;
        page.OuterRegion.Measure(w, h, MeasureFlags.IncludeMargins);
        page.OuterRegion.Arrange(new Rect(0, 0, w, h));
        outerGuest.InnerRegionHost.Measure(w, h, MeasureFlags.IncludeMargins);
        outerGuest.InnerRegionHost.Arrange(new Rect(0, 0, w, h));

        Assert.Contains(regionManager.Regions, r => r.Name == "InnerRegion");

        outerGuest.InnerRegionHost.Measure(w, h, MeasureFlags.IncludeMargins);
        outerGuest.InnerRegionHost.Arrange(new Rect(0, 0, w, h));

        Assert.NotNull(outerGuest.InnerRegionHost.Content);
        Assert.IsType<MockInnerGuestView>(outerGuest.InnerRegionHost.Content);
    }
}
