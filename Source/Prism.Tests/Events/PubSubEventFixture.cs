using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Prism.Events;
using Xunit;

namespace Prism.Tests.Events
{
    public class PubSubEventFixture
    {
        [Fact]
        public void EnsureSubscriptionListIsEmptyAfterPublishingAMessage()
        {
            var pubSubEvent = new TestablePubSubEvent<string>();
            SubscribeExternalActionWithoutReference(pubSubEvent);
            GC.Collect();
            pubSubEvent.Publish("testPayload");
            Assert.True(pubSubEvent.BaseSubscriptions.Count == 0, "Subscriptionlist is not empty");
        }

        [Fact]
        public void EnsureSubscriptionListIsNotEmptyWithoutPublishOrSubscribe()
        {
            var pubSubEvent = new TestablePubSubEvent<string>();
            SubscribeExternalActionWithoutReference(pubSubEvent);
            GC.Collect();
            Assert.True(pubSubEvent.BaseSubscriptions.Count == 1, "Subscriptionlist is empty");
        }

        [Fact]
        public void EnsureSubscriptionListIsEmptyAfterSubscribeAgainAMessage()
        {
            var pubSubEvent = new TestablePubSubEvent<string>();
            SubscribeExternalActionWithoutReference(pubSubEvent);
            GC.Collect();
            SubscribeExternalActionWithoutReference(pubSubEvent);
            Assert.True(pubSubEvent.BaseSubscriptions.Count == 1, "Subscriptionlist is empty");
        }

        private static void SubscribeExternalActionWithoutReference(TestablePubSubEvent<string> pubSubEvent)
        {
            pubSubEvent.Subscribe(new ExternalAction().ExecuteAction);
        }


        [Fact]
        public void CanSubscribeAndRaiseEvent()
        {
            TestablePubSubEvent<string> pubSubEvent = new TestablePubSubEvent<string>();
            bool published = false;
            pubSubEvent.Subscribe(delegate { published = true; }, ThreadOption.PublisherThread, true, delegate { return true; });
            pubSubEvent.Publish(null);

            Assert.True(published);
        }

        [Fact]
        public void CanSubscribeAndRaiseEventNonGeneric()
        {
            var pubSubEvent = new TestablePubSubEvent();
            bool published = false;
            pubSubEvent.Subscribe(delegate { published = true; }, ThreadOption.PublisherThread, true);
            pubSubEvent.Publish();

            Assert.True(published);
        }

        [Fact]
        public void CanSubscribeAndRaiseCustomEvent()
        {
            var customEvent = new TestablePubSubEvent<Payload>();
            Payload payload = new Payload();
            var action = new ActionHelper();
            customEvent.Subscribe(action.Action);

            customEvent.Publish(payload);

            Assert.Same(action.ActionArg<Payload>(), payload);
        }

        [Fact]
        public void CanHaveMultipleSubscribersAndRaiseCustomEvent()
        {
            var customEvent = new TestablePubSubEvent<Payload>();
            Payload payload = new Payload();
            var action1 = new ActionHelper();
            var action2 = new ActionHelper();
            customEvent.Subscribe(action1.Action);
            customEvent.Subscribe(action2.Action);

            customEvent.Publish(payload);

            Assert.Same(action1.ActionArg<Payload>(), payload);
            Assert.Same(action2.ActionArg<Payload>(), payload);
        }

        [Fact]
        public void CanHaveMultipleSubscribersAndRaiseEvent()
        {
            var customEvent = new TestablePubSubEvent();
            var action1 = new ActionHelper();
            var action2 = new ActionHelper();
            customEvent.Subscribe(action1.Action);
            customEvent.Subscribe(action2.Action);

            customEvent.Publish();

            Assert.True(action1.ActionCalled);
            Assert.True(action2.ActionCalled);
        }

        [Fact]
        public void SubscribeTakesExecuteDelegateThreadOptionAndFilter()
        {
            TestablePubSubEvent<string> pubSubEvent = new TestablePubSubEvent<string>();
            var action = new ActionHelper();
            pubSubEvent.Subscribe(action.Action);

            pubSubEvent.Publish("test");

            Assert.Equal("test", action.ActionArg<string>());

        }

