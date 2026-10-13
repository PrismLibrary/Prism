using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using Prism.Events;
using Xunit;

namespace Prism.Tests.Lifetime;

public class EventLifetimeFixture
{
    [Fact]
    public void RetainedAggregatorAllowsWeakActionAndFilterTargetsToCollectAcrossChurn()
    {
        var publisher = new EventAggregator();
        for (var i = 0; i < 32; i++)
        {
            var references = SubscribeWeak(publisher.GetEvent<PubSubEvent<int>>());
            CollectionProbe.Collected(references.Item1);
            CollectionProbe.Collected(references.Item2);
            publisher.GetEvent<PubSubEvent<int>>().Publish(i);
        }
        GC.KeepAlive(publisher);
    }

    [Fact]
    public void StrongSubscriptionOwnsSubscriberUntilTokenDisposed()
    {
        var publisher = new PubSubEvent();
        var pair = SubscribeStrong(publisher);
        CollectionProbe.Collect();
        Assert.True(pair.Item1.IsAlive); // Positive control: this ownership is requested.
        pair.Item2.Dispose();
        pair.Item2.Dispose();
        CollectionProbe.Collected(pair.Item1);
        publisher.Publish();
        GC.KeepAlive(publisher);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void DelegateReferenceHonorsRequestedOwnership(bool strong)
    {
        var pair = CreateReference(strong);
        CollectionProbe.Collect();
        Assert.Equal(strong, pair.Item1.IsAlive);
        Assert.Equal(strong, pair.Item2.Target != null);
        GC.KeepAlive(pair.Item2);
    }

    [Fact]
    public void QueuedUiDeliveryOwnsSubscriberUntilQueueDrained()
    {
        var queue = new QueuedContext();
        var publisher = new PubSubEvent { SynchronizationContext = queue };
        var reference = PublishQueued(publisher);
        CollectionProbe.Collect();
        Assert.True(reference.IsAlive); // The pending callback intentionally owns the delegate.
        Assert.Single(queue.Items);
        queue.Drain();
        CollectionProbe.Collected(reference);
        publisher.Publish();
        GC.KeepAlive(publisher);
        GC.KeepAlive(queue);
    }

    [Fact]
    public void WeakManagerCollectsTargetsAndPrunesWrappersOnRaise()
    {
        var manager = new WeakDelegatesManager();
        for (var i = 0; i < 32; i++)
            CollectionProbe.Collected(AddWeak(manager));
        Assert.Equal(32, ListenerCount(manager)); // Bookkeeping, not retained subscriber graphs.
        manager.Raise();
        Assert.Equal(0, ListenerCount(manager));
        GC.KeepAlive(manager);
    }

    [Fact]
    public void WeakManagerRemoveAlsoPrunesDeadWrappers()
    {
        var manager = new WeakDelegatesManager();
        CollectionProbe.Collected(AddWeak(manager));
        manager.RemoveListener((Action)(() => { }));
        Assert.Equal(0, ListenerCount(manager));
        GC.KeepAlive(manager);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Tuple<WeakReference, WeakReference> SubscribeWeak(PubSubEvent<int> publisher)
    {
        var subscriber = new Subscriber();
        var filter = new Subscriber();
        publisher.Subscribe(subscriber.Receive, ThreadOption.PublisherThread, false, filter.Filter);
        return Tuple.Create(new WeakReference(subscriber), new WeakReference(filter));
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Tuple<WeakReference, SubscriptionToken> SubscribeStrong(PubSubEvent publisher)
    {
        var subscriber = new Subscriber();
        return Tuple.Create(new WeakReference(subscriber), publisher.Subscribe(subscriber.Receive, true));
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Tuple<WeakReference, DelegateReference> CreateReference(bool strong)
    {
        var subscriber = new Subscriber();
        return Tuple.Create(new WeakReference(subscriber), new DelegateReference((Action)subscriber.Receive, strong));
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference PublishQueued(PubSubEvent publisher)
    {
        var subscriber = new Subscriber();
        publisher.Subscribe(subscriber.Receive, ThreadOption.UIThread, false);
        publisher.Publish();
        return new WeakReference(subscriber);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference AddWeak(WeakDelegatesManager manager)
    {
        var subscriber = new Subscriber();
        manager.AddListener((Action)subscriber.Receive);
        return new WeakReference(subscriber);
    }

    private static int ListenerCount(WeakDelegatesManager manager) =>
        ((ICollection)typeof(WeakDelegatesManager).GetField("_listeners", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(manager)!).Count;

    private sealed class Subscriber
    {
        public int Calls;
        public void Receive() => Calls++;
        public void Receive(int value) => Calls += value;
        public bool Filter(int value) => value >= 0;
    }

    private sealed class QueuedContext : SynchronizationContext
    {
        internal Queue<Action> Items { get; } = new();
        public override void Post(SendOrPostCallback callback, object state) => Items.Enqueue(() => callback(state));
        [MethodImpl(MethodImplOptions.NoInlining)]
        internal void Drain()
        {
            while (Items.Count > 0) Items.Dequeue()();
        }
    }
}
