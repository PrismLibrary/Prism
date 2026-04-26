#if NET8_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Prism.Events;
using Xunit;
using static Prism.Tests.Events.PubSubEventFixture;

namespace Prism.Tests.Events
{
    public class EventStreamFixture
    {
        // ── Basic subscribe / publish ────────────────────────────────────────────

        [Fact]
        public void CanSubscribeAndPublishEvent()
        {
            var stream = new TestableEventStream<string>();
            bool received = false;
            stream.Subscribe(_ => received = true, null);

            stream.Publish("hello");

            Assert.True(received);
        }

        [Fact]
        public void CanHaveMultipleSubscribersAndPublish()
        {
            var stream = new TestableEventStream<string>();
            var results = new List<string>();
            stream.Subscribe(v => results.Add("a:" + v), null);
            stream.Subscribe(v => results.Add("b:" + v), null);

            stream.Publish("x");

            Assert.Contains("a:x", results);
            Assert.Contains("b:x", results);
        }

        // ── Backlog replay on subscribe ──────────────────────────────────────────

        [Fact]
        public void NewSubscriberReceivesBacklogOnSubscribe()
        {
            var stream = new TestableEventStream<string>();
            stream.Publish("first");
            stream.Publish("second");

            var backlog = new List<string>();
            stream.Subscribe(_ => { }, v => backlog.Add(v));

            Assert.Equal(new[] { "first", "second" }, backlog);
        }

        [Fact]
        public void BacklogActionIsNullSafeAndDoesNotThrow()
        {
            var stream = new TestableEventStream<string>();
            stream.Publish("item");

            // Should not throw when backlogAction is null
            var ex = Record.Exception(() => stream.Subscribe(_ => { }, null));
            Assert.Null(ex);
        }

        [Fact]
        public void BacklogIsEmptyForFreshStream()
        {
            var stream = new TestableEventStream<string>();
            var backlog = new List<string>();
            stream.Subscribe(_ => { }, v => backlog.Add(v));

            Assert.Empty(backlog);
        }

        [Fact]
        public void BacklogDoesNotIncludeEventsPublishedAfterSubscription()
        {
            var stream = new TestableEventStream<string>();
            stream.Publish("before");

            var backlog = new List<string>();
            var live = new List<string>();
            stream.Subscribe(v => live.Add(v), v => backlog.Add(v));

            stream.Publish("after");

            Assert.Equal(new[] { "before" }, backlog);
            Assert.Equal(new[] { "after" }, live);
        }

        // ── Rolling ring-buffer eviction ─────────────────────────────────────────

        [Fact]
        public void BacklogEvictsOldestWhenCapacityExceeded()
        {
            // Default capacity is 10; publish 12 items → only last 10 replayed
            var stream = new TestableEventStream<int>();
            for (int i = 1; i <= 12; i++)
                stream.Publish(i);

            var backlog = new List<int>();
            stream.Subscribe(_ => { }, v => backlog.Add(v));

            Assert.Equal(10, backlog.Count);
            Assert.Equal(Enumerable.Range(3, 10), backlog);
        }

        [Fact]
        public void CustomBacklogSizeIsRespected()
        {
            var stream = new SmallBacklogEventStream<int>(); // capacity = 3
            for (int i = 1; i <= 5; i++)
                stream.Publish(i);

            var backlog = new List<int>();
            stream.Subscribe(_ => { }, v => backlog.Add(v));

            Assert.Equal(3, backlog.Count);
            Assert.Equal(new[] { 3, 4, 5 }, backlog);
        }

        // ── Filter ───────────────────────────────────────────────────────────────

        [Fact]
        public void FilterPreventsUnmatchedPayloadsFromBeingDelivered()
        {
            var stream = new TestableEventStream<string>();
            var received = new List<string>();
            stream.Subscribe(
                v => received.Add(v),
                null,
                ThreadOption.PublisherThread,
                true,
                v => v.StartsWith("keep"));

            stream.Publish("keep-this");
            stream.Publish("drop-this");

            Assert.Equal(new[] { "keep-this" }, received);
        }

