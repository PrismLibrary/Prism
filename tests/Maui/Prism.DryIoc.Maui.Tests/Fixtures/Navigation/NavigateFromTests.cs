using Prism.Common;
using Prism.DryIoc.Maui.Tests.Mocks.Views;
using Prism.DryIoc.Maui.Tests.Mocks.ViewModels;
using Prism.DryIoc.Maui.Tests.Mocks.Navigation;
using Prism.Events;
using Prism.Mvvm;
using Prism.Navigation.Xaml;
using NavigationMode = Prism.Navigation.NavigationMode;
using TabbedPage = Microsoft.Maui.Controls.TabbedPage;

namespace Prism.DryIoc.Maui.Tests.Fixtures.Navigation;

public class NavigateFromTests : TestBase
{
    public NavigateFromTests(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
    }

    [Theory]
    [InlineData("NavigationPage/Source/Middle/Current")]
    [InlineData("MockHome/NavigationPage/Source/Middle/Current")]
    [InlineData("TabbedPage?createTab=NavigationPage%2FSource%2FMiddle%2FCurrent&createTab=MockViewA")]
    public async Task NavigateFrom_RetainsSourceAndScope_AndDestroysOnlyRemovedPages(string route)
    {
        var window = CreateWindow(route);
        var current = Assert.IsType<MockNavigateFromPage>(window.CurrentPage);
        var navigation = Assert.IsAssignableFrom<NavigationPage>(current.Parent);
        var source = Assert.IsType<MockNavigateFromPage>(navigation.RootPage);
        var middle = Assert.IsType<MockNavigateFromPage>(navigation.Navigation.NavigationStack[1]);
        var sourceScope = source.GetContainerProvider();
        var sourceInitialized = source.Initialized;
        source.AllowNavigation = false;
        var sourceConfirmations = source.Confirmations;
        var sourceFrom = source.NavigatedFromCount;
        var sourceTo = source.NavigatedToCount;
        var currentFrom = current.NavigatedFromCount;
        var currentConfirmations = current.Confirmations;
        var value = new object();
        var service = current.GetContainerProvider().Resolve<INavigationService>();
        var events = new List<NavigationRequestContext>();
        current.GetContainerProvider().Resolve<IEventAggregator>().GetEvent<NavigationRequestEvent>().Subscribe(events.Add);

        var result = await service.NavigateFromAsync("Source", "Destination?query=value", new NavigationParameters { { "object", value } });

        Assert.True(result.Success, result.Exception?.ToString());
        Assert.Collection(navigation.Navigation.NavigationStack,
            page => Assert.Same(source, page),
            page => Assert.Equal("Destination", ViewModelLocator.GetNavigationName(page)));
        var destination = Assert.IsType<MockNavigateFromPage>(navigation.CurrentPage);
        Assert.Same(sourceScope, source.GetContainerProvider());
        Assert.NotSame(sourceScope, destination.GetContainerProvider());
        Assert.Same(destination, destination.GetContainerProvider().Resolve<IPageAccessor>().Page);
        Assert.Equal(sourceInitialized, source.Initialized);
        Assert.Equal(sourceConfirmations, source.Confirmations);
        Assert.Equal(sourceFrom, source.NavigatedFromCount);
        Assert.Equal(sourceTo, source.NavigatedToCount);
        Assert.Equal(0, source.Destroyed);
        Assert.Equal(currentConfirmations + 1, current.Confirmations);
        Assert.Equal(currentFrom + 1, current.NavigatedFromCount);
        Assert.Equal(1, current.Destroyed);
        Assert.Equal(1, middle.Destroyed);
        Assert.Equal(1, destination.Initialized);
        Assert.Equal(1, destination.NavigatedToCount);
        Assert.Equal("value", destination.ReceivedParameters.GetValue<string>("query"));
        Assert.Same(value, destination.ReceivedParameters.GetValue<object>("object"));
        Assert.Equal(NavigationMode.New, destination.ReceivedParameters.GetNavigationMode());
        Assert.True(Assert.Single(events).Result.Success);
    }

    [Fact]
    public async Task NavigateFrom_DuplicateNames_UsesNearestSource()
    {
        var window = CreateWindow("NavigationPage/Source/Middle/Source/Current");
        var navigation = Assert.IsAssignableFrom<NavigationPage>(window.Page);
        var original = navigation.Navigation.NavigationStack.ToArray();
        var service = GetService(window.CurrentPage);

        var result = await service.NavigateFromAsync("Source", "Destination");

        Assert.True(result.Success, result.Exception?.ToString());
        Assert.Equal(4, navigation.Navigation.NavigationStack.Count);
        Assert.Equal(original.Take(3), navigation.Navigation.NavigationStack.Take(3));
        Assert.Equal("Destination", ViewModelLocator.GetNavigationName(navigation.CurrentPage));
    }

    [Fact]
    public async Task NavigateFrom_RegistrationAliases_DoNotMatchOnlyByClrType()
    {
        var window = CreateWindow("NavigationPage/Source/Middle/Current");
        var navigation = Assert.IsAssignableFrom<NavigationPage>(window.Page);
        var middle = navigation.Navigation.NavigationStack[1];

        var result = await GetService(window.CurrentPage).NavigateFromAsync("Middle", new Uri("Destination", UriKind.Relative));

        Assert.True(result.Success, result.Exception?.ToString());
        Assert.Equal(3, navigation.Navigation.NavigationStack.Count);
        Assert.Same(middle, navigation.Navigation.NavigationStack[1]);
        Assert.Equal("Destination", ViewModelLocator.GetNavigationName(navigation.CurrentPage));
    }

