using System.Reflection;
using Prism.Dialogs;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Navigation.Regions;
using Xunit;
#if !PRISM_UNITY
using Prism.Container.DryIoc;
#endif

namespace Prism.ViewRegistry.Tests;

public class ViewRegistryFixture : IDisposable
{
#if UNO_WINUI
    private readonly FieldInfo _threadAccessOverride;
    private readonly object _previousThreadAccessOverride;
#endif

    public ViewRegistryFixture()
    {
#if UNO_WINUI
        // The Skia runtime allows control/DependencyProperty tests without starting a native window.
        var dispatcher = Assembly.Load("Uno.UI.Dispatching").GetType("Uno.UI.Dispatching.NativeDispatcher")!;
        _threadAccessOverride = dispatcher.GetField("HasThreadAccessOverride", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)!;
        _previousThreadAccessOverride = _threadAccessOverride.GetValue(null)!;
        _threadAccessOverride.SetValue(null, (Func<bool>)(() => true));
#endif
        ViewModelLocationProvider.Reset();
        ContainerLocator.ResetContainer();
    }

    public void Dispose()
    {
        ViewModelLocationProvider.Reset();
        ContainerLocator.ResetContainer();
#if UNO_WINUI
        _threadAccessOverride.SetValue(null, _previousThreadAccessOverride);
#endif
    }

    [StaFact]
    public void RegistrationsAreSeparatedByPurposeAndUseTheLastMatchingName()
    {
        var container = CreateContainer();
        container.RegisterForNavigation<TestView, FirstViewModel>("Shared");
        container.RegisterDialog<OtherView, DialogViewModel>("Shared");
        container.RegisterForNavigation<OtherView, SecondViewModel>("Shared");

        var regions = container.Resolve<IRegionNavigationRegistry>();
        var dialogs = container.Resolve<IDialogViewRegistry>();

        Assert.Equal(2, regions.Registrations.Count());
        Assert.Single(dialogs.Registrations);
        Assert.Equal(typeof(OtherView), regions.GetViewType("Shared"));
        Assert.Equal(typeof(OtherView), dialogs.GetViewType("Shared"));
        Assert.Equal("Shared", regions.GetViewModelNavigationKey(typeof(SecondViewModel)));
        Assert.Single(regions.ViewsOfType(typeof(TestView)));
    }

    [StaFact]
    public void RegistryResolvedBeforeModuleRegistrationSeesNewViews()
    {
        var container = CreateContainer();
        var registry = container.Resolve<IRegionNavigationRegistry>();
        Assert.False(registry.IsRegistered("ModuleView"));

        container.RegisterForNavigation<TestView, FirstViewModel>("ModuleView");

        Assert.Same(registry, container.Resolve<IRegionNavigationRegistry>());
        Assert.True(registry.IsRegistered("ModuleView"));
        Assert.Equal(typeof(TestView), registry.GetViewType("ModuleView"));
    }

    [StaFact]
    public void AliasesCreateTheRequestedViewModel()
    {
        var container = CreateContainer();
        container.RegisterForNavigation<TestView, FirstViewModel>("First");
        container.RegisterForNavigation<TestView, SecondViewModel>("Second");
        var registry = container.Resolve<IRegionNavigationRegistry>();

        var first = Assert.IsType<TestView>(registry.CreateView(container, "First"));
        var second = Assert.IsType<TestView>(registry.CreateView(container, "Second"));

        Assert.IsType<FirstViewModel>(first.DataContext);
        Assert.IsType<SecondViewModel>(second.DataContext);
        Assert.Equal("First", ViewModelLocator.GetNavigationName(first));
        Assert.Equal("Second", ViewModelLocator.GetNavigationName(second));
        Assert.NotSame(first, second);
    }

    [StaFact]
    public void NavigationReusesOnlyTheMatchingAlias()
    {
        var container = CreateContainer();
        container.RegisterForNavigation<TestView, FirstViewModel>("First");
        container.RegisterForNavigation<TestView, SecondViewModel>("Second");
        var loader = container.Resolve<IRegionNavigationContentLoader>();
        var region = new Region();

        var first = loader.LoadContent(region, Context("First"));
        var second = loader.LoadContent(region, Context("Second"));

        Assert.NotSame(first, second);
        Assert.Same(first, loader.LoadContent(region, Context("First")));
        Assert.Same(second, loader.LoadContent(region, Context("Second")));
        Assert.Equal(2, region.Views.Count());
    }

