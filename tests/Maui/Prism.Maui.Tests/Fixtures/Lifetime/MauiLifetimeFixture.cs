using System.Runtime.CompilerServices;
using Microsoft.Maui.Dispatching;
using Moq;
using Prism.Behaviors;
using Prism.Common;
using Prism.Maui.Tests.Mocks;
using Prism.Navigation.Xaml;
using Prism.Tests.Lifetime;

namespace Prism.Maui.Tests.Fixtures.Lifetime;

public class MauiLifetimeFixture
{
    [Fact]
    public void DeferredDestroyOwnsPageUntilDispatcherDrainedThenReleasesGraph()
    {
        var dispatcher = new QueuedDispatcher();
        DispatcherProvider.SetCurrent(dispatcher);
        var root = Mock.Of<IContainerProvider>();
        try
        {
            for (var i = 0; i < 16; i++)
            {
                var disposed = new DisposalProbe();
                var references = CreateAndDestroy(disposed);
                CollectionProbe.Collect();
                foreach (var reference in references) Assert.True(reference.IsAlive);
                Assert.Single(dispatcher.Pending);
                dispatcher.Drain();
                Assert.Equal(1, disposed.Calls);
                foreach (var reference in references) CollectionProbe.Collected(reference);
            }
            GC.KeepAlive(root);
            GC.KeepAlive(dispatcher);
        }
        finally { DispatcherProvider.SetCurrent(TestDispatcher.Provider); }
    }

    [Fact]
    public void DetachReleasesBehaviorAndCallbackOwnerWithAncestorPublisherAlive()
    {
        DispatcherProvider.SetCurrent(TestDispatcher.Provider);
        var publisher = new ContentPage();
        var root = Mock.Of<IContainerProvider>();
        for (var i = 0; i < 16; i++)
        {
            foreach (var reference in AttachAndDetach(publisher)) CollectionProbe.Collected(reference);
            publisher.Title = "Late change " + i;
        }
        publisher.SetContainerProvider(root);
        GC.KeepAlive(publisher);
        GC.KeepAlive(root);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference[] CreateAndDestroy(DisposalProbe disposed)
    {
        var owner = new CallbackOwner();
        var page = new ContentPage { BindingContext = owner };
        var scope = new Mock<IScopedProvider>();
        scope.Setup(p => p.Resolve(typeof(IPageAccessor))).Returns(new PageAccessor());
        scope.Setup(p => p.Dispose()).Callback(() => disposed.Calls++);
        page.SetContainerProvider(scope.Object);
        page.Behaviors.Add(new PageScopeBehavior());
        MvvmHelpers.DestroyPage(page);
        return new[] { new WeakReference(page), new WeakReference(owner), new WeakReference(scope.Object) };
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference[] AttachAndDetach(ContentPage publisher)
    {
        var owner = new CallbackOwner();
        var host = new ContentView();
        var behavior = new ElementParentedCallbackBehavior(owner.Callback);
        host.Behaviors.Add(behavior);
        publisher.Content = host;
        host.Behaviors.Remove(behavior);
        publisher.Content = null;
        return new[] { new WeakReference(host), new WeakReference(behavior), new WeakReference(owner) };
    }

    private sealed class DisposalProbe { public int Calls; }

    private sealed class CallbackOwner
    {
        private int _calls;
        public void Callback() => _calls++;
    }

    private sealed class QueuedDispatcher : IDispatcher, IDispatcherProvider
    {
        internal Queue<Action> Pending { get; } = new();
        public bool IsDispatchRequired => false;
        public IDispatcher GetForCurrentThread() => this;
        public bool Dispatch(Action action) { Pending.Enqueue(action); return true; }
        public bool DispatchDelayed(TimeSpan delay, Action action)
        {
            Assert.Equal(TimeSpan.FromMilliseconds(800), delay);
            Pending.Enqueue(action);
            return true;
        }
        public IDispatcherTimer CreateTimer() => throw new NotSupportedException();
        [MethodImpl(MethodImplOptions.NoInlining)]
        internal void Drain() { while (Pending.Count > 0) Pending.Dequeue()(); }
    }
}
