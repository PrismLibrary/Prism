using System;
using Xunit;
using Prism.Events;

namespace Prism.Tests.Events
{
    public class EventBaseFixture
    {
        [Fact]
        public void CanPublishSimpleEvents()
        {
            var eventBase = new TestableEventBase();
            var eventSubscription = new MockEventSubscription();
            bool eventPublished = false;
            eventSubscription.GetPublishActionReturnValue = delegate
                                                                {
                                                                    eventPublished = true;
                                                                };
            eventBase.Subscribe(eventSubscription);

            eventBase.Publish();

            Assert.True(eventSubscription.GetPublishActionCalled);
            Assert.True(eventPublished);
        }

        [Fact]
        public void CanHaveMultipleSubscribersAndRaiseCustomEvent()
        {
            var customEvent = new TestableEventBase();
            Payload payload = new Payload();
            object[] received1 = null;
            object[] received2 = null;
            var eventSubscription1 = new MockEventSubscription();
            eventSubscription1.GetPublishActionReturnValue = delegate(object[] args) { received1 = args; };
            var eventSubscription2 = new MockEventSubscription();
            eventSubscription2.GetPublishActionReturnValue = delegate(object[] args) { received2 = args; };

            customEvent.Subscribe(eventSubscription1);
            customEvent.Subscribe(eventSubscription2);

            customEvent.Publish(payload);

            Assert.Single(received1);
            Assert.Same(received1[0], payload);

            Assert.Single(received2);
            Assert.Same(received2[0], payload);
        }

        [Fact]
        public void ShouldSubscribeAndUnsubscribe()
        {
            var eventBase = new TestableEventBase();

            var eventSubscription = new MockEventSubscription();
            eventBase.Subscribe(eventSubscription);

            Assert.NotNull(eventSubscription.SubscriptionToken);
            Assert.True(eventBase.Contains(eventSubscription.SubscriptionToken));

            eventBase.Unsubscribe(eventSubscription.SubscriptionToken);

            Assert.False(eventBase.Contains(eventSubscription.SubscriptionToken));
        }

        [Fact]
        public void WhenEventSubscriptionActionIsNullPruneItFromList()
        {
            var eventBase = new TestableEventBase();

            var eventSubscription = new MockEventSubscription();
            eventSubscription.GetPublishActionReturnValue = null;

            var token = eventBase.Subscribe(eventSubscription);

            eventBase.Publish();

            Assert.False(eventBase.Contains(token));
        }


        class TestableEventBase : EventBase
        {
            public SubscriptionToken Subscribe(IEventSubscription subscription)
            {
                return base.InternalSubscribe(subscription);
            }

            public void Publish(params object[] arguments)
            {
                base.InternalPublish(arguments);
            }
        }

        class MockEventSubscription : IEventSubscription
        {
            public Action<object[]> GetPublishActionReturnValue;
            public bool GetPublishActionCalled;

            public Action<object[]> GetExecutionStrategy()
            {
                GetPublishActionCalled = true;
                return GetPublishActionReturnValue;
            }

            public SubscriptionToken SubscriptionToken { get; set; }
        }

        class Payload { }

    }
}