    [StaFact]
    public void NavigationHonorsIsNavigationTarget()
    {
        var container = CreateContainer();
        container.RegisterForNavigation<TestView, RejectReuseViewModel>();
        var loader = container.Resolve<IRegionNavigationContentLoader>();
        var region = new Region();

        var first = loader.LoadContent(region, Context(nameof(TestView)));
        var second = loader.LoadContent(region, Context(nameof(TestView)));

        Assert.NotSame(first, second);
    }

    [StaFact]
    public void NavigationReusesAnInjectedViewWithoutAnAlias()
    {
        var container = CreateContainer();
        container.RegisterForNavigation<TestView>("Alias");
        var view = new TestView();
        var region = new Region();
        region.Add(view);

        Assert.Same(view, container.Resolve<IRegionNavigationContentLoader>().LoadContent(region, Context("Alias")));
    }

    [StaFact]
    public void DiscoveryUsesTheRegistryForNamesAndTypes()
    {
        var container = CreateContainer();
        container.RegisterForNavigation<TestView, FirstViewModel>("First");
        container.RegisterForNavigation<TestView, SecondViewModel>("Second");
        var registry = container.Resolve<IRegionViewRegistry>();
        registry.RegisterViewWithRegion("ByName", "First");
        registry.RegisterViewWithRegion("ByType", typeof(TestView));

        var named = Assert.IsType<TestView>(Assert.Single(registry.GetContents("ByName", container)));
        var typed = Assert.IsType<TestView>(Assert.Single(registry.GetContents("ByType", container)));

        Assert.IsType<FirstViewModel>(named.DataContext);
        Assert.IsType<SecondViewModel>(typed.DataContext);
    }

    [StaFact]
    public void NamedDiscoveryUsesTheSuppliedProviderForTheViewAndViewModel()
    {
        var container = CreateContainer();
        container.RegisterForNavigation<TestView, FirstViewModel>("First");
        container.RegisterForNavigation<TestView, SecondViewModel>("Second");
        var registry = container.Resolve<IRegionViewRegistry>();
        registry.RegisterViewWithRegion("Region", "First");
        var view = new TestView();
        var viewModel = new FirstViewModel();
        var provider = new Moq.Mock<IContainerProvider>(Moq.MockBehavior.Strict);
        provider.Setup(x => x.Resolve(typeof(TestView))).Returns(view);
        provider.Setup(x => x.Resolve(typeof(FirstViewModel))).Returns(viewModel);

        Assert.Same(view, Assert.Single(registry.GetContents("Region", provider.Object)));
        Assert.Same(viewModel, view.DataContext);
        Assert.Equal("First", ViewModelLocator.GetNavigationName(view));
    }

    [StaFact]
    public void ExplicitDataContextIsPreserved()
    {
        var container = CreateContainer();
        container.RegisterForNavigation<ContextView, FirstViewModel>();
        var view = Assert.IsType<ContextView>(container.Resolve<IRegionNavigationRegistry>()
            .CreateView(container, nameof(ContextView)));

        Assert.Same(view.OriginalContext, view.DataContext);
    }

    [StaFact]
    public void DisabledAutowiringIsRespected()
    {
        var container = CreateContainer();
        container.RegisterForNavigation<DisabledView, FirstViewModel>();
        var view = Assert.IsType<DisabledView>(container.Resolve<IRegionNavigationRegistry>()
            .CreateView(container, nameof(DisabledView)));

        Assert.Null(view.DataContext);
    }

    [StaFact]
    public void ConstructorAutowiringDoesNotOverrideTheRequestedAlias()
    {
        var container = CreateContainer();
        container.RegisterForNavigation<AutowiringView, FirstViewModel>("First");
        container.RegisterForNavigation<AutowiringView, SecondViewModel>("Second");
        var view = Assert.IsType<AutowiringView>(container.Resolve<IRegionNavigationRegistry>()
            .CreateView(container, "First"));

        Assert.IsType<FirstViewModel>(view.DataContext);
    }

    [StaFact]
    public void ViewModelUsesTheProviderPassedToCreateView()
    {
        var container = CreateContainer();
        container.RegisterForNavigation<TestView, FirstViewModel>();
        var provider = CreateBareContainer();
        var viewModel = new FirstViewModel();
        provider.RegisterInstance(viewModel);
        var view = Assert.IsType<TestView>(container.Resolve<IRegionNavigationRegistry>()
            .CreateView(provider, nameof(TestView)));

        Assert.Same(viewModel, view.DataContext);
    }

