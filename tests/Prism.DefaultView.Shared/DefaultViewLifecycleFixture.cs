#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Moq;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Navigation.Regions;
using Prism.Navigation.Regions.Behaviors;
using Xunit;
#if !WPF
using StaFactAttribute = Xunit.FactAttribute;
using StaTheoryAttribute = Xunit.TheoryAttribute;
#endif

namespace Prism.DefaultView.Tests;

[Collection("DefaultView lifecycle")]
public class DefaultViewLifecycleFixture : IDisposable
{
    private readonly Mock<IContainerExtension> _container = new();
    private readonly RegionViewRegistry _registry;
#if UNO_WINUI
    private readonly FieldInfo _threadAccessOverride;
    private readonly object? _previousThreadAccessOverride;
#endif

    public DefaultViewLifecycleFixture()
    {
#if UNO_WINUI
        // Grant this test thread headless access and restore its cached value on disposal.
        var dispatcher = Assembly.Load("Uno.UI.Dispatching").GetType("Uno.UI.Dispatching.NativeDispatcher")
            ?? throw new TypeLoadException("Uno.UI.Dispatching.NativeDispatcher was not found.");
        _threadAccessOverride = dispatcher.GetField("HasThreadAccessOverride", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            ?? dispatcher.GetField("_hasThreadAccess", BindingFlags.Static | BindingFlags.NonPublic)
            ?? throw new MissingFieldException("Uno dispatcher thread-access override was not found.");
        _previousThreadAccessOverride = _threadAccessOverride.GetValue(null);
        _threadAccessOverride.SetValue(null, _threadAccessOverride.FieldType == typeof(Func<bool>) ? (object)(Func<bool>)(() => true) : true);
#endif
        ContainerLocator.ResetContainer();
        ViewModelLocationProvider.Reset();
        ContainerLocator.SetContainerExtension(_container.Object);
        _registry = new RegionViewRegistry(_container.Object);
        _container.Setup(c => c.Resolve(typeof(LifecycleTestView))).Returns(() => new LifecycleTestView());
        var navigationRegistry = new RegionNavigationRegistry(new[]
        {
            new ViewRegistration { Type = ViewType.Region, View = typeof(LifecycleTestView), Name = "Default" }
        });
        _container.Setup(c => c.IsRegistered(typeof(IRegionNavigationRegistry))).Returns(true);
        _container.Setup(c => c.Resolve(typeof(IRegionNavigationRegistry))).Returns(navigationRegistry);
        _container.Setup(c => c.Resolve(typeof(AutoPopulateRegionBehavior)))
            .Returns(() => new AutoPopulateRegionBehavior(_registry));
    }

    public void Dispose()
    {
        ContainerLocator.ResetContainer();
        ViewModelLocationProvider.Reset();
#if UNO_WINUI
        _threadAccessOverride.SetValue(null, _previousThreadAccessOverride);
#endif
    }

    [StaTheory]
    [InlineData("name")]
    [InlineData("type")]
    [InlineData("instance")]
    public void AdapterSuppliesHostAndPopulatesExactlyOnce(string form)
    {
        var host = new ContentControl();
        var declaration = Declaration(form);
        RegionManager.SetDefaultView(host, declaration);
        var region = CreateAdapter().Initialize(host, "Main");
        var behavior = Assert.IsType<AutoPopulateRegionBehavior>(region.Behaviors[AutoPopulateRegionBehavior.BehaviorKey]);

        behavior.Attach();
        behavior.Attach();

        var view = Assert.IsType<LifecycleTestView>(Assert.Single(region.Views));
        Assert.Same(host, behavior.HostControl);
        Assert.Same(view, host.Content);
        Assert.Single(region.ActiveViews);
        Assert.Empty(_registry.GetContents("Main"));
        if (form == "instance")
            Assert.Same(declaration, view);
        if (form == "name")
            Assert.Same(view, region.GetView("Default"));
    }

    [StaTheory]
    [InlineData("name")]
    [InlineData("type")]
    [InlineData("instance")]
    public void RecreatedHostsWithSameNameDoNotAccumulateRegistrations(string form)
    {
        var manager = new RegionManager();
        object? previous = null;
        for (var i = 0; i < 20; i++)
        {
            var host = new ContentControl();
            RegionManager.SetDefaultView(host, Declaration(form));
            var region = CreateAdapter().Initialize(host, "Main");
            manager.Regions.Add(region);
            var view = Assert.Single(region.Views);
            Assert.NotSame(previous, view);
            Assert.Empty(_registry.GetContents("Main"));
            region.RemoveAll();
            Assert.Null(host.Content);
            manager.Regions.Remove("Main");
            Assert.Empty(manager.Regions);
            previous = view;
        }
    }

    [StaTheory]
    [InlineData("name")]
    [InlineData("type")]
    [InlineData("instance")]
    public void ReleasedHostsAndViewsAreCollectedWhileRegistryLives(string form)
    {
        var weak = CreateAndRelease(form);
        for (var i = 0; i < 3; i++)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }
        Assert.All(weak, reference => Assert.False(reference.IsAlive));
        Assert.Empty(_registry.GetContents("Main"));
        GC.KeepAlive(_registry);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private WeakReference[] CreateAndRelease(string form)
    {
        var host = new ContentControl();
        RegionManager.SetDefaultView(host, Declaration(form));
        var region = CreateAdapter().Initialize(host, "Main");
        var manager = new RegionManager();
        manager.Regions.Add(region);
        var view = Assert.Single(region.Views);
        var references = new[] { new WeakReference(host), new WeakReference(region), new WeakReference(view) };
        region.RemoveAll();
        manager.Regions.Remove("Main");
        // Moq records resolved return values; those are test-only external roots.
        _container.Invocations.Clear();
        // Do not clear DefaultView: the host/instance cycle itself must be collectible.
        return references;
    }

    [StaFact]
    public void RepeatedAttachBeforeNameDoesNotDuplicatePopulationOrLateNotifications()
    {
        var host = new ContentControl();
        RegionManager.SetDefaultView(host, "Default");
        var region = new Region();
        var behavior = new AutoPopulateRegionBehavior(_registry) { Region = region, HostControl = host };
        behavior.Attach();
        behavior.Attach();
        Assert.Empty(region.Views);
        region.Name = "Main";
        behavior.Attach();
        _registry.RegisterViewWithRegion("Main", () => new LifecycleTestView());
        Assert.Equal(2, region.Views.Count());
        Assert.Single(_registry.GetContents("Main"));
    }

    [StaFact]
    public void SameNamedHostsRemainIndependent()
    {
        var first = new ContentControl();
        var second = new ContentControl();
        var firstDefault = new LifecycleTestView();
        var secondDefault = new LifecycleTestView();
        RegionManager.SetDefaultView(first, firstDefault);
        RegionManager.SetDefaultView(second, secondDefault);
        var firstRegion = CreateAdapter().Initialize(first, "Main");
        var secondRegion = CreateAdapter().Initialize(second, "Main");
        new RegionManager().Regions.Add(firstRegion);
        new RegionManager().Regions.Add(secondRegion);
        Assert.Same(firstDefault, Assert.Single(firstRegion.Views));
        Assert.Same(secondDefault, Assert.Single(secondRegion.Views));
        Assert.Empty(_registry.GetContents("Main"));
    }

    [StaFact]
    public void DiscoveryKeepsOwnershipAndAnAlreadyAddedInstanceIsNotAddedTwice()
    {
        var view = new LifecycleTestView();
        _registry.RegisterViewWithRegion("Main", () => view);
        var host = new ContentControl();
        RegionManager.SetDefaultView(host, view);
        var region = CreateAdapter().Initialize(host, "Main");
        Assert.Same(view, Assert.Single(region.Views));
        Assert.Same(view, Assert.Single(_registry.GetContents("Main")));
    }

    [StaFact]
    public void DistinctInstancesThatCompareEqualAreBothAdded()
    {
        var discovered = new LifecycleEqualView();
        var declared = new LifecycleEqualView();
        _registry.RegisterViewWithRegion("Main", () => discovered);
        var host = new ContentControl();
        RegionManager.SetDefaultView(host, declared);
        var region = CreateAdapter().Initialize(host, "Main");
        Assert.Equal(2, region.Views.Count());
        Assert.Contains(region.Views, view => ReferenceEquals(view, discovered));
        Assert.Contains(region.Views, view => ReferenceEquals(view, declared));
    }

    [StaFact]
    public void ExistingUnnamedSingletonIsNotReattachedOrRenamedByStringDefault()
    {
        var view = new LifecycleTestView();
        _container.Setup(c => c.Resolve(typeof(LifecycleTestView))).Returns(view);
        _registry.RegisterViewWithRegion("Main", () => view);
        var host = new ContentControl();
        RegionManager.SetDefaultView(host, "Default");
        var region = CreateAdapter().Initialize(host, "Main");
        Assert.Same(view, Assert.Single(region.Views));
        Assert.Null(region.GetView("Default"));
    }

    [StaFact]
    public void DefaultDoesNotDisplaceDiscoveredContent()
    {
        var discovered = new LifecycleTestView();
        _registry.RegisterViewWithRegion("Main", () => discovered);
        var host = new ContentControl();
        RegionManager.SetDefaultView(host, "Default");
        var region = CreateAdapter().Initialize(host, "Main");
        Assert.Equal(2, region.Views.Count());
        Assert.Same(discovered, Assert.Single(region.ActiveViews));
        Assert.Same(discovered, host.Content);
    }

    [StaFact]
    public void DefaultIsInitialOnlyAndRemovalDoesNotRepopulate()
    {
        var host = new ContentControl();
        RegionManager.SetDefaultView(host, "Default");
        var region = CreateAdapter().Initialize(host, "Main");
        var view = Assert.Single(region.Views);
        RegionManager.SetDefaultView(host, new LifecycleTestView());
        Assert.Same(view, Assert.Single(region.Views));
        region.Remove(view);
        region.Behaviors[AutoPopulateRegionBehavior.BehaviorKey].Attach();
        Assert.Empty(region.Views);
    }

    [StaFact]
    public void NullDefaultAddsNothingAndHostCannotBeReassignedAfterAttach()
    {
        var host = new ContentControl();
        var region = CreateAdapter().Initialize(host, "Main");
        Assert.Empty(region.Views);
        var behavior = (AutoPopulateRegionBehavior)region.Behaviors[AutoPopulateRegionBehavior.BehaviorKey];
        Assert.Throws<InvalidOperationException>(() => behavior.HostControl = new ContentControl());
    }

    [StaFact]
    public void UnregisteredNameFailsClearlyWithoutAddingGlobalRegistration()
    {
        var host = new ContentControl();
        RegionManager.SetDefaultView(host, "Missing");
        Assert.Throws<KeyNotFoundException>(() => CreateAdapter().Initialize(host, "Main"));
        Assert.Empty(_registry.GetContents("Main"));
    }

    [StaFact]
    public void UnregisteredTypeUsesExistingDesktopContainerFallback()
    {
        var host = new ContentControl();
        _container.Setup(c => c.Resolve(typeof(LifecycleOtherView))).Returns(() => new LifecycleOtherView());
        RegionManager.SetDefaultView(host, typeof(LifecycleOtherView));
        var region = CreateAdapter().Initialize(host, "Main");
        Assert.IsType<LifecycleOtherView>(Assert.Single(region.Views));
    }

    private static object Declaration(string form) => form switch
    {
        "name" => "Default",
        "type" => typeof(LifecycleTestView),
        _ => new LifecycleTestView()
    };

    private ContentControlRegionAdapter CreateAdapter()
    {
        var behaviors = new RegionBehaviorFactory(_container.Object);
        behaviors.AddIfMissing<AutoPopulateRegionBehavior>(AutoPopulateRegionBehavior.BehaviorKey);
        return new ContentControlRegionAdapter(behaviors);
    }
}
