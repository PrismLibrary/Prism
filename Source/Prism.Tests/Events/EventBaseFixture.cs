


using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Events;

namespace Prism.Tests.Events
{
    [TestClass]
    public class EventBaseFixture
    {
        [TestMethod]
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

            Assert.IsTrue(eventSubscription.GetPublishActionCalled);
            Assert.IsTrue(eventPublished);
        }

        [TestMethod]
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

            Assert.AreEqual(1, received1.Length);
            Assert.AreSame(received1[0], payload);

            Assert.AreEqual(1, received2.Length);
            Assert.AreSame(received2[0], payload);
        }

        [TestMethod]
        public void ShouldSubscribeAndUnsubscribe()
        {
            var eventBase = new TestableEventBase();

            var eventSubscription = new MockEventSubscription();
            eventBase.Subscribe(eventSubscription);

            Assert.IsNotNull(eventSubscription.SubscriptionToken);
            Assert.IsTrue(eventBase.Contains(eventSubscription.SubscriptionToken));

            eventBase.Unsubscribe(eventSubscription.SubscriptionToken);

            Assert.IsFalse(eventBase.Contains(eventSubscription.SubscriptionToken));
        }

        [TestMethod]
        public void WhenEventSubscriptionActionIsNullPruneItFromList()
        {
            var eventBase = new TestableEventBase();

            var eventSubscription = new MockEventSubscription();
            eventSubscription.GetPublishActionReturnValue = null;

            var token = eventBase.Subscribe(eventSubscription);

            eventBase.Publish();

            Assert.IsFalse(eventBase.Contains(token));
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
