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

[CollectionDefinition("DefaultView lifecycle", DisableParallelization = true)]
public class DefaultViewLifecycleCollection { }

[Collection("DefaultView lifecycle")]
public class DefaultViewLifecycleFixture : IDisposable
{
    private readonly Mock<IContainerExtension> container = new();
    private readonly RegionViewRegistry registry;
#if UNO_WINUI
    private readonly FieldInfo threadAccessOverride;
    private readonly object previousThreadAccessOverride;
#endif

    public DefaultViewLifecycleFixture()
    {
#if UNO_WINUI
        // Same headless dispatcher setup used by Prism.ViewRegistry.Shared.
        var dispatcher = Assembly.Load("Uno.UI.Dispatching").GetType("Uno.UI.Dispatching.NativeDispatcher");
        threadAccessOverride = dispatcher.GetField("HasThreadAccessOverride", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        previousThreadAccessOverride = threadAccessOverride.GetValue(null);
        threadAccessOverride.SetValue(null, (Func<bool>)(() => true));
#endif
        ContainerLocator.ResetContainer();
        ViewModelLocationProvider.Reset();
        ContainerLocator.SetContainerExtension(container.Object);
        registry = new RegionViewRegistry(container.Object);
        container.Setup(c => c.Resolve(typeof(TestView))).Returns(() => new TestView());
        var navigationRegistry = new RegionNavigationRegistry(new[]
        {
            new ViewRegistration { Type = ViewType.Region, View = typeof(TestView), Name = "Default" }
        });
        container.Setup(c => c.IsRegistered(typeof(IRegionNavigationRegistry))).Returns(true);
        container.Setup(c => c.Resolve(typeof(IRegionNavigationRegistry))).Returns(navigationRegistry);
        container.Setup(c => c.Resolve(typeof(AutoPopulateRegionBehavior)))
            .Returns(() => new AutoPopulateRegionBehavior(registry));
    }

    public void Dispose()
    {
        ContainerLocator.ResetContainer();
        ViewModelLocationProvider.Reset();
#if UNO_WINUI
        threadAccessOverride.SetValue(null, previousThreadAccessOverride);
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

        var view = Assert.IsType<TestView>(Assert.Single(region.Views));
        Assert.Same(host, behavior.HostControl);
        Assert.Same(view, host.Content);
        Assert.Single(region.ActiveViews);
        Assert.Empty(registry.GetContents("Main"));
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
        object previous = null;
        for (var i = 0; i < 20; i++)
        {
            var host = new ContentControl();
            RegionManager.SetDefaultView(host, Declaration(form));
            var region = CreateAdapter().Initialize(host, "Main");
            manager.Regions.Add(region);
            var view = Assert.Single(region.Views);
            Assert.NotSame(previous, view);
            Assert.Empty(registry.GetContents("Main"));
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
        Assert.Empty(registry.GetContents("Main"));
        GC.KeepAlive(registry);
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
        container.Invocations.Clear();
        // Do not clear DefaultView: the host/instance cycle itself must be collectible.
        return references;
    }

    [StaFact]
    public void RepeatedAttachBeforeNameDoesNotDuplicatePopulationOrLateNotifications()
    {
        var host = new ContentControl();
        RegionManager.SetDefaultView(host, "Default");
        var region = new Region();
        var behavior = new AutoPopulateRegionBehavior(registry) { Region = region, HostControl = host };
        behavior.Attach();
        behavior.Attach();
        Assert.Empty(region.Views);
        region.Name = "Main";
        behavior.Attach();
        registry.RegisterViewWithRegion("Main", () => new TestView());
        Assert.Equal(2, region.Views.Count());
        Assert.Single(registry.GetContents("Main"));
    }

    [StaFact]
    public void SameNamedHostsRemainIndependent()
    {
        var first = new ContentControl();
        var second = new ContentControl();
        var firstDefault = new TestView();
        var secondDefault = new TestView();
        RegionManager.SetDefaultView(first, firstDefault);
        RegionManager.SetDefaultView(second, secondDefault);
        var firstRegion = CreateAdapter().Initialize(first, "Main");
        var secondRegion = CreateAdapter().Initialize(second, "Main");
        new RegionManager().Regions.Add(firstRegion);
        new RegionManager().Regions.Add(secondRegion);
        Assert.Same(firstDefault, Assert.Single(firstRegion.Views));
        Assert.Same(secondDefault, Assert.Single(secondRegion.Views));
        Assert.Empty(registry.GetContents("Main"));
    }

    [StaFact]
    public void DiscoveryKeepsOwnershipAndAnAlreadyAddedInstanceIsNotAddedTwice()
    {
        var view = new TestView();
        registry.RegisterViewWithRegion("Main", () => view);
        var host = new ContentControl();
        RegionManager.SetDefaultView(host, view);
        var region = CreateAdapter().Initialize(host, "Main");
        Assert.Same(view, Assert.Single(region.Views));
        Assert.Same(view, Assert.Single(registry.GetContents("Main")));
    }

    [StaFact]
    public void DistinctInstancesThatCompareEqualAreBothAdded()
    {
        var discovered = new EqualView();
        var declared = new EqualView();
        registry.RegisterViewWithRegion("Main", () => discovered);
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
        var view = new TestView();
        container.Setup(c => c.Resolve(typeof(TestView))).Returns(view);
        registry.RegisterViewWithRegion("Main", () => view);
        var host = new ContentControl();
        RegionManager.SetDefaultView(host, "Default");
        var region = CreateAdapter().Initialize(host, "Main");
        Assert.Same(view, Assert.Single(region.Views));
        Assert.Null(region.GetView("Default"));
    }

    [StaFact]
    public void DefaultDoesNotDisplaceDiscoveredContent()
    {
        var discovered = new TestView();
        registry.RegisterViewWithRegion("Main", () => discovered);
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
        RegionManager.SetDefaultView(host, new TestView());
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
        Assert.Empty(registry.GetContents("Main"));
    }

    [StaFact]
    public void UnregisteredTypeUsesExistingDesktopContainerFallback()
    {
        var host = new ContentControl();
        container.Setup(c => c.Resolve(typeof(OtherView))).Returns(() => new OtherView());
        RegionManager.SetDefaultView(host, typeof(OtherView));
        var region = CreateAdapter().Initialize(host, "Main");
        Assert.IsType<OtherView>(Assert.Single(region.Views));
    }

    private static object Declaration(string form) => form switch
    {
        "name" => "Default",
        "type" => typeof(TestView),
        _ => new TestView()
    };

    private ContentControlRegionAdapter CreateAdapter()
    {
        var behaviors = new RegionBehaviorFactory(container.Object);
        behaviors.AddIfMissing<AutoPopulateRegionBehavior>(AutoPopulateRegionBehavior.BehaviorKey);
        return new ContentControlRegionAdapter(behaviors);
    }

    public class EqualView
    {
        public override bool Equals(object obj) => obj is EqualView;
        public override int GetHashCode() => 0;
    }

    public class TestView : ContentControl { }
    public class OtherView : ContentControl { }
}
