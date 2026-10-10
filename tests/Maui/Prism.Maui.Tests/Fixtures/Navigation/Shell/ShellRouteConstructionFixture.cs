using Prism.Common;
using Prism.Maui.Tests.Mocks.Shell;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Navigation.Xaml;

namespace Prism.Maui.Tests.Fixtures.Navigation.Shell;

public sealed class ShellRouteConstructionFixture
{
    [Fact]
    public void RouteFactoryCreatesSeparatePageScopesWithExplicitViewModels()
    {
        using var host = new ShellProbeHarness();
        var first = host.CreatePage();
        var second = host.CreatePage();
        var firstModel = Assert.IsType<ShellProbeViewModel>(first.BindingContext);
        var secondModel = Assert.IsType<ShellProbeViewModel>(second.BindingContext);

        Assert.NotSame(first, second);
        Assert.NotSame(first.GetContainerProvider(), second.GetContainerProvider());
        Assert.NotSame(first.Dependency, second.Dependency);
        Assert.Same(first.Dependency, firstModel.Dependency);
        Assert.Same(second.Dependency, secondModel.Dependency);
        Assert.Same(first, first.GetContainerProvider().Resolve<IPageAccessor>().Page);
        Assert.Same(second, second.GetContainerProvider().Resolve<IPageAccessor>().Page);
        Assert.Equal("detail", ViewModelLocator.GetNavigationName(first));
        Assert.Equal(typeof(ShellProbeViewModel), first.GetValue(ViewModelLocator.ViewModelProperty));
    }

    [Fact]
    public void GlobalRouteFactoryUsesTheProviderSuppliedByTheOwningHost()
    {
        using var firstHost = new ShellProbeHarness();
        using var secondHost = new ShellProbeHarness();
        var route = $"prism-shell-probe-{Guid.NewGuid():N}";
        Routing.RegisterRoute(route, new ShellRouteFactoryPrototype("detail"));
        try
        {
            var first = firstHost.Track(Assert.IsType<ShellProbePage>(Routing.GetOrCreateContent(route, firstHost.Services)));
            var second = secondHost.Track(Assert.IsType<ShellProbePage>(Routing.GetOrCreateContent(route, secondHost.Services)));

            Assert.Same(firstHost.Container.CurrentScope, first.GetContainerProvider());
            Assert.Same(secondHost.Container.CurrentScope, second.GetContainerProvider());
            Assert.NotSame(first.Dependency, second.Dependency);
            Assert.Equal(route, Routing.GetRoute(first));
        }
        finally
        {
            Routing.UnRegisterRoute(route);
        }
    }

    [Fact]
    public void ConflictingGlobalRouteRegistrationFailsInsteadOfReplacingAnotherHostFactory()
    {
        var route = $"prism-shell-collision-probe-{Guid.NewGuid():N}";
        Routing.RegisterRoute(route, new ShellRouteFactoryPrototype("detail"));
        try
        {
            Assert.Throws<ArgumentException>(() =>
                Routing.RegisterRoute(route, new ShellRouteFactoryPrototype("other-detail")));
        }
        finally
        {
            Routing.UnRegisterRoute(route);
        }
    }

    [Fact]
    public void MissingHostContextFailsWithoutGlobalContainerFallback()
    {
        var factory = new ShellRouteFactoryPrototype("detail");
        Assert.Throws<InvalidOperationException>(() => factory.GetOrCreate());
    }

    [Fact]
    public void UnknownRouteFailsBeforeAllocatingAScope()
    {
        using var host = new ShellProbeHarness();
        var factory = new ShellRouteFactoryPrototype("missing");

        Assert.Throws<KeyNotFoundException>(() => factory.GetOrCreate(host.Services));
        Assert.Null(host.Container.CurrentScope);
    }

    [Fact]
    public void PrismNavigationContainersAreRejectedBeforeScopeCreation()
    {
        using var host = new ShellProbeHarness();
        host.Container.RegisterForNavigation<NavigationPage>("stack");
        var factory = new ShellRouteFactoryPrototype("stack");

        Assert.Throws<NotSupportedException>(() => factory.GetOrCreate(host.Services));
        Assert.Null(host.Container.CurrentScope);
    }