        [Fact]
        public void FilterIsAppliedToBacklogReplay()
        {
            var stream = new TestableEventStream<string>();
            stream.Publish("keep-1");
            stream.Publish("drop-1");
            stream.Publish("keep-2");

            var backlog = new List<string>();
            stream.Subscribe(
                _ => { },
                v => backlog.Add(v),
                ThreadOption.PublisherThread,
                true,
                v => v.StartsWith("keep"));

            Assert.Equal(new[] { "keep-1", "keep-2" }, backlog);
        }

        // ── Unsubscribe ───────────────────────────────────────────────────────────

        [Fact]
        public void UnsubscribeByDelegateStopsDelivery()
        {
            var stream = new TestableEventStream<string>();
            var received = new List<string>();
            Action<string> handler = v => received.Add(v);
            stream.Subscribe(handler, null, ThreadOption.PublisherThread, true);

            stream.Publish("before");
            stream.Unsubscribe(handler);
            stream.Publish("after");

            Assert.Equal(new[] { "before" }, received);
        }

        [Fact]
        public void UnsubscribeByTokenStopsDelivery()
        {
            var stream = new TestableEventStream<string>();
            var received = new List<string>();
            var token = stream.Subscribe(v => received.Add(v), null, ThreadOption.PublisherThread, true);

            stream.Publish("before");
            stream.Unsubscribe(token);
            stream.Publish("after");

            Assert.Equal(new[] { "before" }, received);
        }

        [Fact]
        public void UnsubscribeNonSubscriberDoesNotThrow()
        {
            var stream = new TestableEventStream<string>();
            Action<string> stranger = _ => { };

            var ex = Record.Exception(() => stream.Unsubscribe(stranger));
            Assert.Null(ex);
        }

        [Fact]
        public void UnsubscribeRemovesOnlyFirstMatchingDelegate()
        {
            var stream = new TestableEventStream<string>();
            int callCount = 0;
            Action<string> handler = _ => callCount++;
            stream.Subscribe(handler, null, ThreadOption.PublisherThread, true);
            stream.Subscribe(handler, null, ThreadOption.PublisherThread, true);

            stream.Publish("x");
            Assert.Equal(2, callCount);

            callCount = 0;
            stream.Unsubscribe(handler);
            stream.Publish("x");
            Assert.Equal(1, callCount);
        }

        // ── Thread options ────────────────────────────────────────────────────────

        [Fact]
        public void SubscribeOnPublisherThreadCreatesEventSubscription()
        {
            var stream = new TestableEventStream<string>();
            stream.Subscribe(_ => { }, null, ThreadOption.PublisherThread, true);

            Assert.Equal(
                typeof(EventSubscription<string>),
                stream.BaseSubscriptions.Single().GetType());
        }

        [Fact]
        public void SubscribeOnBackgroundThreadCreatesBackgroundEventSubscription()
        {
            var stream = new TestableEventStream<string>();
            stream.Subscribe(_ => { }, null, ThreadOption.BackgroundThread, true);

            Assert.Equal(
                typeof(BackgroundEventSubscription<string>),
                stream.BaseSubscriptions.Single().GetType());
        }

        [Fact]
        public void SubscribeOnUIThreadThrowsWhenSynchronizationContextIsNull()
        {
            var stream = new TestableEventStream<string>();

            Assert.Throws<InvalidOperationException>(
                () => stream.Subscribe(_ => { }, null, ThreadOption.UIThread, true));
        }

        [Fact]
        public void SubscribeOnUIThreadCreatesDispatcherEventSubscription()
        {
            var stream = new TestableEventStream<string>();
            stream.SynchronizationContext = new SynchronizationContext();

            stream.Subscribe(_ => { }, null, ThreadOption.UIThread, true);

            Assert.Equal(
                typeof(DispatcherEventSubscription<string>),
                stream.BaseSubscriptions.Single().GetType());
        }