    [Theory]
    [InlineData("Missing", "Destination")]
    [InlineData("Source", "/Destination")]
    [InlineData("Source", "https://example.com/Destination")]
    [InlineData("Source", "UnregisteredDestination")]
    [InlineData("Source", "TabbedPage?createTab=NavigationPage|UnregisteredDestination")]
    [InlineData("Source", "../Destination?useModalNavigation=true")]
    [InlineData("", "Destination")]
    [InlineData(null, "Destination")]
    [InlineData("Source", "")]
    [InlineData("Source", null)]
    public async Task NavigateFrom_InvalidRequest_DoesNotMutateOrLockNavigation(string sourceName, string route)
    {
        var window = CreateWindow("NavigationPage/Source/Current");
        var navigation = Assert.IsAssignableFrom<NavigationPage>(window.Page);
        var pages = navigation.Navigation.NavigationStack.ToArray();
        var current = Assert.IsType<MockNavigateFromPage>(window.CurrentPage);
        var confirmations = current.Confirmations;
        var service = GetService(current);

        var result = await service.NavigateFromAsync(sourceName, route);

        Assert.False(result.Success);
        Assert.NotNull(result.Exception);
        Assert.Equal(pages, navigation.Navigation.NavigationStack);
        Assert.Equal(confirmations, current.Confirmations);
        Assert.Equal(0, current.Destroyed);
        Assert.Equal(PageNavigationSource.Device, PageNavigationService.NavigationSource);
        Assert.True((await service.NavigateFromAsync("Source", "Destination")).Success);
    }

    [Fact]
    public async Task NavigateFrom_ConfirmationVeto_PreservesBothStacks()
    {
        var window = CreateWindow("NavigationPage/Source/Middle");
        var navigation = Assert.IsAssignableFrom<NavigationPage>(window.Page);
        await GetService(window.CurrentPage).NavigateAsync("Current?useModalNavigation=true");
        var current = Assert.IsType<MockNavigateFromPage>(window.CurrentPage);
        current.AllowNavigation = false;
        var pages = navigation.Navigation.NavigationStack.ToArray();
        var from = current.NavigatedFromCount;

        var result = await GetService(current).NavigateFromAsync("Source", "Destination");

        Assert.False(result.Success);
        Assert.Equal(NavigationException.IConfirmNavigationReturnedFalse, result.Exception.Message);
        Assert.Equal(pages, navigation.Navigation.NavigationStack);
        Assert.Same(current, Assert.Single(window.Navigation.ModalStack));
        Assert.Equal(from, current.NavigatedFromCount);
        Assert.Equal(0, current.Destroyed);
    }

    [Fact]
    public async Task NavigateFrom_PendingConfirmation_DoesNotMutateEitherStack()
    {
        var window = CreateWindow("NavigationPage/Source");
        await GetService(window.CurrentPage).NavigateAsync("Current?useModalNavigation=true");
        var current = Assert.IsType<MockNavigateFromPage>(window.CurrentPage);
        current.Decision = new(TaskCreationOptions.RunContinuationsAsynchronously);
        var request = GetService(current).NavigateFromAsync("Source", "Destination");
        await current.ConfirmationStarted.Task.WaitAsync(TimeSpan.FromSeconds(5));

        Assert.False(request.IsCompleted);
        Assert.Same(current, Assert.Single(window.Navigation.ModalStack));
        Assert.Same(current, window.PendingModalConfirmation);
        Assert.Equal(0, current.Destroyed);
        await window.Navigation.PopModalAsync();
        Assert.Same(current, Assert.Single(window.Navigation.ModalStack));
        Assert.Equal(1, current.Confirmations);
        Assert.Equal(0, current.Destroyed);
        current.Decision.SetResult(true);

        Assert.True((await request.WaitAsync(TimeSpan.FromSeconds(5))).Success);
        Assert.Null(window.PendingModalConfirmation);
        Assert.Empty(window.Navigation.ModalStack);
        Assert.Equal(1, current.Destroyed);
    }

    [Theory]
    [InlineData("Current?useModalNavigation=true")]
    [InlineData("NavigationPage?useModalNavigation=true/Current")]
    [InlineData("NavigationPage?useModalNavigation=true/Middle/Current")]
    public async Task NavigateFrom_CrossesModalBoundary_AndResumesSourceStack(string modalRoute)
    {
        var window = CreateWindow("NavigationPage/Source/Middle");
        var navigation = Assert.IsAssignableFrom<NavigationPage>(window.Page);
        var source = navigation.RootPage;
        Assert.True((await GetService(window.CurrentPage).NavigateAsync(modalRoute)).Success);
        var modalPages = PageNavigationHistory.GetPages(Assert.Single(window.Navigation.ModalStack)).OfType<MockNavigateFromPage>().ToArray();
        var current = Assert.IsType<MockNavigateFromPage>(window.CurrentPage);
        var from = current.NavigatedFromCount;

        var result = await GetService(current).NavigateFromAsync("Source", "Destination");

        Assert.True(result.Success, result.Exception?.ToString());
        Assert.Empty(window.Navigation.ModalStack);
        Assert.Collection(navigation.Navigation.NavigationStack,
            page => Assert.Same(source, page),
            page => Assert.Equal("Destination", ViewModelLocator.GetNavigationName(page)));
        Assert.Equal(from + 1, current.NavigatedFromCount);
        Assert.All(modalPages, page => Assert.Equal(1, page.Destroyed));
    }