    [Fact]
    public void PageConstructionFailureDisposesResolvedScopedDependencies()
    {
        using var host = new ShellProbeHarness();
        var dependency = new ShellProbeDependency();
        host.Container.RegisterScoped<ShellProbeDependency>(() => dependency);
        host.Container.RegisterForNavigation<ShellProbeFailingPage>("broken-page");

        var error = Assert.Throws<ViewCreationException>(() =>
            new ShellRouteFactoryPrototype("broken-page").GetOrCreate(host.Services));

        Assert.Contains("Shell probe page creation failed.", error.ToString());
        Assert.Equal(1, dependency.DisposeCount);
    }

    [Fact]
    public void ViewModelConstructionFailureDetachesAndDisposesThePageScope()
    {
        using var host = new ShellProbeHarness();
        var dependency = new ShellProbeDependency();
        ShellProbePage created = null;
        host.Container.RegisterScoped<ShellProbeDependency>(() => dependency);
        host.Container.RegisterForNavigation<ShellProbePage, ShellProbeFailingViewModel>("broken-model");
        host.Container.Register<ShellProbePage>(provider =>
        {
            created = new ShellProbePage(provider.Resolve<ShellProbeDependency>());
            return created;
        });

        var error = Assert.Throws<ViewModelCreationException>(() =>
            new ShellRouteFactoryPrototype("broken-model").GetOrCreate(host.Services));

        Assert.Contains("Shell probe view model creation failed.", error.ToString());
        Assert.Equal(1, dependency.DisposeCount);
        Assert.NotNull(created);
        Assert.Null(created.GetContainerProvider());
    }

    [Fact]
    public void CleanupFailureDoesNotHideTheOriginalConstructionFailure()
    {
        using var host = new ShellProbeHarness();
        var dependency = new ShellProbeDependency { ThrowOnDispose = true };
        host.Container.RegisterScoped<ShellProbeDependency>(() => dependency);
        host.Container.RegisterForNavigation<ShellProbeFailingPage>("broken-page");

        var error = Assert.Throws<ViewCreationException>(() =>
            new ShellRouteFactoryPrototype("broken-page").GetOrCreate(host.Services));

        Assert.Contains("Shell probe page creation failed.", error.ToString());
        var cleanup = Assert.IsType<InvalidOperationException>(error.Data["ShellScopeCleanupException"]);
        Assert.Equal("Shell probe dependency disposal failed.", cleanup.Message);
        Assert.Equal(1, dependency.DisposeCount);
    }

    [Fact]
    public void ExplicitViewModelWiringWorksWithoutReflectionBasedDiscovery()
    {
        const string reflectionSwitch = "Prism.Mvvm.ReflectionBasedViewModelLocationEnabled";
        var configured = AppContext.TryGetSwitch(reflectionSwitch, out var previous);
        AppContext.SetSwitch(reflectionSwitch, false);
        try
        {
            using var host = new ShellProbeHarness();
            var page = host.CreatePage();

            var model = Assert.IsType<ShellProbeViewModel>(page.BindingContext);
            Assert.Same(page.Dependency, model.Dependency);
        }
        finally
        {
            AppContext.SetSwitch(reflectionSwitch, configured ? previous : true);
        }
    }

    [Fact]
    public void ScopeBehaviorReleasesOnlyTheDepartingPageScope()
    {
        using var host = new ShellProbeHarness();
        var departing = host.CreatePage();
        var retained = host.CreatePage();

        departing.Behaviors.Clear();
        departing.SetContainerProvider(null);

        Assert.Equal(1, departing.Dependency.DisposeCount);
        Assert.Equal(0, retained.Dependency.DisposeCount);
        Assert.Null(departing.GetContainerProvider());
        Assert.Same(retained, retained.GetContainerProvider().Resolve<IPageAccessor>().Page);
    }

    [Fact]
    public void ShellContentTemplateReusesTheScopedConstructionPathAndCachesItsRoot()
    {
        using var host = new ShellProbeHarness();
        var content = new ShellContent
        {
            Route = "home",
            ContentTemplate = new DataTemplate(() => host.CreatePage())
        };
        var controller = (IShellContentController)content;

        var first = Assert.IsType<ShellProbePage>(controller.GetOrCreateContent());
        var second = controller.GetOrCreateContent();

        Assert.Same(first, second);
        Assert.NotNull(first.GetContainerProvider());
        Assert.IsType<ShellProbeViewModel>(first.BindingContext);
        Assert.Equal(0, first.Dependency.DisposeCount);
    }
}