        [Fact]
        public void FilterEnablesActionTarget()
        {
            TestablePubSubEvent<string> pubSubEvent = new TestablePubSubEvent<string>();
            var goodFilter = new MockFilter { FilterReturnValue = true };
            var actionGoodFilter = new ActionHelper();
            var badFilter = new MockFilter { FilterReturnValue = false };
            var actionBadFilter = new ActionHelper();
            pubSubEvent.Subscribe(actionGoodFilter.Action, ThreadOption.PublisherThread, true, goodFilter.FilterString);
            pubSubEvent.Subscribe(actionBadFilter.Action, ThreadOption.PublisherThread, true, badFilter.FilterString);

            pubSubEvent.Publish("test");

            Assert.True(actionGoodFilter.ActionCalled);
            Assert.False(actionBadFilter.ActionCalled);

        }

        [Fact]
        public void SubscribeDefaultsThreadOptionAndNoFilter()
        {
            TestablePubSubEvent<string> pubSubEvent = new TestablePubSubEvent<string>();
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
            SynchronizationContext calledSyncContext = null;
            var myAction = new ActionHelper()
            {
                ActionToExecute =
                    () => calledSyncContext = SynchronizationContext.Current
            };
            pubSubEvent.Subscribe(myAction.Action);

            pubSubEvent.Publish("test");

            Assert.Equal(SynchronizationContext.Current, calledSyncContext);
        }

        [Fact]
        public void SubscribeDefaultsThreadOptionAndNoFilterNonGeneric()
        {
            var pubSubEvent = new TestablePubSubEvent();
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
            SynchronizationContext calledSyncContext = null;
            var myAction = new ActionHelper()
            {
                ActionToExecute =
                    () => calledSyncContext = SynchronizationContext.Current
            };
            pubSubEvent.Subscribe(myAction.Action);

            pubSubEvent.Publish();

            Assert.Equal(SynchronizationContext.Current, calledSyncContext);
        }

        [Fact]
        public void ShouldUnsubscribeFromPublisherThread()
        {
            var PubSubEvent = new TestablePubSubEvent<string>();

            var actionEvent = new ActionHelper();
            PubSubEvent.Subscribe(
                actionEvent.Action,
                ThreadOption.PublisherThread);

            Assert.True(PubSubEvent.Contains(actionEvent.Action));
            PubSubEvent.Unsubscribe(actionEvent.Action);
            Assert.False(PubSubEvent.Contains(actionEvent.Action));
        }

        [Fact]
        public void ShouldUnsubscribeFromPublisherThreadNonGeneric()
        {
            var pubSubEvent = new TestablePubSubEvent();

            var actionEvent = new ActionHelper();
            pubSubEvent.Subscribe(
                actionEvent.Action,
                ThreadOption.PublisherThread);

            Assert.True(pubSubEvent.Contains(actionEvent.Action));
            pubSubEvent.Unsubscribe(actionEvent.Action);
            Assert.False(pubSubEvent.Contains(actionEvent.Action));
        }

        [Fact]
        public void UnsubscribeShouldNotFailWithNonSubscriber()
        {
            TestablePubSubEvent<string> pubSubEvent = new TestablePubSubEvent<string>();

            Action<string> subscriber = delegate { };
            pubSubEvent.Unsubscribe(subscriber);
        }

        [Fact]
        public void UnsubscribeShouldNotFailWithNonSubscriberNonGeneric()
        {
            var pubSubEvent = new TestablePubSubEvent();

            Action subscriber = delegate { };
            pubSubEvent.Unsubscribe(subscriber);
        }

        [Fact]
        public void ShouldUnsubscribeFromBackgroundThread()
        {
            var PubSubEvent = new TestablePubSubEvent<string>();

            var actionEvent = new ActionHelper();
            PubSubEvent.Subscribe(
                actionEvent.Action,
                ThreadOption.BackgroundThread);

            Assert.True(PubSubEvent.Contains(actionEvent.Action));
            PubSubEvent.Unsubscribe(actionEvent.Action);
            Assert.False(PubSubEvent.Contains(actionEvent.Action));
        }