    [Fact]
    public async Task NavigateFrom_PlainModalSource_PreservesSourceAndPushesModally()
    {
        var window = CreateWindow("MockViewA");
        Assert.True((await GetService(window.CurrentPage).NavigateAsync("Source/Current")).Success);
        var source = window.Navigation.ModalStack[0];

        var result = await GetService(window.CurrentPage).NavigateFromAsync("Source", "Destination");

        Assert.True(result.Success, result.Exception?.ToString());
        Assert.Collection(window.Navigation.ModalStack,
            page => Assert.Same(source, page),
            page => Assert.Equal("Destination", ViewModelLocator.GetNavigationName(page)));
    }

    [Fact]
    public async Task NavigateFrom_CurrentSource_DoesNotDestroyOrReinitializeIt()
    {
        var window = CreateWindow("NavigationPage/Source");
        var source = Assert.IsType<MockNavigateFromPage>(window.CurrentPage);
        var from = source.NavigatedFromCount;

        var result = await GetService(source).NavigateFromAsync("Source", "Destination");

        Assert.True(result.Success, result.Exception?.ToString());
        Assert.Equal(1, source.Initialized);
        Assert.Equal(from + 1, source.NavigatedFromCount);
        Assert.Equal(0, source.Destroyed);
    }

    [Fact]
    public async Task NavigateFrom_NavigationContainer_UsesOrdinaryStackReplacement()
    {
        var window = CreateWindow("NavigationPage/Source/Current");
        var navigation = Assert.IsAssignableFrom<NavigationPage>(window.Page);
        var oldPages = navigation.Navigation.NavigationStack.Cast<MockNavigateFromPage>().ToArray();

        var result = await GetService(window.CurrentPage).NavigateFromAsync("NavigationPage", "MockViewA");

        Assert.True(result.Success, result.Exception?.ToString());
        Assert.Same(navigation, window.Page);
        Assert.IsType<MockViewA>(Assert.Single(navigation.Navigation.NavigationStack));
        Assert.All(oldPages, page => Assert.Equal(1, page.Destroyed));
    }

    [Fact]
    public async Task NavigateFrom_FlyoutContainer_UsesOrdinaryDetailReplacement()
    {
        var window = CreateWindow("MockHome/NavigationPage/Source/Current");
        var flyout = Assert.IsAssignableFrom<FlyoutPage>(window.Page);
        var original = PageNavigationHistory.GetPages(flyout.Detail).OfType<MockNavigateFromPage>().ToArray();

        var result = await GetService(window.CurrentPage).NavigateFromAsync("MockHome", "Destination");

        Assert.True(result.Success, result.Exception?.ToString());
        Assert.Same(flyout, window.Page);
        Assert.Equal("Destination", ViewModelLocator.GetNavigationName(flyout.Detail));
        Assert.All(original, page => Assert.Equal(1, page.Destroyed));
    }

    [Fact]
    public async Task NavigateFrom_TabbedContainer_DoesNotImplicitlySelectOrCreateATab()
    {
        var window = CreateWindow("TabbedPage?createTab=Source&createTab=Current");
        var tabbed = Assert.IsAssignableFrom<TabbedPage>(window.Page);
        var selected = tabbed.CurrentPage;
        var tabs = tabbed.Children.ToArray();

        var result = await GetService(selected).NavigateFromAsync("TabbedPage", "Destination");

        Assert.True(result.Success, result.Exception?.ToString());
        Assert.Equal(tabs, tabbed.Children);
        Assert.Same(selected, tabbed.CurrentPage);
        Assert.Equal("Destination", ViewModelLocator.GetNavigationName(Assert.Single(window.Navigation.ModalStack)));
    }

    [Fact]
    public async Task NavigateFrom_DoesNotSearchInactiveTabs()
    {
        var window = CreateWindow("TabbedPage?createTab=Source&createTab=Current");
        var tabbed = Assert.IsAssignableFrom<TabbedPage>(window.Page);
        var selected = tabbed.CurrentPage;

        var result = await GetService(selected).NavigateFromAsync("Current", "Destination");

        Assert.False(result.Success);
        Assert.Equal(NavigationException.NavigationSourceNotFound, result.Exception.Message);
        Assert.Same(selected, tabbed.CurrentPage);
        Assert.Empty(window.Navigation.ModalStack);
    }

    [Fact]
    public async Task NavigateFrom_InactiveScopedCaller_DoesNotChangeActiveTab()
    {
        var window = CreateWindow("TabbedPage?createTab=Source&createTab=Current");
        var tabbed = Assert.IsAssignableFrom<TabbedPage>(window.Page);

        var result = await GetService(tabbed.Children[1]).NavigateFromAsync("Source", "Destination");

        Assert.False(result.Success);
        Assert.Same(tabbed.Children[0], tabbed.CurrentPage);
        Assert.Empty(window.Navigation.ModalStack);
    }

