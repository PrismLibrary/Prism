using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Moq;
using Prism.Ioc;
using Prism.Navigation.Regions;
using Prism.Navigation.Regions.Behaviors;
using Xunit;
#if AVALONIA
using Avalonia.Controls;

#elif UNO_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using System.Windows;
using System.Windows.Controls;
#endif

namespace Prism.Tests.Lifetime;

public class RegionLifetimeFixture
{
    [StaFact]
    public void RetainedManagerAndRegistryAllowRemovedHostGraphToCollectAcrossRecreation()
    {
        var container = Mock.Of<IContainerExtension>();
        ContainerLocator.SetContainerExtension(container);
        var manager = new RegionManager();
        var registry = new RegionViewRegistry(container);
        registry.RegisterViewWithRegion("Content", _ => new ContentControl { DataContext = new object() });
        for (var i = 0; i < 16; i++)
        {
            foreach (var reference in CreateAndRemove(manager, registry)) CollectionProbe.Collected(reference);
            Assert.Empty(manager.Regions);
        }
        GC.KeepAlive(manager);
        GC.KeepAlive(registry);
        GC.KeepAlive(container);
    }

    [StaFact]
    public void UnnamedRegionAndPropertyChangedCycleAreCollectibleWithRegistryAlive()
    {
        var registry = new RegionViewRegistry(Mock.Of<IContainerExtension>());
        foreach (var reference in AbandonUnnamed(registry)) CollectionProbe.Collected(reference);
        registry.RegisterViewWithRegion("Later", _ => new object());
        GC.KeepAlive(registry);
    }

    [StaFact]
    public void RetainedHostIntentionallyOwnsItsRegion()
    {
        var pair = CreateRetainedHost();
        CollectionProbe.Collect();
        Assert.True(pair.Item2.IsAlive);
        GC.KeepAlive(pair.Item1);
    }

    [StaFact]
    [Trait("Lifetime", "KnownRetention")]
    public void AbandonedBareHostShouldReleaseDelayedBehaviorAfterRealUpdate()
    {
        var references = new List<WeakReference>();
        try
        {
            for (var i = 0; i < 16; i++)
            {
                var pair = AbandonBareHost();
                references.Add(pair.Item2);
                CollectionProbe.Collected(pair.Item1);
                RegionManager.UpdateRegions();
                CollectionProbe.Collected(pair.Item2);
            }
        }
        finally
        {
            // Remove only this fixture's entries, preserving unrelated global state.
            var tracker = (ICollection<DelayedRegionCreationBehavior>)typeof(DelayedRegionCreationBehavior)
                .GetField("_instanceTracker", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;
            foreach (var reference in references)
                if (reference.Target is DelayedRegionCreationBehavior behavior) tracker.Remove(behavior);
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference[] CreateAndRemove(RegionManager manager, RegionViewRegistry registry)
    {
        var host = new ContentControl();
        var region = new ContentControlRegionAdapter(null).Initialize(host, "Content");
        manager.Regions.Add(region);
        // All default behaviors, on the actual adapter and host, in initialization order.
#if AVALONIA
        Add(region, host, BindRegionContextToAvaloniaObjectBehavior.BehaviorKey, new BindRegionContextToAvaloniaObjectBehavior());
#else
        Add(region, host, BindRegionContextToDependencyObjectBehavior.BehaviorKey, new BindRegionContextToDependencyObjectBehavior());
#endif
        Add(region, host, RegionActiveAwareBehavior.BehaviorKey, new RegionActiveAwareBehavior());
        Add(region, host, SyncRegionContextWithHostBehavior.BehaviorKey, new SyncRegionContextWithHostBehavior());
        Add(region, host, RegionManagerRegistrationBehavior.BehaviorKey, new RegionManagerRegistrationBehavior());
        Add(region, host, RegionMemberLifetimeBehavior.BehaviorKey, new RegionMemberLifetimeBehavior());
        Add(region, host, ClearChildViewsRegionBehavior.BehaviorKey, new ClearChildViewsRegionBehavior());
        Add(region, host, AutoPopulateRegionBehavior.BehaviorKey, new AutoPopulateRegionBehavior(registry));
        Add(region, host, DestructibleRegionBehavior.BehaviorKey, new DestructibleRegionBehavior());
        var view = region.Views.Single();
        var model = ((ContentControl)view).DataContext;
        Assert.Same(view, host.Content);
        region.Remove(view);
        Assert.Null(host.Content);
        manager.Regions.Remove("Content");
        return new[] { new WeakReference(host), new WeakReference(region), new WeakReference(view), new WeakReference(model) };
    }

    private static void Add(IRegion region, DependencyObject host, string key, IRegionBehavior behavior)
    {
        if (behavior is IHostAwareRegionBehavior hostAware) hostAware.HostControl = host;
        region.Behaviors.Add(key, behavior);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference[] AbandonUnnamed(RegionViewRegistry registry)
    {
        var region = new Region();
        var behavior = new AutoPopulateRegionBehavior(registry);
        region.Behaviors.Add(AutoPopulateRegionBehavior.BehaviorKey, behavior);
        return new[] { new WeakReference(region), new WeakReference(behavior) };
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Tuple<ContentControl, WeakReference> CreateRetainedHost()
    {
        var host = new ContentControl();
        var region = new ContentControlRegionAdapter(null).Initialize(host, "Content");
        return Tuple.Create(host, new WeakReference(region));
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Tuple<WeakReference, WeakReference> AbandonBareHost()
    {
#if UNO_WINUI
        var host = new Prism.Interactivity.InvokeCommandAction();
#else
        var host = new DependencyObject();
#endif
        var behavior = new DelayedRegionCreationBehavior(new RegionAdapterMappings()) { TargetElement = host };
        behavior.Attach();
        return Tuple.Create(new WeakReference(host), new WeakReference(behavior));
    }
}
