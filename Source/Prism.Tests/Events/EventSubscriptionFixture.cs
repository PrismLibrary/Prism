using System;
using System.Collections.Generic;
using Xunit;
using Prism.Events;

namespace Prism.Tests.Events
{
    public class EventSubscriptionFixture
    {
        [Fact]
        public void NullTargetInActionThrows()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var actionDelegateReference = new MockDelegateReference()
                {
                    Target = null
                };
                var filterDelegateReference = new MockDelegateReference()
                {
                    Target = (Predicate<object>)(arg =>
                    {
                        return true;
                    })
                };
                var eventSubscription = new EventSubscription<object>(actionDelegateReference,
                                                                                filterDelegateReference);
            });

        }

        [Fact]
        public void NullTargetInActionThrowsNonGeneric()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var actionDelegateReference = new MockDelegateReference()
                {
                    Target = null
                };
                var eventSubscription = new EventSubscription(actionDelegateReference);
            });
        }

        [Fact]
        public void DifferentTargetTypeInActionThrows()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var actionDelegateReference = new MockDelegateReference()
                {
                    Target = (Action<int>)delegate { }
                };
                var filterDelegateReference = new MockDelegateReference()
                {
                    Target = (Predicate<string>)(arg =>
                    {
                        return true;
                    })
                };
                var eventSubscription = new EventSubscription<string>(actionDelegateReference,
                                                                                filterDelegateReference);
            });
        }

        [Fact]
        public void DifferentTargetTypeInActionThrowsNonGeneric()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var actionDelegateReference = new MockDelegateReference()
                {
                    Target = (Action<int>)delegate { }
                };

                var eventSubscription = new EventSubscription(actionDelegateReference);
            });
        }

        [Fact]
        public void NullActionThrows()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var filterDelegateReference = new MockDelegateReference()
                {
                    Target = (Predicate<object>)(arg =>
                    {
                        return true;
                    })
                };
                var eventSubscription = new EventSubscription<object>(null, filterDelegateReference);
            });
        }

        [Fact]
        public void NullActionThrowsNonGeneric()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var eventSubscription = new EventSubscription(null);
            });
        }

        [Fact]
        public void NullTargetInFilterThrows()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var actionDelegateReference = new MockDelegateReference()
                {
                    Target = (Action<object>)delegate { }
                };

                var filterDelegateReference = new MockDelegateReference()
                {
                    Target = null
                };
                var eventSubscription = new EventSubscription<object>(actionDelegateReference,
                                                                                filterDelegateReference);
            });
        }


        [Fact]
        public void DifferentTargetTypeInFilterThrows()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var actionDelegateReference = new MockDelegateReference()
                {
                    Target = (Action<string>)delegate { }
                };

                var filterDelegateReference = new MockDelegateReference()
                {
                    Target = (Predicate<int>)(arg =>
                    {
                        return true;
                    })
                };

                var eventSubscription = new EventSubscription<string>(actionDelegateReference,
                                                                                filterDelegateReference);
            });
        }

        [Fact]
        public void NullFilterThrows()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var actionDelegateReference = new MockDelegateReference()
                {
                    Target = (Action<object>)delegate { }
                };

                var eventSubscription = new EventSubscription<object>(actionDelegateReference,
                                                                                null);
            });
        }

        [Fact]
        public void CanInitEventSubscription()
        {
            var actionDelegateReference = new MockDelegateReference((Action<object>)delegate { });
            var filterDelegateReference = new MockDelegateReference((Predicate<object>)delegate { return true; });
            var eventSubscription = new EventSubscription<object>(actionDelegateReference, filterDelegateReference);

            var subscriptionToken = new SubscriptionToken(t => { });

            eventSubscription.SubscriptionToken = subscriptionToken;

            Assert.Same(actionDelegateReference.Target, eventSubscription.Action);
            Assert.Same(filterDelegateReference.Target, eventSubscription.Filter);
            Assert.Same(subscriptionToken, eventSubscription.SubscriptionToken);
        }

        [Fact]
        public void CanInitEventSubscriptionNonGeneric()
        {
            var actionDelegateReference = new MockDelegateReference((Action)delegate { });
            var eventSubscription = new EventSubscription(actionDelegateReference);

            var subscriptionToken = new SubscriptionToken(t => { });

            eventSubscription.SubscriptionToken = subscriptionToken;

            Assert.Same(actionDelegateReference.Target, eventSubscription.Action);
            Assert.Same(subscriptionToken, eventSubscription.SubscriptionToken);
        }

        [Fact]
        public void GetPublishActionReturnsDelegateThatExecutesTheFilterAndThenTheAction()
        {
            var executedDelegates = new List<string>();
            var actionDelegateReference =
                new MockDelegateReference((Action<object>)delegate { executedDelegates.Add("Action"); });

            var filterDelegateReference = new MockDelegateReference((Predicate<object>)delegate
            {
                executedDelegates.Add(
                    "Filter");
                return true;
            });

            var eventSubscription = new EventSubscription<object>(actionDelegateReference, filterDelegateReference);


            var publishAction = eventSubscription.GetExecutionStrategy();

            Assert.NotNull(publishAction);

            publishAction.Invoke(null);

            Assert.Equal(2, executedDelegates.Count);
            Assert.Equal("Filter", executedDelegates[0]);
            Assert.Equal("Action", executedDelegates[1]);
        }

        [Fact]
        public void GetPublishActionReturnsNullIfActionIsNull()
        {
            var actionDelegateReference = new MockDelegateReference((Action<object>)delegate { });
            var filterDelegateReference = new MockDelegateReference((Predicate<object>)delegate { return true; });

            var eventSubscription = new EventSubscription<object>(actionDelegateReference, filterDelegateReference);

            var publishAction = eventSubscription.GetExecutionStrategy();

            Assert.NotNull(publishAction);

            actionDelegateReference.Target = null;

            publishAction = eventSubscription.GetExecutionStrategy();

            Assert.Null(publishAction);
        }

        [Fact]
        public void GetPublishActionReturnsNullIfActionIsNullNonGeneric()
        {
            var actionDelegateReference = new MockDelegateReference((Action)delegate { });

            var eventSubscription = new EventSubscription(actionDelegateReference);

            var publishAction = eventSubscription.GetExecutionStrategy();

            Assert.NotNull(publishAction);

            actionDelegateReference.Target = null;

            publishAction = eventSubscription.GetExecutionStrategy();

            Assert.Null(publishAction);
        }

        [Fact]
        public void GetPublishActionReturnsNullIfFilterIsNull()
        {
            var actionDelegateReference = new MockDelegateReference((Action<object>)delegate { });
            var filterDelegateReference = new MockDelegateReference((Predicate<object>)delegate { return true; });

            var eventSubscription = new EventSubscription<object>(actionDelegateReference, filterDelegateReference);

            var publishAction = eventSubscription.GetExecutionStrategy();

            Assert.NotNull(publishAction);

            filterDelegateReference.Target = null;

            publishAction = eventSubscription.GetExecutionStrategy();

            Assert.Null(publishAction);
        }

        [Fact]
        public void GetPublishActionDoesNotExecuteActionIfFilterReturnsFalse()
        {
            bool actionExecuted = false;
            var actionDelegateReference = new MockDelegateReference()
            {
                Target = (Action<int>)delegate { actionExecuted = true; }
            };
            var filterDelegateReference = new MockDelegateReference((Predicate<int>)delegate
            {
                return false;
            });

            var eventSubscription = new EventSubscription<int>(actionDelegateReference, filterDelegateReference);


            var publishAction = eventSubscription.GetExecutionStrategy();

            publishAction.Invoke(new object[] { null });

            Assert.False(actionExecuted);
        }

        [Fact]
        public void StrategyPassesArgumentToDelegates()
        {
            string passedArgumentToAction = null;
            string passedArgumentToFilter = null;

            var actionDelegateReference = new MockDelegateReference((Action<string>)(obj => passedArgumentToAction = obj));
            var filterDelegateReference = new MockDelegateReference((Predicate<string>)(obj =>
            {
                passedArgumentToFilter = obj;
                return true;
            }));

            var eventSubscription = new EventSubscription<string>(actionDelegateReference, filterDelegateReference);
            var publishAction = eventSubscription.GetExecutionStrategy();

            publishAction.Invoke(new[] { "TestString" });

            Assert.Equal("TestString", passedArgumentToAction);
            Assert.Equal("TestString", passedArgumentToFilter);
        }
    }
}