    [Theory]
    [InlineData("../Destination", false)]
    [InlineData("../", true)]
    public async Task NavigateFrom_RelativeBackRoute_IsResolvedFromNamedSource(string route, bool backOnly)
    {
        var window = CreateWindow("NavigationPage/Source/Middle/Current");
        var navigation = Assert.IsAssignableFrom<NavigationPage>(window.Page);
        var source = Assert.IsType<MockNavigateFromPage>(navigation.RootPage);
        var middle = Assert.IsType<MockNavigateFromPage>(navigation.Navigation.NavigationStack[1]);
        var current = Assert.IsType<MockNavigateFromPage>(window.CurrentPage);
        var sourceTo = source.NavigatedToCount;
        var currentFrom = current.NavigatedFromCount;
        var result = await GetService(current).NavigateFromAsync("Middle", route);

        Assert.True(result.Success, result.Exception?.ToString());
        Assert.Same(source, navigation.RootPage);
        Assert.Equal(1, middle.Destroyed);
        Assert.Equal(1, current.Destroyed);
        Assert.Equal(currentFrom + 1, current.NavigatedFromCount);
        if (backOnly)
        {
            Assert.Same(source, Assert.Single(navigation.Navigation.NavigationStack));
            Assert.Equal(sourceTo + 1, source.NavigatedToCount);
            Assert.Equal(NavigationMode.Back, source.ReceivedParameters.GetNavigationMode());
        }
        else
        {
            Assert.Equal(2, navigation.Navigation.NavigationStack.Count);
            Assert.Equal("Destination", ViewModelLocator.GetNavigationName(navigation.CurrentPage));
            Assert.Equal(sourceTo, source.NavigatedToCount);
        }
    }

    [Theory]
    [InlineData("../Destination")]
    [InlineData("../")]
    public async Task NavigateFrom_RelativeBackRoute_CrossesModalSourceRoot(string route)
    {
        var window = CreateWindow("NavigationPage/Source");
        var navigation = Assert.IsAssignableFrom<NavigationPage>(window.Page);
        var source = navigation.RootPage;
        Assert.True((await GetService(source).NavigateAsync("NavigationPage?useModalNavigation=true/Middle/Current")).Success);

        var result = await GetService(window.CurrentPage).NavigateFromAsync("Middle", route);

        Assert.True(result.Success, result.Exception?.ToString());
        Assert.Empty(window.Navigation.ModalStack);
        Assert.Same(source, navigation.RootPage);
        Assert.Equal(route == "../" ? 1 : 2, navigation.Navigation.NavigationStack.Count);
    }

    [Theory]
    [InlineData("../")]
    [InlineData("../../Destination")]
    public async Task NavigateFrom_RelativeBackBeyondRoot_FailsWithoutMutation(string route)
    {
        var window = CreateWindow("NavigationPage/Source/Current");
        var navigation = Assert.IsAssignableFrom<NavigationPage>(window.Page);
        var pages = navigation.Navigation.NavigationStack.ToArray();

        var result = await GetService(window.CurrentPage).NavigateFromAsync("Source", route);

        Assert.False(result.Success);
        Assert.Equal(pages, navigation.Navigation.NavigationStack);
        Assert.All(pages.Cast<MockNavigateFromPage>(), page => Assert.Equal(0, page.Destroyed));
    }

    [Fact]
    public async Task NavigateFrom_RelativeForwardRoute_CanReplaceNavigationRoot()
    {
        var window = CreateWindow("NavigationPage/Source/Current");
        var navigation = Assert.IsAssignableFrom<NavigationPage>(window.Page);
        var source = Assert.IsType<MockNavigateFromPage>(navigation.RootPage);

        var result = await GetService(window.CurrentPage).NavigateFromAsync("Source", "../Destination");

        Assert.True(result.Success, result.Exception?.ToString());
        Assert.Equal("Destination", ViewModelLocator.GetNavigationName(Assert.Single(navigation.Navigation.NavigationStack)));
        Assert.Equal(1, source.Destroyed);
    }

    [Fact]
    public async Task NavigateFrom_UsesCallingPagesWindow()
    {
        var firstWindow = CreateWindow("NavigationPage/Source/Current");
        var navigation = Assert.IsAssignableFrom<NavigationPage>(firstWindow.Page);
        var current = navigation.CurrentPage;
        var service = GetService(current);
        var otherRoot = new ContentPage();
        firstWindow.Page = otherRoot;
        var secondWindow = new PrismWindow("second") { Page = navigation };
        Assert.Same(secondWindow, current.GetParentWindow());

        var result = await service.NavigateFromAsync("Source", "Destination");

        Assert.True(result.Success, result.Exception?.ToString());
        Assert.Same(otherRoot, firstWindow.Page);
        Assert.Same(navigation, secondWindow.Page);
        Assert.Equal("Destination", ViewModelLocator.GetNavigationName(navigation.CurrentPage));
        Assert.Empty(firstWindow.Navigation.ModalStack);
    }

    [Fact]
    public async Task NavigateFrom_AnimationParameter_AppliesToRemovalAndForwardNavigation()
    {
        var window = CreateWindow("NavigationPage/Source/Current");
        var service = GetService(window.CurrentPage);

        var result = await service.NavigateFromAsync("Source", "Destination?animated=false");

        Assert.True(result.Success, result.Exception?.ToString());
        Assert.False(Assert.Single(service.GetPops()).Animated);
        Assert.False(Assert.Single(service.GetPushes()).Animated);
    }

    [Fact]
    public async Task NavigateFrom_RemovesMultipleModalLayers_WithoutDestroyingDescendantsTwice()
    {
        var window = CreateWindow("NavigationPage/Source");
        Assert.True((await GetService(window.CurrentPage).NavigateAsync("NavigationPage?useModalNavigation=true/Middle")).Success);
        Assert.True((await GetService(window.CurrentPage).NavigateAsync("Current?useModalNavigation=true")).Success);
        var removed = window.Navigation.ModalStack.SelectMany(PageNavigationHistory.GetPages).OfType<MockNavigateFromPage>().ToArray();

        var result = await GetService(window.CurrentPage).NavigateFromAsync("Source", "Destination");

        Assert.True(result.Success, result.Exception?.ToString());
        Assert.Empty(window.Navigation.ModalStack);
        Assert.All(removed, page => Assert.Equal(1, page.Destroyed));
    }