        [Fact]
        public void ShouldUnsubscribeFromBackgroundThreadNonGeneric()
        {
            var pubSubEvent = new TestablePubSubEvent();

            var actionEvent = new ActionHelper();
            pubSubEvent.Subscribe(
                actionEvent.Action,
                ThreadOption.BackgroundThread);

            Assert.True(pubSubEvent.Contains(actionEvent.Action));
            pubSubEvent.Unsubscribe(actionEvent.Action);
            Assert.False(pubSubEvent.Contains(actionEvent.Action));
        }

        [Fact]
        public void ShouldUnsubscribeFromUIThread()
        {
            var PubSubEvent = new TestablePubSubEvent<string>();
            PubSubEvent.SynchronizationContext = new SynchronizationContext();

            var actionEvent = new ActionHelper();
            PubSubEvent.Subscribe(
                actionEvent.Action,
                ThreadOption.UIThread);

            Assert.True(PubSubEvent.Contains(actionEvent.Action));
            PubSubEvent.Unsubscribe(actionEvent.Action);
            Assert.False(PubSubEvent.Contains(actionEvent.Action));
        }

        [Fact]
        public void ShouldUnsubscribeFromUIThreadNonGeneric()
        {
            var pubSubEvent = new TestablePubSubEvent();
            pubSubEvent.SynchronizationContext = new SynchronizationContext();

            var actionEvent = new ActionHelper();
            pubSubEvent.Subscribe(
                actionEvent.Action,
                ThreadOption.UIThread);

            Assert.True(pubSubEvent.Contains(actionEvent.Action));
            pubSubEvent.Unsubscribe(actionEvent.Action);
            Assert.False(pubSubEvent.Contains(actionEvent.Action));
        }

        [Fact]
        public void ShouldUnsubscribeASingleDelegate()
        {
            var PubSubEvent = new TestablePubSubEvent<string>();

            int callCount = 0;

            var actionEvent = new ActionHelper() { ActionToExecute = () => callCount++ };
            PubSubEvent.Subscribe(actionEvent.Action);
            PubSubEvent.Subscribe(actionEvent.Action);

            PubSubEvent.Publish(null);
            Assert.Equal<int>(2, callCount);

            callCount = 0;
            PubSubEvent.Unsubscribe(actionEvent.Action);
            PubSubEvent.Publish(null);
            Assert.Equal<int>(1, callCount);
        }

        [Fact]
        public void ShouldUnsubscribeASingleDelegateNonGeneric()
        {
            var pubSubEvent = new TestablePubSubEvent();

            int callCount = 0;

            var actionEvent = new ActionHelper() { ActionToExecute = () => callCount++ };
            pubSubEvent.Subscribe(actionEvent.Action);
            pubSubEvent.Subscribe(actionEvent.Action);

            pubSubEvent.Publish();
            Assert.Equal<int>(2, callCount);

            callCount = 0;
            pubSubEvent.Unsubscribe(actionEvent.Action);
            pubSubEvent.Publish();
            Assert.Equal<int>(1, callCount);
        }