    [StaFact]
    public void ExplicitRegistrationUsesTheCustomViewModelFactory()
    {
        var container = CreateContainer();
        container.RegisterForNavigation<TestView, FirstViewModel>("First");
        container.RegisterForNavigation<TestView, SecondViewModel>("Second");
        var viewModel = new FirstViewModel();
        object factoryView = null;
        ViewModelLocationProvider.SetDefaultViewModelFactory((view, type) =>
        {
            Assert.Equal(typeof(FirstViewModel), type);
            factoryView = view;
            return viewModel;
        });

        var view = Assert.IsType<TestView>(container.Resolve<IRegionNavigationRegistry>().CreateView(container, "First"));

        Assert.Same(view, factoryView);
        Assert.Same(viewModel, view.DataContext);
    }

    [StaFact]
    public void RegisteredViewModelFactoryRemainsAvailable()
    {
        var container = CreateContainer();
        container.RegisterForNavigation<TestView, FirstViewModel>();
        var viewModel = new FirstViewModel();
        ViewModelLocationProvider.Register<TestView>(() => viewModel);

        var view = Assert.IsType<TestView>(container.Resolve<IRegionNavigationRegistry>()
            .CreateView(container, nameof(TestView)));

        Assert.Same(viewModel, view.DataContext);
    }

    [StaFact]
    public void ConventionBasedAutowiringRemainsAvailable()
    {
        var container = CreateContainer();
        container.RegisterForNavigation<TestView>();
        ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver(_ => typeof(FirstViewModel));
        var view = Assert.IsType<TestView>(container.Resolve<IRegionNavigationRegistry>()
            .CreateView(container, nameof(TestView)));

        Assert.IsType<FirstViewModel>(view.DataContext);
    }

    [StaFact]
    public void DialogAndRegionCanUseTheSameNameWithDifferentViewModels()
    {
        var container = CreateContainer();
        container.RegisterForNavigation<TestView, FirstViewModel>("Shared");
        container.RegisterDialog<TestView, DialogViewModel>("Shared");
        var region = Assert.IsType<TestView>(container.Resolve<IRegionNavigationRegistry>().CreateView(container, "Shared"));
        var dialog = Assert.IsType<TestView>(container.Resolve<IDialogViewRegistry>().CreateView(container, "Shared"));

        Assert.IsType<FirstViewModel>(region.DataContext);
        Assert.IsType<DialogViewModel>(dialog.DataContext);
    }

    [StaFact]
    public void UnknownAndWrongCategoryNamesAreRejected()
    {
        var container = CreateContainer();
        container.RegisterDialog<TestView, DialogViewModel>("DialogOnly");
        var registry = container.Resolve<IRegionNavigationRegistry>();

        Assert.Null(registry.GetViewType("Missing"));
        Assert.Throws<KeyNotFoundException>(() => registry.CreateView(container, "Missing"));
        Assert.Throws<KeyNotFoundException>(() => registry.CreateView(container, "DialogOnly"));
    }

    [StaFact]
    public void ViewCreationFailureIncludesTheAliasAndCategory()
    {
        var container = CreateContainer();
        container.RegisterForNavigation<ThrowingView>("Broken");

        var error = Assert.Throws<ViewCreationException>(() =>
            container.Resolve<IRegionNavigationRegistry>().CreateView(container, "Broken"));

        Assert.Equal("Broken", error.ViewName);
        Assert.Equal(ViewType.Region, error.ViewType);
        Assert.IsType<ContainerResolutionException>(error.InnerException);
    }

    [StaFact]
    public void ViewModelFailureIsReportedSeparately()
    {
        var container = CreateContainer();
        container.RegisterForNavigation<TestView, ThrowingViewModel>("Broken");

        var error = Assert.Throws<ViewModelCreationException>(() =>
            container.Resolve<IRegionNavigationRegistry>().CreateView(container, "Broken"));

        Assert.IsType<TestView>(error.View);
        Assert.Equal("Broken", ViewModelLocator.GetNavigationName((TestView)error.View));
    }