    [Fact]
    public async Task NavigateFrom_MissingSource_DoesNotPopAnyModal()
    {
        var window = CreateWindow("NavigationPage/Source");
        Assert.True((await GetService(window.CurrentPage).NavigateAsync("Middle?useModalNavigation=true/Current")).Success);
        var modals = window.Navigation.ModalStack.ToArray();

        var result = await GetService(window.CurrentPage).NavigateFromAsync("Missing", "Destination");

        Assert.False(result.Success);
        Assert.Equal(modals, window.Navigation.ModalStack);
        Assert.All(modals.Cast<MockNavigateFromPage>(), page => Assert.Equal(0, page.Destroyed));
    }

    [Fact]
    public async Task NavigateFrom_PopFailure_ReturnsFailureAndReleasesNavigation()
    {
        var window = CreateWindow("NavigationPage/Source/Current");
        var navigation = Assert.IsAssignableFrom<NavigationPage>(window.Page);
        var pages = navigation.Navigation.NavigationStack.ToArray();
        var service = GetService(window.CurrentPage);
        TestPageNavigationService.ForceNextDoPopToReturnNull = true;

        var result = await service.NavigateFromAsync("Source", "Destination");

        Assert.False(result.Success);
        Assert.Equal(pages, navigation.Navigation.NavigationStack);
        Assert.All(pages.Cast<MockNavigateFromPage>(), page => Assert.Equal(0, page.Destroyed));
        Assert.Equal(PageNavigationSource.Device, PageNavigationService.NavigationSource);
        Assert.True((await service.NavigateFromAsync("Source", "Destination")).Success);
    }

    [Fact]
    public async Task NavigateFrom_TabParameters_AreValidatedBeforeRemovingPages()
    {
        var window = CreateWindow("NavigationPage/Source/Current");
        var pages = window.Page.Navigation.NavigationStack.ToArray();
        var parameters = new NavigationParameters { { KnownNavigationParameters.CreateTab, "NavigationPage/Unregistered" } };

        var result = await GetService(window.CurrentPage).NavigateFromAsync("Source", "TabbedPage", parameters);

        Assert.False(result.Success);
        Assert.Equal(pages, window.Page.Navigation.NavigationStack);
        Assert.All(pages.Cast<MockNavigateFromPage>(), page => Assert.Equal(0, page.Destroyed));
    }