        // ── Weak references / GC ─────────────────────────────────────────────────

        [Fact]
        public async Task DeadWeakReferenceSubscriptionIsPrunedOnPublish()
        {
            var stream = new TestableEventStream<string>();
            SubscribeWithoutKeepAlive(stream);

            await Task.Delay(100);
            GC.Collect();

            stream.Publish("trigger");

            Assert.Empty(stream.BaseSubscriptions);
        }

        [Fact]
        public void StrongReferenceSubscriberIsNotCollected()
        {
            var stream = new TestableEventStream<string>();
            var action = new ExternalAction();
            stream.Subscribe(action.ExecuteAction, null, ThreadOption.PublisherThread, true);

            var weakRef = new WeakReference(action);
            action = null;
            GC.Collect();
            GC.Collect();

            Assert.True(weakRef.IsAlive);
            stream.Publish("test");
            Assert.Equal("test", ((ExternalAction)weakRef.Target).PassedValue);
        }

        // ── Subscription token / Contains ────────────────────────────────────────

        [Fact]
        public void ContainsByDelegateReturnsTrueAfterSubscribe()
        {
            var stream = new TestableEventStream<string>();
            Action<string> handler = _ => { };
            stream.Subscribe(handler, null, ThreadOption.PublisherThread, true);

            Assert.True(stream.Contains(handler));
        }

        [Fact]
        public void ContainsByTokenReturnsTrueAfterSubscribeAndFalseAfterUnsubscribe()
        {
            var stream = new TestableEventStream<string>();
            var token = stream.Subscribe(_ => { }, null, ThreadOption.PublisherThread, true);

            Assert.True(stream.Contains(token));

            stream.Unsubscribe(token);
            Assert.False(stream.Contains(token));
        }

        // ── Concurrent subscribe while publishing ─────────────────────────────────

        [Fact]
        public void CanAddSubscriptionWhileEventIsFiring()
        {
            var stream = new TestableEventStream<string>();
            var lateHandler = new ActionHelper();
            var earlyHandler = new ActionHelper
            {
                ActionToExecute = () => stream.Subscribe(lateHandler.Action, null, ThreadOption.PublisherThread, true)
            };

            stream.Subscribe(earlyHandler.Action, null, ThreadOption.PublisherThread, true);
            Assert.False(stream.Contains(lateHandler.Action));

            stream.Publish("fire");

            Assert.True(stream.Contains(lateHandler.Action));
        }

        // ── Helpers ───────────────────────────────────────────────────────────────