    [StaTheory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void EmptyNavigationNamesDefaultToTheViewType(string name)
    {
        var container = CreateContainer();
        container.RegisterForNavigation(typeof(TestView), name);

        Assert.True(container.Resolve<IRegionNavigationRegistry>().IsRegistered(nameof(TestView)));
    }

    [StaFact]
    public void InvalidViewTypesAreRejectedAtRegistration()
    {
        var container = CreateContainer();

        Assert.Throws<ArgumentException>(() => container.RegisterDialog<object>());
#if UNO_WINUI
        Assert.Throws<ArgumentException>(() => container.RegisterForNavigation<object>());
#endif
        Assert.Throws<ArgumentNullException>(() => container.RegisterForNavigation(null!, "Invalid"));
    }

    [StaFact]
    public void DialogServiceUsesTheInjectedRegistry()
    {
        var container = new Moq.Mock<IContainerExtension>(Moq.MockBehavior.Strict);
        var registry = new Moq.Mock<IDialogViewRegistry>(Moq.MockBehavior.Strict);
        var viewModel = new DialogViewModel();
        var view = new TestView { DataContext = viewModel };
        var parameters = new DialogParameters();
        registry.Setup(x => x.CreateView(container.Object, "Dialog")).Returns(view);
        var window = new Moq.Mock<IDialogWindow>();
        window.SetupAllProperties();
        var service = new DialogService(container.Object, registry.Object);

        typeof(DialogService).GetMethod("ConfigureDialogWindowContent", BindingFlags.NonPublic | BindingFlags.Instance)!
            .Invoke(service, new object[] { "Dialog", window.Object, parameters });

        Assert.Same(view, window.Object.Content);
        Assert.Same(viewModel, window.Object.DataContext);
        Assert.Same(parameters, viewModel.Parameters);
        registry.Verify(x => x.CreateView(container.Object, "Dialog"), Moq.Times.Once);
        container.VerifyNoOtherCalls();
    }

#if !UNO_WINUI
    [StaFact]
    public void RegionNavigationSupportsViewModelsAsContent()
    {
        var container = CreateContainer();
        container.RegisterForNavigation<FirstViewModel>("ViewModel");
        var loader = container.Resolve<IRegionNavigationContentLoader>();
        var region = new Region();

        var viewModel = loader.LoadContent(region, Context("ViewModel"));

        Assert.IsType<FirstViewModel>(viewModel);
        Assert.Same(viewModel, loader.LoadContent(region, Context("ViewModel")));
    }
#endif

    private static NavigationContext Context(string name) => new(null!, new Uri(name, UriKind.Relative));

    private static IContainerExtension CreateContainer()
    {
        var container = CreateBareContainer();
        ContainerLocator.SetContainerExtension(container);
        typeof(PrismApplicationBase).Assembly.GetType("Prism.PrismInitializationExtensions")!
            .GetMethod("ConfigureViewModelLocator", BindingFlags.NonPublic | BindingFlags.Static)!
            .Invoke(null, null);
        typeof(PrismApplicationBase).Assembly.GetType("Prism.PrismInitializationExtensions")!
            .GetMethod("RegisterRequiredTypes", BindingFlags.NonPublic | BindingFlags.Static)!
            .Invoke(null, new object[] { container
#if !UNO_WINUI
                , new Prism.Modularity.ModuleCatalog()
#endif
            });
        return container;
    }

    private static IContainerExtension CreateBareContainer() =>
#if PRISM_UNITY
        new Prism.Container.Unity.UnityContainerExtension(new global::Unity.UnityContainer());
#else
        new DryIocContainerExtension();
#endif

    private static void SetAutowire(DependencyObject view, bool value)
    {
#if UNO_WINUI
        ViewModelLocator.SetAutowireViewModel(view, value);
#else
        ViewModelLocator.SetAutoWireViewModel(view, value);
#endif
    }

    public class TestView : ContentControl { }
    public class OtherView : ContentControl { }
    public class ContextView : ContentControl
    {
        public object OriginalContext { get; } = new();
        public ContextView() => DataContext = OriginalContext;
    }
    public class DisabledView : ContentControl
    {
        public DisabledView() => SetAutowire(this, false);
    }
    public class AutowiringView : ContentControl
    {
        public AutowiringView() => SetAutowire(this, true);
    }
    public class ThrowingView : ContentControl
    {
        public ThrowingView() => throw new InvalidOperationException("View failure");
    }
    public class ThrowingViewModel
    {
        public ThrowingViewModel() => throw new InvalidOperationException("ViewModel failure");
    }
    public class FirstViewModel { }
    public class SecondViewModel { }
    public class RejectReuseViewModel : IRegionAware
    {
        public bool IsNavigationTarget(NavigationContext context) => false;
        public void OnNavigatedTo(NavigationContext context) { }
        public void OnNavigatedFrom(NavigationContext context) { }
    }
    public class DialogViewModel : IDialogAware
    {
        public IDialogParameters Parameters { get; private set; }
        public DialogCloseListener RequestClose { get; }
        public bool CanCloseDialog() => true;
        public void OnDialogClosed() { }
        public void OnDialogOpened(IDialogParameters parameters) => Parameters = parameters;
    }
}