    [Fact]
    public async Task NavigateFrom_TabValidation_PreservesOrdinaryContentTabSegmentHandling()
    {
        var window = CreateWindow("NavigationPage/Source/Current");

        var result = await GetService(window.CurrentPage).NavigateFromAsync("Source", "TabbedPage?createTab=Current|UnusedSegment");

        Assert.True(result.Success, result.Exception?.ToString());
        var tabbed = Assert.IsAssignableFrom<TabbedPage>(window.Page.Navigation.NavigationStack.Last());
        Assert.Equal("Current", ViewModelLocator.GetNavigationName(Assert.Single(tabbed.Children)));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task NavigateFrom_InitializationFailure_RestoresStacksScopesAndLifecycles(bool modal)
    {
        var window = CreateWindow("NavigationPage/Source/Middle/Current");
        if (modal)
            Assert.True((await GetService(window.CurrentPage).NavigateAsync("Destination?useModalNavigation=true")).Success);
        var pages = window.Page.Navigation.NavigationStack.ToArray();
        var modals = window.Navigation.ModalStack.ToArray();
        var current = Assert.IsType<MockNavigateFromPage>(window.CurrentPage);
        var scope = current.GetContainerProvider();
        var from = current.NavigatedFromCount;
        var created = new List<MockNavigateFromPage>();

        var result = await GetService(current).NavigateFromAsync("Source", "Destination?failInitialize=true/Current",
            new NavigationParameters { { "createdPages", created } });

        Assert.False(result.Success);
        Assert.Equal(pages, window.Page.Navigation.NavigationStack);
        Assert.Equal(modals, window.Navigation.ModalStack);
        Assert.Same(scope, current.GetContainerProvider());
        Assert.Same(current, scope.Resolve<IPageAccessor>().Page);
        Assert.Equal(from, current.NavigatedFromCount);
        Assert.All(pages.Concat(modals).Cast<MockNavigateFromPage>(), page => Assert.Equal(0, page.Destroyed));
        Assert.Equal(2, created.Count);
        Assert.All(created, page =>
        {
            Assert.Equal(0, page.NavigatedToCount);
            Assert.Equal(0, page.NavigatedFromCount);
            Assert.Equal(1, page.Destroyed);
        });
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public async Task NavigateFrom_NativeMutationThenFailure_RestoresOriginalPages(bool modal, bool push)
    {
        var window = CreateWindow("NavigationPage/Source/Current");
        if (modal)
            Assert.True((await GetService(window.CurrentPage).NavigateAsync("Middle?useModalNavigation=true")).Success);
        var pages = window.Page.Navigation.NavigationStack.ToArray();
        var modals = window.Navigation.ModalStack.ToArray();
        var current = Assert.IsType<MockNavigateFromPage>(window.CurrentPage);
        var from = current.NavigatedFromCount;
        TestPageNavigationService.ThrowAfterNextPop = !push;
        TestPageNavigationService.ThrowAfterNextPush = push;
        try
        {
            var result = await GetService(current).NavigateFromAsync("Source", "Destination");

            Assert.False(result.Success);
            Assert.Equal(pages, window.Page.Navigation.NavigationStack);
            Assert.Equal(modals, window.Navigation.ModalStack);
            Assert.Equal(from, current.NavigatedFromCount);
            Assert.All(pages.Concat(modals).Cast<MockNavigateFromPage>(), page => Assert.Equal(0, page.Destroyed));
            Assert.True((await GetService(current).NavigateFromAsync("Source", "Destination")).Success);
        }
        finally
        {
            TestPageNavigationService.ThrowAfterNextPop = false;
            TestPageNavigationService.ThrowAfterNextPush = false;
        }
    }

    [Fact]
    public async Task NavigateFrom_StackChangesDuringConfirmation_FailsWithoutRemovingNewPage()
    {
        var window = CreateWindow("NavigationPage/Source/Current");
        var current = Assert.IsType<MockNavigateFromPage>(window.CurrentPage);
        current.Decision = new(TaskCreationOptions.RunContinuationsAsynchronously);
        var request = GetService(current).NavigateFromAsync("Source", "Destination");
        await current.ConfirmationStarted.Task.WaitAsync(TimeSpan.FromSeconds(5));
        var added = new ContentPage();
        await window.Navigation.PushModalAsync(added);
        current.Decision.SetResult(true);

        var result = await request;

        Assert.False(result.Success);
        Assert.Same(added, Assert.Single(window.Navigation.ModalStack));
        Assert.Same(current, window.Page.Navigation.NavigationStack.Last());
        Assert.Equal(0, current.Destroyed);
    }

    [Theory]
    [InlineData("NavigationPage?useModalNavigation=true/Current")]
    [InlineData("TabbedPage?useModalNavigation=true&createTab=NavigationPage|Current")]
    [InlineData("MockHome?useModalNavigation=true/NavigationPage/Current")]
    public async Task NavigateFrom_ConfirmsInitiatingContainer_AndDeduplicatesSharedLifecycle(string route)
    {
        var window = CreateWindow("NavigationPage/Source");
        Assert.True((await GetService(window.CurrentPage).NavigateAsync(route)).Success);
        var modal = Assert.Single(window.Navigation.ModalStack);
        var current = window.CurrentPage;
        var lifecycle = new MockNavigateFromLifecycle { AllowNavigation = false };
        modal.BindingContext = lifecycle;
        current.BindingContext = lifecycle;
        var service = GetService(modal);

        var veto = await service.NavigateFromAsync("Source", "Destination");

        Assert.False(veto.Success);
        Assert.Equal(1, lifecycle.Confirmations);
        Assert.Same(modal, Assert.Single(window.Navigation.ModalStack));
        Assert.Equal(0, lifecycle.Departures);
        lifecycle.AllowNavigation = true;

        Assert.True((await service.NavigateFromAsync("Source", "Destination")).Success);
        Assert.Equal(2, lifecycle.Confirmations);
        Assert.Equal(1, lifecycle.Departures);
        Assert.Empty(window.Navigation.ModalStack);
    }

    [Fact]
    public async Task NavigateFrom_PendingTabbedConfirmation_CoalescesNestedNavigationBack()
    {
        var window = CreateWindow("NavigationPage/Source");
        Assert.True((await GetService(window.CurrentPage).NavigateAsync(
            "TabbedPage?useModalNavigation=true&createTab=NavigationPage|Current")).Success);
        var modal = Assert.IsAssignableFrom<TabbedPage>(Assert.Single(window.Navigation.ModalStack));
        var navigation = Assert.IsAssignableFrom<NavigationPage>(modal.CurrentPage);
        var lifecycle = new MockNavigateFromLifecycle
        {
            Decision = new(TaskCreationOptions.RunContinuationsAsynchronously)
        };
        modal.BindingContext = lifecycle;
        var request = GetService(modal).NavigateFromAsync("Source", "Destination");
        await lifecycle.ConfirmationStarted.Task.WaitAsync(TimeSpan.FromSeconds(5));

        await MvvmHelpers.HandleNavigationPageGoBack(navigation).WaitAsync(TimeSpan.FromSeconds(1));
        Assert.Same(modal, Assert.Single(window.Navigation.ModalStack));
        lifecycle.Decision.SetResult(false);
        Assert.False((await request).Success);
        Assert.Equal(1, lifecycle.Confirmations);
        Assert.Same(modal, Assert.Single(window.Navigation.ModalStack));
    }

    [Fact]
    public async Task NavigateFrom_FlyoutDetailMutationThenFailure_RestoresOriginalDetail()
    {
        var window = CreateWindow("MockHome/NavigationPage/Source/Current");
        var flyout = Assert.IsAssignableFrom<FlyoutPage>(window.Page);
        var detail = flyout.Detail;
        var current = Assert.IsType<MockNavigateFromPage>(window.CurrentPage);
        var scope = current.GetContainerProvider();
        var from = current.NavigatedFromCount;
        var changed = false;
        flyout.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(FlyoutPage.Detail) && flyout.Detail != detail && !changed)
            {
                changed = true;
                throw new InvalidOperationException("Detail changed before platform failure.");
            }
        };

        var result = await GetService(current).NavigateFromAsync("MockHome", "Destination");

        Assert.False(result.Success);
        Assert.True(changed);
        Assert.Same(detail, flyout.Detail);
        Assert.Same(scope, current.GetContainerProvider());
        Assert.Equal(from, current.NavigatedFromCount);
        Assert.Equal(0, current.Destroyed);
    }

