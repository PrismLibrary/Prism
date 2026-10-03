using Microsoft.Extensions.DependencyInjection;
using Prism.Common;
using Prism.DryIoc.Maui.Tests.Mocks.Views;
using Prism.Navigation.Regions;
using Prism.Navigation.Regions.Adapters;
using Prism.Navigation.Regions.Behaviors;
using Prism.Navigation.Xaml;
using PageNavigation = Prism.Navigation.Xaml.Navigation;
using RegionManager = Prism.Navigation.Regions.Xaml.RegionManager;
using TabbedPage = Microsoft.Maui.Controls.TabbedPage;

namespace Prism.DryIoc.Maui.Tests.Fixtures.Regions;

public class ContentPageRegionFixture : TestBase
{
    public ContentPageRegionFixture(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task RegionPage_UsesInjectedViewModelAndTitleLifecycle(bool useServiceCollection)
    {
        var builder = CreateBuilder(prism => prism
            .RegisterTypes(container =>
            {
                if (!useServiceCollection)
                    container.RegisterRegionPage("RegionPage", "MainRegion");
                container.RegisterForNavigation<ContentPage>("PlainPage");
            })
            .CreateWindow("NavigationPage/RegionPage?title=Initial"));
        if (useServiceCollection)
            builder.Services.RegisterRegionPage("RegionPage", "MainRegion");

        var mauiApp = builder.Build();
        var navigationPage = Assert.IsAssignableFrom<NavigationPage>(GetWindow(mauiApp).Page);
        var page = Assert.IsType<RegionPage>(navigationPage.CurrentPage);
        var viewModel = Assert.IsType<RegionPageViewModel>(page.BindingContext);
        Assert.Equal("Initial", viewModel.Title);
        Assert.Equal("Initial", page.Title);
        Assert.Equal("MainRegion", RegionManager.GetRegionName(page));
        Assert.All(mauiApp.Services.GetRequiredService<INavigationRegistry>().Registrations,
            registration => Assert.IsType<ViewRegistration>(registration));

        var notifications = 0;
        viewModel.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(RegionPageViewModel.Title))
                notifications++;
        };
        foreach (var title in new[] { "Updated", "Updated", null })
        {
            var push = await PageNavigation.GetNavigationService(page).NavigateAsync("PlainPage");
            Assert.True(push.Success, push.Exception?.ToString());
            var parameters = new NavigationParameters();
            if (title is not null)
                parameters.Add(KnownNavigationParameters.Title, title);
            var back = await PageNavigation.GetNavigationService(navigationPage.CurrentPage).GoBackAsync(parameters);
            Assert.True(back.Success, back.Exception?.ToString());
            Assert.Same(page, navigationPage.CurrentPage);
            Assert.Same(viewModel, page.BindingContext);
            Assert.Equal("Updated", viewModel.Title);
            Assert.Equal("Updated", page.Title);
        }
        Assert.Equal(1, notifications);
    }

    [Fact]
    public void RegionPageViewModel_InitializationAndNavigationPreserveMissingOrUnchangedTitle()
    {
        var viewModel = new RegionPageViewModel();
        viewModel.Initialize(new NavigationParameters());
        Assert.Null(viewModel.Title);
        var notifications = 0;
        viewModel.PropertyChanged += (_, _) => notifications++;
        var parameters = new NavigationParameters { { KnownNavigationParameters.Title, "Initial" } };
        viewModel.Initialize(parameters);
        viewModel.OnNavigatedTo(parameters);
        viewModel.Initialize(new NavigationParameters());
        viewModel.OnNavigatedTo(new NavigationParameters());
        viewModel.OnNavigatedFrom(new NavigationParameters { { KnownNavigationParameters.Title, "Ignored" } });
        Assert.Equal("Initial", viewModel.Title);
        Assert.Equal(1, notifications);
        viewModel.OnNavigatedTo(new NavigationParameters { { KnownNavigationParameters.Title, "Updated" } });
        Assert.Equal("Updated", viewModel.Title);
        Assert.Equal(2, notifications);
    }

    [Fact]
    public void RegisterRegionPage_ReturnsContainerRegistry()
    {
        CreateBuilder(prism => prism.RegisterTypes(container =>
        {
            Assert.Same(container, container.RegisterRegionPage("RegionPage", "MainRegion"));
        })).Build();
    }

    [Fact]
    public void RegisterRegionPage_RejectsNullContainer()
    {
        var exception = Assert.Throws<ArgumentNullException>(() =>
            RegionNavigationRegistrationExtensions.RegisterRegionPage((IContainerRegistry)null, "RegionPage", "MainRegion"));

        Assert.Equal("containerRegistry", exception.ParamName);
    }

    [Fact]
    public void ServiceCollectionRegisterRegionPage_RejectsNullServices()
    {
        var exception = Assert.Throws<ArgumentNullException>(() =>
            RegionNavigationRegistrationExtensions.RegisterRegionPage((IServiceCollection)null, "RegionPage", "MainRegion"));

        Assert.Equal("services", exception.ParamName);
    }

    [Theory]
    [InlineData(null, "MainRegion", "name")]
    [InlineData("", "MainRegion", "name")]
    [InlineData(" \t", "MainRegion", "name")]
    [InlineData("RegionPage", null, "regionName")]
    [InlineData("RegionPage", "", "regionName")]
    [InlineData("RegionPage", " \t", "regionName")]
    public void ServiceCollectionRegisterRegionPage_RejectsMissingNames(string name, string regionName, string parameterName)
    {
        var services = new ServiceCollection();

        var exception = Assert.ThrowsAny<ArgumentException>(() => services.RegisterRegionPage(name, regionName));

        Assert.Equal(parameterName, exception.ParamName);
        Assert.Empty(services);
    }

    [Theory]
    [InlineData(null, "MainRegion", "name")]
    [InlineData("", "MainRegion", "name")]
    [InlineData(" \t", "MainRegion", "name")]
    [InlineData("RegionPage", null, "regionName")]
    [InlineData("RegionPage", "", "regionName")]
    [InlineData("RegionPage", " \t", "regionName")]
    public void RegisterRegionPage_RejectsMissingNames(string name, string regionName, string parameterName)
    {
        CreateBuilder(prism => prism.RegisterTypes(container =>
        {
            var exception = Assert.ThrowsAny<ArgumentException>(() => container.RegisterRegionPage(name, regionName));

            Assert.Equal(parameterName, exception.ParamName);
        })).Build();
    }

    [Fact]
    public void ContentPage_HasDefaultRegionAdapter()
    {
        var mauiApp = CreateBuilder(_ => { }).Build();
        var mappings = mauiApp.Services.GetRequiredService<RegionAdapterMappings>();

        Assert.IsType<ContentPageRegionAdapter>(mappings.GetMapping<ContentPage>());
        Assert.IsType<ContentPageRegionAdapter>(mappings.GetMapping<MockViewA>());
    }

    [Fact]
    public void CustomContentPageAdapter_IsNotReplacedByDefaultMapping()
    {
        var mauiApp = CreateBuilder(prism => prism
            .ConfigureRegionAdapters(mappings => mappings.RegisterMapping<ContentPage, CustomContentPageRegionAdapter>())
            .RegisterTypes(container => container.RegisterRegionPage("RegionPage", "MainRegion"))
            .CreateWindow("RegionPage"))
            .Build();
        var window = GetWindow(mauiApp);
        var mappings = mauiApp.Services.GetRequiredService<RegionAdapterMappings>();
        var adapter = Assert.IsType<CustomContentPageRegionAdapter>(mappings.GetMapping<ContentPage>());

        Assert.Same(window.Page, adapter.AdaptedPage);
        Assert.IsType<SingleActiveRegion>(RegionManager.GetObservableRegion(window.Page).Value);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void RegionPageAliases_HaveIndependentRegionsAndPageScopes(bool useServiceCollection)
    {
        var builder = CreateBuilder(prism => prism
            .RegisterTypes(container =>
            {
                if (!useServiceCollection)
                    container.RegisterRegionPage("FirstPage", "FirstRegion")
                        .RegisterRegionPage("SecondPage", "SecondRegion");
            })
            .CreateWindow("NavigationPage/FirstPage/SecondPage"));
        if (useServiceCollection)
        {
            Assert.Same(builder.Services, builder.Services.RegisterRegionPage("FirstPage", "FirstRegion"));
            builder.Services.RegisterRegionPage("SecondPage", "SecondRegion");
        }

        var mauiApp = builder.Build();
        var window = GetWindow(mauiApp);
        var navigationPage = Assert.IsAssignableFrom<NavigationPage>(window.Page);
        var firstPage = Assert.IsAssignableFrom<ContentPage>(navigationPage.RootPage);
        var secondPage = Assert.IsAssignableFrom<ContentPage>(navigationPage.CurrentPage);
        var regionManager = mauiApp.Services.GetRequiredService<IRegionManager>();
        var firstRegion = Assert.Single(regionManager.Regions, region => region.Name == "FirstRegion");
        var secondRegion = Assert.Single(regionManager.Regions, region => region.Name == "SecondRegion");

        Assert.Equal(2, navigationPage.Navigation.NavigationStack.Count);
        Assert.Equal(2, regionManager.Regions.Count());
        Assert.NotSame(firstPage, secondPage);
        Assert.NotSame(firstPage.GetContainerProvider(), secondPage.GetContainerProvider());
        Assert.Equal("FirstPage", ViewModelLocator.GetNavigationName(firstPage));
        Assert.Equal("SecondPage", ViewModelLocator.GetNavigationName(secondPage));
        Assert.Equal("FirstRegion", RegionManager.GetRegionName(firstPage));
        Assert.Equal("SecondRegion", RegionManager.GetRegionName(secondPage));
        Assert.Same(firstPage, Assert.IsAssignableFrom<ITargetAwareRegion>(firstRegion).TargetElement);
        Assert.Same(secondPage, Assert.IsAssignableFrom<ITargetAwareRegion>(secondRegion).TargetElement);

        var firstView = new ContentView();
        var secondView = new ContentView();
        firstRegion.Add(firstView);
        secondRegion.Add(secondView);

        Assert.Same(firstView, firstPage.Content);
        Assert.Same(secondView, secondPage.Content);
    }

    [Theory]
    [InlineData("TabbedPage?createTab=Dashboard&createTab=Reports&selectedTab=Reports")]
    [InlineData("TabbedPage?createTab=NavigationPage%2FDashboard&createTab=NavigationPage%2FReports&selectedTab=NavigationPage%7CReports")]
    public void SelectedTab_MatchesRegionPageAlias_InsteadOfSharedContentPageType(string uri)
    {
        var mauiApp = CreateBuilder(prism => prism
            .RegisterTypes(container => container
                .RegisterRegionPage("Dashboard", "DashboardRegion")
                .RegisterRegionPage("Reports", "ReportsRegion"))
            .CreateWindow(uri))
            .Build();
        var window = GetWindow(mauiApp);
        var tabbedPage = Assert.IsType<TabbedPage>(window.Page);
        var selectedPage = Assert.IsAssignableFrom<ContentPage>(window.CurrentPage);

        Assert.Equal(2, tabbedPage.Children.Count);
        Assert.Same(tabbedPage.Children[1], tabbedPage.CurrentPage);
        Assert.Equal("Reports", ViewModelLocator.GetNavigationName(selectedPage));
        Assert.Equal("ReportsRegion", RegionManager.GetRegionName(selectedPage));
        Assert.Same(selectedPage, selectedPage.GetContainerProvider().Resolve<IPageAccessor>().Page);
        Assert.Equal(2, mauiApp.Services.GetRequiredService<IRegionManager>().Regions.Count());
    }

    [Fact]
    public void SelectedTab_OrdinaryContentPageAlias_DoesNotSelectRegionPage()
    {
        var mauiApp = CreateBuilder(prism => prism
            .RegisterTypes(container => container
                .RegisterRegionPage("RegionPage", "MainRegion")
                .RegisterForNavigation<ContentPage>("PlainPage"))
            .CreateWindow("TabbedPage?createTab=RegionPage&createTab=PlainPage&selectedTab=PlainPage"))
            .Build();
        var window = GetWindow(mauiApp);
        var tabbedPage = Assert.IsType<TabbedPage>(window.Page);
        var selectedPage = Assert.IsAssignableFrom<ContentPage>(window.CurrentPage);

        Assert.Same(tabbedPage.Children[1], tabbedPage.CurrentPage);
        Assert.Equal("PlainPage", ViewModelLocator.GetNavigationName(selectedPage));
        Assert.Null(RegionManager.GetRegionName(selectedPage));
        Assert.Null(RegionManager.GetObservableRegion(selectedPage).Value);
        Assert.Single(mauiApp.Services.GetRequiredService<IRegionManager>().Regions);
    }

    [Fact]
    public async Task AttachedRegionName_WaitsForHostingPagesScope_WhenParentAlreadyHasScope()
    {
        var mauiApp = CreateBuilder(prism => prism
            .RegisterTypes(container => container.RegisterForRegionNavigation<TrackingRegionView, TrackingRegionViewModel>())
            .CreateWindow("NavigationPage/MockViewA"))
            .Build();
        var navigationPage = Assert.IsAssignableFrom<NavigationPage>(GetWindow(mauiApp).Page);
        var page = new ContentPage();
        await navigationPage.Navigation.PushAsync(page);
        var parentContainer = navigationPage.GetContainerProvider();
        var regionManager = mauiApp.Services.GetRequiredService<IRegionManager>();

        // A XAML-set region name can arrive before Prism assigns this page its scope.
        RegionManager.SetRegionName(page, "AttachedRegion");

        Assert.Same(navigationPage, page.Parent);
        Assert.Null(page.GetContainerProvider());
        Assert.Null(RegionManager.GetObservableRegion(page).Value);
        Assert.Empty(regionManager.Regions);
        Assert.Same(navigationPage, parentContainer.Resolve<IPageAccessor>().Page);

        var pageContainer = parentContainer.CreateScope();
        page.SetContainerProvider(pageContainer);

        var region = Assert.IsType<SingleActiveRegion>(Assert.Single(regionManager.Regions));
        Assert.Equal("AttachedRegion", region.Name);
        Assert.Same(page, region.TargetElement);
        Assert.Same(pageContainer, ((ITargetAwareRegion)region).Container);
        Assert.Same(page, pageContainer.Resolve<IPageAccessor>().Page);
        Assert.Same(navigationPage, parentContainer.Resolve<IPageAccessor>().Page);
        Assert.NotNull(page.GetChildRegions());
        Assert.Null(navigationPage.GetChildRegions());

        region.Add(nameof(TrackingRegionView));
        var view = Assert.IsType<TrackingRegionView>(page.Content);
        var viewModel = Assert.IsType<TrackingRegionViewModel>(view.BindingContext);
        Assert.Same(page, viewModel.PageAccessor.Page);
    }

    [Theory]
    [InlineData("RegionPage")]
    [InlineData("NavigationPage/RegionPage")]
    [InlineData("MockHome/RegionPage")]
    [InlineData("MockHome/NavigationPage/RegionPage")]
    public void RegionAndGuest_UseHostingPagesScope(string uri)
    {
        var mauiApp = CreateRegionApp(uri);
        var window = GetWindow(mauiApp);
        var page = Assert.IsAssignableFrom<ContentPage>(window.CurrentPage);
        var regionManager = mauiApp.Services.GetRequiredService<IRegionManager>();
        var region = Assert.IsType<SingleActiveRegion>(Assert.Single(regionManager.Regions));
        var container = page.GetContainerProvider();
        var pageAccessor = container.Resolve<IPageAccessor>();

        Assert.Equal("MainRegion", region.Name);
        Assert.Same(page, pageAccessor.Page);
        Assert.Same(page, region.TargetElement);
        Assert.Same(container, ((ITargetAwareRegion)region).Container);
        Assert.Same(region, RegionManager.GetObservableRegion(page).Value);

        NavigationResult result = null;
        regionManager.RequestNavigate("MainRegion", nameof(TrackingRegionView), navigationResult => result = navigationResult);

        Assert.NotNull(result);
        Assert.True(result.Success, result.Exception?.ToString());
        var view = Assert.IsType<TrackingRegionView>(page.Content);
        var viewModel = Assert.IsType<TrackingRegionViewModel>(view.BindingContext);
        Assert.Same(container, view.GetContainerProvider());
        Assert.Same(pageAccessor, viewModel.PageAccessor);
        Assert.Same(page, viewModel.PageAccessor.Page);
        Assert.Same(PageNavigation.GetNavigationService(page), viewModel.NavigationService);
        Assert.Same(view, Assert.Single(page.GetChildRegions()));

        if (window.Page != page)
            AssertHasSeparatePageScope(window.Page, page);
        if (window.Page is FlyoutPage { Detail: NavigationPage navigationPage })
            AssertHasSeparatePageScope(navigationPage, page);
    }

    [Fact]
    public void RegionPage_AutoPopulatesRegisteredView()
    {
        var mauiApp = CreateBuilder(prism => prism
            .RegisterTypes(container => container
                .RegisterRegionPage("RegionPage", "MainRegion")
                .RegisterForRegionNavigation<TrackingRegionView, TrackingRegionViewModel>())
            .OnInitialized(container => container.Resolve<IRegionManager>()
                .RegisterViewWithRegion("MainRegion", nameof(TrackingRegionView)))
            .CreateWindow("RegionPage"))
            .Build();
        var window = GetWindow(mauiApp);
        var page = Assert.IsAssignableFrom<ContentPage>(window.Page);
        var view = Assert.IsType<TrackingRegionView>(page.Content);
        var viewModel = Assert.IsType<TrackingRegionViewModel>(view.BindingContext);

        Assert.Same(page, viewModel.PageAccessor.Page);
        Assert.Same(view, Assert.Single(RegionManager.GetObservableRegion(page).Value.ActiveViews));
    }

    [Fact]
    public void OrdinaryContentPageRegistration_DoesNotAcquireRegion()
    {
        var mauiApp = CreateBuilder(prism => prism
            .RegisterTypes(container => container
                .RegisterRegionPage("RegionPage", "MainRegion")
                .RegisterForNavigation<ContentPage>("PlainPage"))
            .CreateWindow("NavigationPage/RegionPage/PlainPage"))
            .Build();
        var window = GetWindow(mauiApp);
        var navigationPage = Assert.IsAssignableFrom<NavigationPage>(window.Page);
        var page = Assert.IsAssignableFrom<ContentPage>(navigationPage.CurrentPage);

        Assert.Equal("PlainPage", ViewModelLocator.GetNavigationName(page));
        Assert.Null(RegionManager.GetRegionName(page));
        Assert.Null(RegionManager.GetObservableRegion(page).Value);
        Assert.Null(page.GetChildRegions());
        Assert.Single(mauiApp.Services.GetRequiredService<IRegionManager>().Regions);
    }

    [Fact]
    public void Region_ActivatesFirstView_AndDisplaysOnlyActiveView()
    {
        var mauiApp = CreateRegionApp("RegionPage");
        var page = Assert.IsAssignableFrom<ContentPage>(GetWindow(mauiApp).Page);
        var region = Assert.IsType<SingleActiveRegion>(RegionManager.GetObservableRegion(page).Value);
        var firstView = new ContentView();
        var secondView = new ContentView();

        Assert.Null(page.Content);
        region.Add(firstView);

        Assert.Same(firstView, page.Content);
        Assert.Same(firstView, Assert.Single(region.ActiveViews));

        region.Add(secondView);

        Assert.Equal(2, region.Views.Count());
        Assert.Same(firstView, page.Content);
        Assert.Same(firstView, Assert.Single(region.ActiveViews));

        region.Activate(secondView);

        Assert.Same(secondView, page.Content);
        Assert.Same(secondView, Assert.Single(region.ActiveViews));
        Assert.Contains(firstView, region.Views);

        region.Deactivate(secondView);

        Assert.Null(page.Content);
        Assert.Empty(region.ActiveViews);

        region.Activate(firstView);
        region.Remove(firstView);

        Assert.Null(page.Content);
        Assert.Empty(region.ActiveViews);
        Assert.Same(secondView, Assert.Single(region.Views));

        var thirdView = new ContentView();
        region.Add(thirdView);

        Assert.Same(thirdView, page.Content);
        Assert.Same(thirdView, Assert.Single(region.ActiveViews));
    }

    [Fact]
    public void Adapter_RejectsNullRegion()
    {
        var adapter = new TestableContentPageRegionAdapter();

        var exception = Assert.Throws<ArgumentNullException>(() => adapter.AdaptRegion(null, new ContentPage()));

        Assert.Equal("region", exception.ParamName);
    }

    [Fact]
    public void Adapter_RejectsNullTarget()
    {
        var mauiApp = CreateRegionApp("RegionPage");
        var page = GetWindow(mauiApp).Page;
        var region = RegionManager.GetObservableRegion(page).Value;
        var adapter = new TestableContentPageRegionAdapter();

        var exception = Assert.Throws<ArgumentNullException>(() => adapter.AdaptRegion(region, null));

        Assert.Equal("regionTarget", exception.ParamName);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Adapter_RejectsExistingContent_WithoutReplacingIt(bool useBinding)
    {
        var mauiApp = CreateRegionApp("RegionPage");
        var region = RegionManager.GetObservableRegion(GetWindow(mauiApp).Page).Value;
        var content = new ContentView();
        var page = new ContentPage();
        if (useBinding)
            page.SetBinding(ContentPage.ContentProperty, new Binding(nameof(ContentSource.Content), source: new ContentSource { Content = content }));
        else
            page.Content = content;

        Assert.Same(content, page.Content);

        var adapter = new TestableContentPageRegionAdapter();
        Assert.Throws<InvalidOperationException>(() => adapter.AdaptRegion(region, page));

        Assert.Same(content, page.Content);
    }

    [Fact]
    public void Adapter_RejectsContentBinding_EvenWhenCurrentValueIsNull()
    {
        var mauiApp = CreateRegionApp("RegionPage");
        var region = RegionManager.GetObservableRegion(GetWindow(mauiApp).Page).Value;
        var page = new ContentPage();
        page.SetBinding(ContentPage.ContentProperty, new Binding(nameof(ContentSource.Content), source: new ContentSource()));

        Assert.Null(page.Content);

        var adapter = new TestableContentPageRegionAdapter();
        Assert.Throws<InvalidOperationException>(() => adapter.AdaptRegion(region, page));
    }

    [Theory]
    [InlineData("", "RegionPage")]
    [InlineData("NavigationPage/", "NavigationPage/RegionPage")]
    public async Task FlyoutDetailNavigation_BetweenAliases_ReplacesTheRegion(string prefix, string initialDetail)
    {
        var mauiApp = CreateBuilder(prism => prism
            .RegisterTypes(container => container
                .RegisterRegionPage("RegionPage", "MainRegion")
                .RegisterRegionPage("OtherRegionPage", "OtherRegion"))
            .CreateWindow($"MockHome/{initialDetail}"))
            .Build();
        var window = GetWindow(mauiApp);
        var flyout = Assert.IsType<MockHome>(window.Page);
        var firstPage = Assert.IsAssignableFrom<ContentPage>(window.CurrentPage);
        var firstRegion = RegionManager.GetObservableRegion(firstPage).Value;
        var regionManager = mauiApp.Services.GetRequiredService<IRegionManager>();

        var result = await PageNavigation.GetNavigationService(flyout).NavigateAsync($"{prefix}OtherRegionPage");

        Assert.True(result.Success, result.Exception?.ToString());
        var secondPage = Assert.IsAssignableFrom<ContentPage>(window.CurrentPage);
        var secondRegion = Assert.Single(regionManager.Regions);
        Assert.NotSame(firstPage, secondPage);
        Assert.NotSame(firstRegion, secondRegion);
        Assert.Equal("OtherRegionPage", ViewModelLocator.GetNavigationName(secondPage));
        Assert.Equal("OtherRegion", RegionManager.GetRegionName(secondPage));
        Assert.Equal("OtherRegion", secondRegion.Name);
        Assert.Null(firstPage.GetChildRegions());
        Assert.Null(firstRegion.RegionManager);
        Assert.Same(secondPage, Assert.IsAssignableFrom<ITargetAwareRegion>(secondRegion).TargetElement);
    }

    [Theory]
    [InlineData("RegionPage")]
    [InlineData("NavigationPage/RegionPage")]
    [InlineData("MockHome/RegionPage")]
    [InlineData("MockHome/NavigationPage/RegionPage")]
    public async Task AbsoluteNavigation_DestroysRegionAndItsActiveViewAndViewModel(string uri)
    {
        var mauiApp = CreateRegionApp(uri);
        var window = GetWindow(mauiApp);
        var page = Assert.IsAssignableFrom<ContentPage>(window.CurrentPage);
        var regionManager = mauiApp.Services.GetRequiredService<IRegionManager>();
        var region = Assert.Single(regionManager.Regions);
        region.Add(nameof(TrackingRegionView));
        var view = Assert.IsType<TrackingRegionView>(page.Content);
        var viewModel = Assert.IsType<TrackingRegionViewModel>(view.BindingContext);

        var result = await PageNavigation.GetNavigationService(page).NavigateAsync("/MockViewA");

        Assert.True(result.Success, result.Exception?.ToString());
        Assert.IsType<MockViewA>(window.Page);
        Assert.Empty(regionManager.Regions);
        Assert.Null(region.RegionManager);
        Assert.Null(page.GetChildRegions());
        Assert.Equal(1, view.DestroyCount);
        Assert.Equal(1, viewModel.DestroyCount);
    }

    [Fact]
    public async Task RegionPage_CanBeRecreatedAfterGoBack_WithFreshPageRegionAndScope()
    {
        var mauiApp = CreateRegionApp("NavigationPage/MockViewA");
        var window = GetWindow(mauiApp);
        var navigationPage = Assert.IsAssignableFrom<NavigationPage>(window.Page);
        var navigationService = PageNavigation.GetNavigationService(navigationPage.RootPage);
        var regionManager = mauiApp.Services.GetRequiredService<IRegionManager>();
        ContentPage previousPage = null;
        IRegion previousRegion = null;
        IContainerProvider previousContainer = null;

        for (var creation = 0; creation < 3; creation++)
        {
            var result = await navigationService.NavigateAsync("RegionPage");

            Assert.True(result.Success, result.Exception?.ToString());
            var page = Assert.IsAssignableFrom<ContentPage>(navigationPage.CurrentPage);
            var region = Assert.Single(regionManager.Regions);
            var container = page.GetContainerProvider();
            Assert.Equal("MainRegion", region.Name);
            Assert.NotSame(previousPage, page);
            Assert.NotSame(previousRegion, region);
            Assert.NotSame(previousContainer, container);
            Assert.Same(page, container.Resolve<IPageAccessor>().Page);
            region.Add(nameof(TrackingRegionView));
            var view = Assert.IsType<TrackingRegionView>(page.Content);
            var viewModel = Assert.IsType<TrackingRegionViewModel>(view.BindingContext);
            Assert.Same(page, viewModel.PageAccessor.Page);

            var goBackResult = await PageNavigation.GetNavigationService(page).GoBackAsync();

            Assert.True(goBackResult.Success, goBackResult.Exception?.ToString());
            Assert.IsType<MockViewA>(navigationPage.CurrentPage);
            Assert.Empty(regionManager.Regions);
            Assert.Null(page.GetChildRegions());
            Assert.Equal(1, view.DestroyCount);
            Assert.Equal(1, viewModel.DestroyCount);
            previousPage = page;
            previousRegion = region;
            previousContainer = container;
        }
    }

    private MauiApp CreateRegionApp(string uri) =>
        CreateBuilder(prism => prism
            .RegisterTypes(container => container
                .RegisterRegionPage("RegionPage", "MainRegion")
                .RegisterForRegionNavigation<TrackingRegionView, TrackingRegionViewModel>())
            .CreateWindow(uri))
            .Build();

    private static void AssertHasSeparatePageScope(Page ancestor, Page regionPage)
    {
        var container = ancestor.GetContainerProvider();
        Assert.NotSame(regionPage.GetContainerProvider(), container);
        Assert.Same(ancestor, container.Resolve<IPageAccessor>().Page);
    }

    private sealed class TestableContentPageRegionAdapter : ContentPageRegionAdapter
    {
        public TestableContentPageRegionAdapter()
            : base(null)
        {
        }

        public void AdaptRegion(IRegion region, ContentPage page) => Adapt(region, page);
    }

    public sealed class CustomContentPageRegionAdapter : ContentPageRegionAdapter
    {
        public CustomContentPageRegionAdapter(IRegionBehaviorFactory regionBehaviorFactory)
            : base(regionBehaviorFactory)
        {
        }

        public ContentPage AdaptedPage { get; private set; }

        protected override void Adapt(IRegion region, ContentPage regionTarget)
        {
            AdaptedPage = regionTarget;
            base.Adapt(region, regionTarget);
        }
    }

    public sealed class ContentSource
    {
        public View Content { get; set; }
    }

    public sealed class TrackingRegionView : ContentView, IDestructible
    {
        public int DestroyCount { get; private set; }

        public void Destroy() => DestroyCount++;
    }

    public sealed class TrackingRegionViewModel : IDestructible
    {
        public TrackingRegionViewModel(IPageAccessor pageAccessor, INavigationService navigationService)
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