        [Fact]
        public async Task ShouldNotExecuteOnGarbageCollectedDelegateReferenceWhenNotKeepAlive()
        {
            var PubSubEvent = new TestablePubSubEvent<string>();

            ExternalAction externalAction = new ExternalAction();
            PubSubEvent.Subscribe(externalAction.ExecuteAction);

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
        public async Task ShouldNotExecuteOnGarbageCollectedDelegateReferenceWhenNotKeepAliveNonGeneric()
        {
            var pubSubEvent = new TestablePubSubEvent();

            var externalAction = new ExternalAction();
            pubSubEvent.Subscribe(externalAction.ExecuteAction);

            pubSubEvent.Publish();
            Assert.True(externalAction.Executed);

            var actionEventReference = new WeakReference(externalAction);
            externalAction = null;
            await Task.Delay(100);
            GC.Collect();
            Assert.False(actionEventReference.IsAlive);

            pubSubEvent.Publish();
        }

        [Fact]
        public async Task ShouldNotExecuteOnGarbageCollectedFilterReferenceWhenNotKeepAlive()
        {
            var PubSubEvent = new TestablePubSubEvent<string>();

            bool wasCalled = false;
            var actionEvent = new ActionHelper() { ActionToExecute = () => wasCalled = true };

            ExternalFilter filter = new ExternalFilter();
            PubSubEvent.Subscribe(actionEvent.Action, ThreadOption.PublisherThread, false, filter.AlwaysTrueFilter);

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
        public void CanAddSubscriptionWhileEventIsFiring()
        {
            var PubSubEvent = new TestablePubSubEvent<string>();

            var emptyAction = new ActionHelper();
            var subscriptionAction = new ActionHelper
            {
                ActionToExecute = (() =>
                                                  PubSubEvent.Subscribe(
                                                      emptyAction.Action))
            };

            PubSubEvent.Subscribe(subscriptionAction.Action);

            Assert.False(PubSubEvent.Contains(emptyAction.Action));

            PubSubEvent.Publish(null);

            Assert.True((PubSubEvent.Contains(emptyAction.Action)));
        }

        [Fact]
        public void CanAddSubscriptionWhileEventIsFiringNonGeneric()
        {
            var pubSubEvent = new TestablePubSubEvent();

            var emptyAction = new ActionHelper();
            var subscriptionAction = new ActionHelper
            {
                ActionToExecute = (() =>
                                          pubSubEvent.Subscribe(
                                          emptyAction.Action))
            };

            pubSubEvent.Subscribe(subscriptionAction.Action);

            Assert.False(pubSubEvent.Contains(emptyAction.Action));

            pubSubEvent.Publish();

            Assert.True((pubSubEvent.Contains(emptyAction.Action)));
        }

        [Fact]
        public void InlineDelegateDeclarationsDoesNotGetCollectedIncorrectlyWithWeakReferences()
        {
            var PubSubEvent = new TestablePubSubEvent<string>();
            bool published = false;
            PubSubEvent.Subscribe(delegate { published = true; }, ThreadOption.PublisherThread, false, delegate { return true; });
            GC.Collect();
            PubSubEvent.Publish(null);

            Assert.True(published);
        }

        [Fact]
        public void InlineDelegateDeclarationsDoesNotGetCollectedIncorrectlyWithWeakReferencesNonGeneric()
        {
            var pubSubEvent = new TestablePubSubEvent();
            bool published = false;
            pubSubEvent.Subscribe(delegate { published = true; }, ThreadOption.PublisherThread, false);
            GC.Collect();
            pubSubEvent.Publish();

            Assert.True(published);
        }

        [Fact]
        public void ShouldNotGarbageCollectDelegateReferenceWhenUsingKeepAlive()
        {
            var PubSubEvent = new TestablePubSubEvent<string>();

            var externalAction = new ExternalAction();
            PubSubEvent.Subscribe(externalAction.ExecuteAction, ThreadOption.PublisherThread, true);

            WeakReference actionEventReference = new WeakReference(externalAction);
            externalAction = null;
            GC.Collect();
            GC.Collect();
            Assert.True(actionEventReference.IsAlive);

            PubSubEvent.Publish("testPayload");

            Assert.Equal("testPayload", ((ExternalAction)actionEventReference.Target).PassedValue);
        }

        [Fact]
        public void ShouldNotGarbageCollectDelegateReferenceWhenUsingKeepAliveNonGeneric()
        {
            var pubSubEvent = new TestablePubSubEvent();

            var externalAction = new ExternalAction();
            pubSubEvent.Subscribe(externalAction.ExecuteAction, ThreadOption.PublisherThread, true);

            WeakReference actionEventReference = new WeakReference(externalAction);
            externalAction = null;
            GC.Collect();
            GC.Collect();
            Assert.True(actionEventReference.IsAlive);

            pubSubEvent.Publish();

            Assert.True(((ExternalAction)actionEventReference.Target).Executed);
        }

        [Fact]
        public void RegisterReturnsTokenThatCanBeUsedToUnsubscribe()
        {
            var PubSubEvent = new TestablePubSubEvent<string>();
            var emptyAction = new ActionHelper();

            var token = PubSubEvent.Subscribe(emptyAction.Action);
            PubSubEvent.Unsubscribe(token);

            Assert.False(PubSubEvent.Contains(emptyAction.Action));
        }

        [Fact]
        public void RegisterReturnsTokenThatCanBeUsedToUnsubscribeNonGeneric()
        {
            var pubSubEvent = new TestablePubSubEvent();
            var emptyAction = new ActionHelper();

            var token = pubSubEvent.Subscribe(emptyAction.Action);
            pubSubEvent.Unsubscribe(token);

            Assert.False(pubSubEvent.Contains(emptyAction.Action));
        }

        [Fact]
        public void ContainsShouldSearchByToken()
        {
            var PubSubEvent = new TestablePubSubEvent<string>();
            var emptyAction = new ActionHelper();
            var token = PubSubEvent.Subscribe(emptyAction.Action);

            Assert.True(PubSubEvent.Contains(token));

            PubSubEvent.Unsubscribe(emptyAction.Action);
            Assert.False(PubSubEvent.Contains(token));
        }

        [Fact]
        public void ContainsShouldSearchByTokenNonGeneric()
        {
            var pubSubEvent = new TestablePubSubEvent();
            var emptyAction = new ActionHelper();
            var token = pubSubEvent.Subscribe(emptyAction.Action);

            Assert.True(pubSubEvent.Contains(token));

            pubSubEvent.Unsubscribe(emptyAction.Action);
            Assert.False(pubSubEvent.Contains(token));
        }

        [Fact]
        public void SubscribeDefaultsToPublisherThread()
        {
            var PubSubEvent = new TestablePubSubEvent<string>();
            Action<string> action = delegate { };
            var token = PubSubEvent.Subscribe(action, true);

            Assert.Equal(1, PubSubEvent.BaseSubscriptions.Count);
            Assert.Equal(typeof(EventSubscription<string>), PubSubEvent.BaseSubscriptions.ElementAt(0).GetType());
        }

        [Fact]
        public void SubscribeDefaultsToPublisherThreadNonGeneric()
        {
            var pubSubEvent = new TestablePubSubEvent();
            Action action = delegate { };
            var token = pubSubEvent.Subscribe(action, true);

            Assert.Equal(1, pubSubEvent.BaseSubscriptions.Count);
            Assert.Equal(typeof(EventSubscription), pubSubEvent.BaseSubscriptions.ElementAt(0).GetType());
        }

        public class ExternalFilter
        {
            public bool AlwaysTrueFilter(string value)
            {
                return true;
            }
        }

        public class ExternalAction
        {
            public string PassedValue;
            public bool Executed = false;

            public void ExecuteAction(string value)
            {
                PassedValue = value;
                Executed = true;
            }

            public void ExecuteAction()
            {
                Executed = true;
            }
        }

        class TestablePubSubEvent<TPayload> : PubSubEvent<TPayload>
        {
            public ICollection<IEventSubscription> BaseSubscriptions
            {
                get { return base.Subscriptions; }
            }
        }

        class TestablePubSubEvent : PubSubEvent
        {
            public ICollection<IEventSubscription> BaseSubscriptions
            {
                get { return base.Subscriptions; }
            }
        }

        public class Payload { }
    }

    public class ActionHelper
    {
        public bool ActionCalled;
        public Action ActionToExecute = null;
        private object actionArg;

        public T ActionArg<T>()
        {
            return (T)actionArg;
        }

        public void Action(PubSubEventFixture.Payload arg)
        {
            Action((object)arg);
        }

        public void Action(string arg)
        {
            Action((object)arg);
        }

        public void Action(object arg)
        {
            actionArg = arg;
            ActionCalled = true;
            if (ActionToExecute != null)
            {
                ActionToExecute.Invoke();
            }
        }

        public void Action()
        {
            ActionCalled = true;
            if (ActionToExecute != null)
            {
                ActionToExecute.Invoke();
            }
        }
    }

    public class MockFilter
    {
        public bool FilterReturnValue;

        public bool FilterString(string arg)
        {
            return FilterReturnValue;
        }
    }
}