    [Fact]
    public async Task NavigateFrom_TabSelectionMutationThenFailure_RestoresOriginalSelection()
    {
        var window = CreateWindow("NavigationPage/TabbedPage?createTab=Source&createTab=MockViewA");
        var navigation = Assert.IsAssignableFrom<NavigationPage>(window.Page);
        var tabs = Assert.IsAssignableFrom<TabbedPage>(navigation.RootPage);
        var selected = tabs.CurrentPage;
        var changed = false;
        tabs.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(TabbedPage.CurrentPage) && tabs.CurrentPage != selected && !changed)
            {
                changed = true;
                throw new InvalidOperationException("Selection changed before platform failure.");
            }
        };

        var result = await GetService(window.CurrentPage).NavigateFromAsync("NavigationPage", "TabbedPage?selectedTab=MockViewA");

        Assert.False(result.Success);
        Assert.True(changed);
        Assert.Same(tabs, navigation.RootPage);
        Assert.Same(selected, tabs.CurrentPage);
        Assert.Equal(0, Assert.IsType<MockNavigateFromPage>(selected).Destroyed);
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public async Task NavigateFrom_ExternalNavigationDuringInitialization_IsNotOverwritten(bool fail, bool modal)
    {
        var window = CreateWindow("NavigationPage/Source/Current");
        var started = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var created = new List<MockNavigateFromPage>();
        var parameters = new NavigationParameters
        {
            { "createdPages", created },
            { "duringInitialize", (Func<Task>)(async () => { started.SetResult(true); await release.Task; }) },
            { "failInitialize", fail }
        };
        var current = Assert.IsType<MockNavigateFromPage>(window.CurrentPage);
        var request = GetService(current).NavigateFromAsync("Source", "Destination", parameters);
        await started.Task.WaitAsync(TimeSpan.FromSeconds(5));
        var unrelated = new ContentPage();
        if (modal)
            await window.Navigation.PushModalAsync(unrelated);
        else
            await window.Page.Navigation.PushAsync(unrelated);
        release.SetResult(true);

        var result = await request;

        Assert.False(result.Success);
        if (modal)
            Assert.Same(unrelated, Assert.Single(window.Navigation.ModalStack));
        else
            Assert.Same(unrelated, window.Page.Navigation.NavigationStack.Last());
        Assert.Equal(0, current.Destroyed);
        Assert.Equal(1, Assert.Single(created).Destroyed);
    }

    [Fact]
    public async Task NavigateFrom_ExternalNavigationDuringRollback_IsNotPopped()
    {
        var window = CreateWindow("NavigationPage/Source/Current");
        var unrelated = new ContentPage();
        TestPageNavigationService.ThrowAfterNextPush = true;
        TestPageNavigationService.AfterNextPush = () =>
        {
            TestPageNavigationService.AfterNextPop = () => window.Navigation.PushModalAsync(unrelated);
            return Task.CompletedTask;
        };
        try
        {
            var result = await GetService(window.CurrentPage).NavigateFromAsync("Source", "Destination?useModalNavigation=true");

            Assert.False(result.Success);
            Assert.Same(unrelated, Assert.Single(window.Navigation.ModalStack));
        }
        finally
        {
            TestPageNavigationService.ThrowAfterNextPush = false;
            TestPageNavigationService.AfterNextPop = null;
            TestPageNavigationService.AfterNextPush = null;
        }
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task NavigateFrom_CleanupFailure_DoesNotDestroyTwiceOrMaskOriginalFailure(bool conflict)
    {
        var window = CreateWindow("NavigationPage/Source/Current");
        var created = new List<MockNavigateFromPage>();
        var started = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var parameters = new NavigationParameters { { "createdPages", created } };
        if (conflict)
            parameters.Add("duringInitialize", (Func<Task>)(async () => { started.SetResult(true); await release.Task; }));
        var unrelated = new ContentPage();
        var request = GetService(window.CurrentPage).NavigateFromAsync("Source", "Destination?failInitialize=true&failDestroy=true", parameters);
        if (conflict)
        {
            await started.Task.WaitAsync(TimeSpan.FromSeconds(5));
            await window.Navigation.PushModalAsync(unrelated);
            release.SetResult(true);
        }

        var result = await request;

        Assert.False(result.Success);
        Assert.Contains("Initialization failed.", result.Exception.ToString());
        Assert.Contains("Destroy failed.", result.Exception.ToString());
        Assert.Equal(1, Assert.Single(created).Destroyed);
        if (conflict)
        {
            Assert.Contains("Another navigation", result.Exception.ToString());
            Assert.Same(unrelated, Assert.Single(window.Navigation.ModalStack));
        }
    }

    [Fact]
    public async Task NavigateFrom_ExternalPopDuringInitialization_IsNotUndone()
    {
        var window = CreateWindow("NavigationPage/Source/Middle/Current");
        var navigation = Assert.IsAssignableFrom<NavigationPage>(window.Page);
        var source = navigation.RootPage;
        var middle = navigation.Navigation.NavigationStack[1];
        var started = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var parameters = new NavigationParameters
        {
            { "duringInitialize", (Func<Task>)(async () => { started.SetResult(true); await release.Task; }) }
        };
        var request = GetService(window.CurrentPage).NavigateFromAsync("Middle", "Destination", parameters);
        await started.Task.WaitAsync(TimeSpan.FromSeconds(5));
        Assert.Same(middle, await navigation.PopAsync());
        release.SetResult(true);

        var result = await request;

        Assert.False(result.Success);
        Assert.Same(source, Assert.Single(navigation.Navigation.NavigationStack));
    }

    [Fact]
    public async Task NavigateFrom_ExternalPopDuringRollback_IsNotUndone()
    {
        var window = CreateWindow("NavigationPage/Source/Middle/Current");
        var navigation = Assert.IsAssignableFrom<NavigationPage>(window.Page);
        var source = navigation.RootPage;
        var middle = navigation.Navigation.NavigationStack[1];
        TestPageNavigationService.ThrowAfterNextPush = true;
        TestPageNavigationService.AfterNextPush = () =>
        {
            TestPageNavigationService.AfterNextPop = async () => Assert.Same(middle, await navigation.PopAsync());
            return Task.CompletedTask;
        };
        try
        {
            var result = await GetService(window.CurrentPage).NavigateFromAsync("Middle", "Destination?useModalNavigation=true");

            Assert.False(result.Success);
            Assert.Same(source, Assert.Single(navigation.Navigation.NavigationStack));
            Assert.Empty(window.Navigation.ModalStack);
        }
        finally
        {
            TestPageNavigationService.ThrowAfterNextPush = false;
            TestPageNavigationService.AfterNextPop = null;
            TestPageNavigationService.AfterNextPush = null;
        }
    }

    [Fact]
    public async Task NavigateFrom_DestinationPoppedByNativeCallback_DoesNotPublishStaleSuccess()
    {
        var window = CreateWindow("NavigationPage/Source/Current");
        var navigation = Assert.IsAssignableFrom<NavigationPage>(window.Page);
        var source = navigation.RootPage;
        var created = new List<MockNavigateFromPage>();
        TestPageNavigationService.AfterNextPush = async () => await navigation.PopAsync();
        try
        {
            var result = await GetService(window.CurrentPage).NavigateFromAsync("Source", "Destination",
                new NavigationParameters { { "createdPages", created } });

            Assert.False(result.Success);
            Assert.Same(source, Assert.Single(navigation.Navigation.NavigationStack));
            var destination = Assert.Single(created);
            Assert.Equal(0, destination.NavigatedToCount);
            Assert.Equal(1, destination.Destroyed);
        }
        finally
        {
            TestPageNavigationService.AfterNextPush = null;
        }
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task RelativeAndNamedNavigation_FailedRequestRestoresScopeForOtherApi(bool relativeFirst)
    {
        var window = CreateWindow("NavigationPage/Source");
        var source = Assert.IsType<MockNavigateFromPage>(window.CurrentPage);
        Assert.True((await GetService(source).NavigateAsync("Current?useModalNavigation=true")).Success);
        var current = Assert.IsType<MockNavigateFromPage>(window.CurrentPage);
        var scope = current.GetContainerProvider();
        var service = GetService(current);
        var created = new List<MockNavigateFromPage>();
        var parameters = new NavigationParameters
        {
            { "createdPages", created },
            { "failInitialize", true }
        };
        var from = current.NavigatedFromCount;

        var failed = relativeFirst
            ? await service.NavigateAsync("../Destination", parameters)
            : await service.NavigateFromAsync("Source", "Destination", parameters);

        Assert.False(failed.Success);
        Assert.Same(current, Assert.Single(window.Navigation.ModalStack));
        Assert.Same(source, Assert.Single(window.Page.Navigation.NavigationStack));
        Assert.Same(scope, current.GetContainerProvider());
        Assert.Same(current, scope.Resolve<IPageAccessor>().Page);
        Assert.Equal(from, current.NavigatedFromCount);
        Assert.Equal(0, current.Destroyed);
        var failedDestination = Assert.Single(created);
        Assert.Equal(1, failedDestination.Destroyed);
        Assert.Equal(0, failedDestination.NavigatedToCount);

        var succeeded = relativeFirst
            ? await service.NavigateFromAsync("Source", "Destination")
            : await service.NavigateAsync("../Destination");

        Assert.True(succeeded.Success, succeeded.Exception?.ToString());
        Assert.Empty(window.Navigation.ModalStack);
        Assert.Collection(window.Page.Navigation.NavigationStack,
            page => Assert.Same(source, page),
            page => Assert.Equal("Destination", ViewModelLocator.GetNavigationName(page)));
        Assert.Equal(from + 1, current.NavigatedFromCount);
        Assert.Equal(1, current.Destroyed);
        Assert.Equal(0, source.Destroyed);
        Assert.Equal(1, Assert.IsType<MockNavigateFromPage>(window.CurrentPage).NavigatedToCount);
    }

    private PrismWindow CreateWindow(string route)
    {
        var app = CreateBuilder(prism => prism.RegisterTypes(container =>
            container.RegisterForNavigation<MockNavigateFromPage>("Source")
                .RegisterForNavigation<MockNavigateFromPage>("Middle")
                .RegisterForNavigation<MockNavigateFromPage>("Current")
                .RegisterForNavigation<MockNavigateFromPage>("Destination"))
            .CreateWindow(route)).Build();
        return GetWindow(app);
    }

    private static INavigationService GetService(Page page) =>
        page.GetContainerProvider().Resolve<INavigationService>();
}