        [MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        private static void SubscribeWithoutKeepAlive(TestableEventStream<string> stream)
        {
            stream.Subscribe(new ExternalAction().ExecuteAction, null);
        }

        [Fact]
        public void EnsureSubscriptionListIsEmptyAfterPublishingAMessage()
        {
            var pubSubEvent = new TestableEventStream<string>();
            SubscribeExternalActionWithoutReference(pubSubEvent);
            GC.Collect();
            pubSubEvent.Publish("testPayload");
            Assert.True(pubSubEvent.BaseSubscriptions.Count == 0, "Subscriptionlist is not empty");
        }

        [Fact]
        public void EnsureSubscriptionListIsNotEmptyWithoutPublishOrSubscribe()
        {
            var pubSubEvent = new TestableEventStream<string>();
            SubscribeExternalActionWithoutReference(pubSubEvent);
            GC.Collect();
            Assert.True(pubSubEvent.BaseSubscriptions.Count == 1, "Subscriptionlist is empty");
        }

        [Fact]
        public void EnsureSubscriptionListIsEmptyAfterSubscribeAgainAMessage()
        {
            var pubSubEvent = new TestableEventStream<string>();
            SubscribeExternalActionWithoutReference(pubSubEvent);
            GC.Collect();
            SubscribeExternalActionWithoutReference(pubSubEvent);
            pubSubEvent.Prune();
            Assert.True(pubSubEvent.BaseSubscriptions.Count == 1, "Subscriptionlist is empty");
        }

        private static void SubscribeExternalActionWithoutReference(TestableEventStream<string> pubSubEvent)
        {
            var externalAction = new ExternalAction();
            pubSubEvent.Subscribe(externalAction.ExecuteAction, externalAction.ExecuteActionBacklog);
        }


        [Fact]
        public void CanSubscribeAndRaiseEvent()
        {
            TestableEventStream<string> pubSubEvent = new TestableEventStream<string>();
            bool published = false;
            pubSubEvent.Subscribe(delegate { published = true; },null, ThreadOption.PublisherThread, true, delegate { return true; });
            pubSubEvent.Publish(null);

            Assert.True(published);
        }

        [Fact]
        public void CanSubscribeAndRaiseCustomEvent()
        {
            var customEvent = new TestableEventStream<Payload>();
            Payload payload = new Payload();
            var action = new ActionHelper();
            customEvent.Subscribe(action.Action, action.Action);

            customEvent.Publish(payload);

            Assert.Same(action.ActionArg<Payload>(), payload);
        }

        [Fact]
        public void CanHaveMultipleSubscribersAndRaiseCustomEvent()
        {
            var customEvent = new TestableEventStream<Payload>();
            Payload payload = new Payload();
            var action1 = new ActionHelper();
            var action2 = new ActionHelper();
            customEvent.Subscribe(action1.Action, action1.Action);
            customEvent.Subscribe(action2.Action, action2.Action);

            customEvent.Publish(payload);

            Assert.Same(action1.ActionArg<Payload>(), payload);
            Assert.Same(action2.ActionArg<Payload>(), payload);
        }


        [Fact]
        public void SubscribeTakesExecuteDelegateThreadOptionAndFilter()
        {
            TestableEventStream<string> pubSubEvent = new TestableEventStream<string>();
            var action = new ActionHelper();
            pubSubEvent.Subscribe(action.Action, action.Action);

            pubSubEvent.Publish("test");

            Assert.Equal("test", action.ActionArg<string>());

        }

        [Fact]
        public void FilterEnablesActionTarget()
        {
            TestableEventStream<string> pubSubEvent = new TestableEventStream<string>();
            var goodFilter = new MockFilter { FilterReturnValue = true };
            var actionGoodFilter = new ActionHelper();
            var badFilter = new MockFilter { FilterReturnValue = false };
            var actionBadFilter = new ActionHelper();
            pubSubEvent.Subscribe(actionGoodFilter.Action, actionGoodFilter.Action, ThreadOption.PublisherThread, true, goodFilter.FilterString);
            pubSubEvent.Subscribe(actionBadFilter.Action, actionBadFilter.Action, ThreadOption.PublisherThread, true, badFilter.FilterString);

            pubSubEvent.Publish("test");

            Assert.True(actionGoodFilter.ActionCalled);
            Assert.False(actionBadFilter.ActionCalled);

        }

        [Fact]
        public void FilterEnablesActionTarget_Weak()
        {
            TestableEventStream<string> pubSubEvent = new TestableEventStream<string>();
            var goodFilter = new MockFilter { FilterReturnValue = true };
            var actionGoodFilter = new ActionHelper();
            var badFilter = new MockFilter { FilterReturnValue = false };
            var actionBadFilter = new ActionHelper();
            pubSubEvent.Subscribe(actionGoodFilter.Action, actionGoodFilter.Action, ThreadOption.PublisherThread, false, goodFilter.FilterString);
            pubSubEvent.Subscribe(actionBadFilter.Action, actionBadFilter.Action, ThreadOption.PublisherThread, false, badFilter.FilterString);

            pubSubEvent.Publish("test");

            Assert.True(actionGoodFilter.ActionCalled);
            Assert.False(actionBadFilter.ActionCalled);

        }

        [Fact]
        public void SubscribeDefaultsThreadOptionAndNoFilter()
        {
            TestableEventStream<string> pubSubEvent = new TestableEventStream<string>();
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
            SynchronizationContext calledSyncContext = null;
            var myAction = new ActionHelper()
            {
                ActionToExecute =
                    () => calledSyncContext = SynchronizationContext.Current
            };
            pubSubEvent.Subscribe(myAction.Action, myAction.Action);

            pubSubEvent.Publish("test");

            Assert.Equal(SynchronizationContext.Current, calledSyncContext);
        }


        [Fact]
        public void ShouldUnsubscribeFromPublisherThread()
        {
            var PubSubEvent = new TestableEventStream<string>();

            var actionEvent = new ActionHelper();
            PubSubEvent.Subscribe(
                actionEvent.Action,
                actionEvent.Action,
                ThreadOption.PublisherThread);

            Assert.True(PubSubEvent.Contains(actionEvent.Action));
            PubSubEvent.Unsubscribe(actionEvent.Action);
            Assert.False(PubSubEvent.Contains(actionEvent.Action));
        }

        [Fact]
        public void UnsubscribeShouldNotFailWithNonSubscriber()
        {
            TestableEventStream<string> pubSubEvent = new TestableEventStream<string>();

            Action<string> subscriber = delegate { };
            pubSubEvent.Unsubscribe(subscriber);
        }

        [Fact]
        public void ShouldUnsubscribeFromBackgroundThread()
        {
            var PubSubEvent = new TestableEventStream<string>();

            var actionEvent = new ActionHelper();
            PubSubEvent.Subscribe(
                actionEvent.Action,
                actionEvent.Action,
                ThreadOption.BackgroundThread);

            Assert.True(PubSubEvent.Contains(actionEvent.Action));
            PubSubEvent.Unsubscribe(actionEvent.Action);
            Assert.False(PubSubEvent.Contains(actionEvent.Action));
        }

        [Fact]
        public void ShouldUnsubscribeFromUIThread()
        {
            var PubSubEvent = new TestableEventStream<string>();
            PubSubEvent.SynchronizationContext = new SynchronizationContext();

            var actionEvent = new ActionHelper();
            PubSubEvent.Subscribe(
                actionEvent.Action,
                actionEvent.Action,
                ThreadOption.UIThread);

            Assert.True(PubSubEvent.Contains(actionEvent.Action));
            PubSubEvent.Unsubscribe(actionEvent.Action);
            Assert.False(PubSubEvent.Contains(actionEvent.Action));
        }

        [Fact]
        public void ShouldUnsubscribeASingleDelegate()
        {
            var PubSubEvent = new TestableEventStream<string>();

            int callCount = 0;

            var actionEvent = new ActionHelper() { ActionToExecute = () => callCount++ };
            PubSubEvent.Subscribe(actionEvent.Action, actionEvent.Action);
            PubSubEvent.Subscribe(actionEvent.Action, actionEvent.Action);

            PubSubEvent.Publish(null);
            Assert.Equal<int>(2, callCount);

            callCount = 0;
            PubSubEvent.Unsubscribe(actionEvent.Action);
            PubSubEvent.Publish(null);
            Assert.Equal<int>(1, callCount);
        }


        [Fact]
        public async Task ShouldNotExecuteOnGarbageCollectedDelegateReferenceWhenNotKeepAlive()
        {
            var PubSubEvent = new TestableEventStream<string>();

            ExternalAction externalAction = new ExternalAction();
            PubSubEvent.Subscribe(externalAction.ExecuteAction, externalAction.ExecuteAction);

            PubSubEvent.Publish("testPayload");
            Assert.Equal("testPayload", externalAction.PassedValue);

            WeakReference actionEventReference = new WeakReference(externalAction);
            externalAction = null;
            await Task.Delay(100);
            GC.Collect();
            Assert.False(actionEventReference.IsAlive);

            PubSubEvent.Publish("testPayload");
        }


        [Fact]
        public async Task ShouldNotExecuteOnGarbageCollectedFilterReferenceWhenNotKeepAlive()
        {
            var PubSubEvent = new TestableEventStream<string>();

            bool wasCalled = false;
            var actionEvent = new ActionHelper() { ActionToExecute = () => wasCalled = true };

            ExternalFilter filter = new ExternalFilter();
            PubSubEvent.Subscribe(actionEvent.Action, actionEvent.Action, ThreadOption.PublisherThread, false, filter.AlwaysTrueFilter);

            PubSubEvent.Publish("testPayload");
            Assert.True(wasCalled);

            wasCalled = false;
            WeakReference filterReference = new WeakReference(filter);
            filter = null;
            await Task.Delay(100);
            GC.Collect();
            Assert.False(filterReference.IsAlive);

            PubSubEvent.Publish("testPayload");
            Assert.False(wasCalled);
        }


        [Fact]
        public void InlineDelegateDeclarationsDoesNotGetCollectedIncorrectlyWithWeakReferences()
        {
            var PubSubEvent = new TestableEventStream<string>();
            bool published = false;
            PubSubEvent.Subscribe(delegate { published = true; }, null, ThreadOption.PublisherThread, false, delegate { return true; });
            GC.Collect();
            PubSubEvent.Publish(null);

            Assert.True(published);
        }

        [Fact]
        public void ShouldNotGarbageCollectDelegateReferenceWhenUsingKeepAlive()
        {
            var PubSubEvent = new TestableEventStream<string>();

            var externalAction = new ExternalAction();
            PubSubEvent.Subscribe(externalAction.ExecuteAction, null, ThreadOption.PublisherThread, true);

            WeakReference actionEventReference = new WeakReference(externalAction);
            externalAction = null;
            GC.Collect();
            GC.Collect();
            Assert.True(actionEventReference.IsAlive);

            PubSubEvent.Publish("testPayload");

            Assert.Equal("testPayload", ((ExternalAction)actionEventReference.Target).PassedValue);
        }

        [Fact]
        public void RegisterReturnsTokenThatCanBeUsedToUnsubscribe()
        {
            var PubSubEvent = new TestableEventStream<string>();
            var emptyAction = new ActionHelper();

            var token = PubSubEvent.Subscribe(emptyAction.Action, null);
            PubSubEvent.Unsubscribe(token);

            Assert.False(PubSubEvent.Contains(emptyAction.Action));
        }

        [Fact]
        public void ContainsShouldSearchByToken()
        {
            var PubSubEvent = new TestableEventStream<string>();
            var emptyAction = new ActionHelper();
            var token = PubSubEvent.Subscribe(emptyAction.Action, null);

            Assert.True(PubSubEvent.Contains(token));

            PubSubEvent.Unsubscribe(emptyAction.Action);
            Assert.False(PubSubEvent.Contains(token));
        }

        [Fact]
        public void SubscribeDefaultsToPublisherThread()
        {
            var PubSubEvent = new TestableEventStream<string>();
            Action<string> action = delegate { };
            var token = PubSubEvent.Subscribe(action, action, ThreadOption.PublisherThread, true);

            Assert.Single(PubSubEvent.BaseSubscriptions);
            Assert.Equal(typeof(EventSubscription<string>), PubSubEvent.BaseSubscriptions.ElementAt(0).GetType());
        }


        // ── Test doubles ──────────────────────────────────────────────────────────

        private class TestableEventStream<TPayload> : EventStream<TPayload>
        {
            public ICollection<IEventSubscription> BaseSubscriptions => Subscriptions;
        }

        private class SmallBacklogEventStream<TPayload> : EventStream<TPayload>
        {
            protected override uint BacklogSize() => 3;
        }

        private class ExternalAction
        {
            public string PassedValue;
            public bool Executed = false;

            public void ExecuteAction(string value)
            {
                PassedValue = value;
                Executed = true;
            }

            public void ExecuteActionBacklog(string value)
            {
                Executed = true;
            }
        }
    }
}
#endif
