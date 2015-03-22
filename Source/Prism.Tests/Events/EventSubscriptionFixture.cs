// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.


using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Events;

namespace Prism.Tests.Events
{
    [TestClass]
    public class EventSubscriptionFixture
    {
        [TestInitialize]
        public void Setup()
        {
        }

        //todo: fix
        //[TestMethod]
        //public void NullTargetInActionThrows()
        //{
        //    Assert.ThrowsException<ArgumentException>(() =>
        //    {
        //        var actionDelegateReference = new MockDelegateReference()
        //        {
        //            Target = null
        //        };
        //        var filterDelegateReference = new MockDelegateReference()
        //        {
        //            Target = (Predicate<object>)(arg =>
        //            {
        //                return true;
        //            })
        //        };
        //        var eventSubscription = new EventSubscription<object>(actionDelegateReference,
        //                                                                        filterDelegateReference);
        //    });
        //}

        //[TestMethod]
        //public void DifferentTargetTypeInActionThrows()
        //{
        //    Assert.ThrowsException<ArgumentException>(() =>
        //    {
        //        var actionDelegateReference = new MockDelegateReference()
        //        {
        //            Target = (Action<int>)delegate { }
        //        };
        //        var filterDelegateReference = new MockDelegateReference()
        //        {
        //            Target = (Predicate<string>)(arg =>
        //            {
        //                return true;
        //            })
        //        };
        //        var eventSubscription = new EventSubscription<string>(actionDelegateReference,
        //                                                                        filterDelegateReference);
        //    });
        //}

        //[TestMethod]
        //public void NullActionThrows()
        //{
        //    Assert.ThrowsException<ArgumentNullException>(() =>
        //    {
        //        var filterDelegateReference = new MockDelegateReference()
        //        {
        //            Target = (Predicate<object>)(arg =>
        //            {
        //                return true;
        //            })
        //        };
        //        var eventSubscription = new EventSubscription<object>(null,
        //                                                                        filterDelegateReference);
        //    });
        //}

        //[TestMethod]
        //public void NullTargetInFilterThrows()
        //{
        //    Assert.ThrowsException<ArgumentException>(() =>
        //    {
        //        var actionDelegateReference = new MockDelegateReference()
        //        {
        //            Target = (Action<object>)delegate { }
        //        };

        //        var filterDelegateReference = new MockDelegateReference()
        //        {
        //            Target = null
        //        };
        //        var eventSubscription = new EventSubscription<object>(actionDelegateReference,
        //                                                                        filterDelegateReference);
        //    });
        //}


        //[TestMethod]
        //public void DifferentTargetTypeInFilterThrows()
        //{
        //    Assert.ThrowsException<ArgumentException>(() =>
        //    {

        //        var actionDelegateReference = new MockDelegateReference()
        //        {
        //            Target = (Action<string>)delegate { }
        //        };

        //        var filterDelegateReference = new MockDelegateReference()
        //        {
        //            Target = (Predicate<int>)(arg =>
        //            {
        //                return true;
        //            })
        //        };
        //        var eventSubscription = new EventSubscription<string>(actionDelegateReference,
        //                                                                        filterDelegateReference);
        //    });
        //}

        //[TestMethod]
        //public void NullFilterThrows()
        //{
        //    Assert.ThrowsException<ArgumentNullException>(() =>
        //    {
        //        var actionDelegateReference = new MockDelegateReference()
        //        {
        //            Target = (Action<object>)delegate { }
        //        };

        //        var eventSubscription = new EventSubscription<object>(actionDelegateReference,
        //                                                                        null);
        //    });
        //}

        [TestMethod]
        public void CanInitEventSubscription()
        {
            var actionDelegateReference = new MockDelegateReference((Action<object>)delegate { });
            var filterDelegateReference = new MockDelegateReference((Predicate<object>)delegate { return true; });
            var eventSubscription = new EventSubscription<object>(actionDelegateReference, filterDelegateReference);

            var subscriptionToken = new SubscriptionToken(t => { });

            eventSubscription.SubscriptionToken = subscriptionToken;

            Assert.AreSame(actionDelegateReference.Target, eventSubscription.Action);
            Assert.AreSame(filterDelegateReference.Target, eventSubscription.Filter);
            Assert.AreSame(subscriptionToken, eventSubscription.SubscriptionToken);
        }

        [TestMethod]
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

            Assert.IsNotNull(publishAction);

            publishAction.Invoke(null);

            Assert.AreEqual(2, executedDelegates.Count);
            Assert.AreEqual("Filter", executedDelegates[0]);
            Assert.AreEqual("Action", executedDelegates[1]);
        }

        [TestMethod]
        public void GetPublishActionReturnsNullIfActionIsNull()
        {
            var actionDelegateReference = new MockDelegateReference((Action<object>)delegate { });
            var filterDelegateReference = new MockDelegateReference((Predicate<object>)delegate { return true; });

            var eventSubscription = new EventSubscription<object>(actionDelegateReference, filterDelegateReference);

            var publishAction = eventSubscription.GetExecutionStrategy();

            Assert.IsNotNull(publishAction);

            actionDelegateReference.Target = null;

            publishAction = eventSubscription.GetExecutionStrategy();

            Assert.IsNull(publishAction);
        }

        [TestMethod]
        public void GetPublishActionReturnsNullIfFilterIsNull()
        {
            var actionDelegateReference = new MockDelegateReference((Action<object>)delegate { });
            var filterDelegateReference = new MockDelegateReference((Predicate<object>)delegate { return true; });

            var eventSubscription = new EventSubscription<object>(actionDelegateReference, filterDelegateReference);

            var publishAction = eventSubscription.GetExecutionStrategy();

            Assert.IsNotNull(publishAction);

            filterDelegateReference.Target = null;

            publishAction = eventSubscription.GetExecutionStrategy();

            Assert.IsNull(publishAction);
        }

        [TestMethod]
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

            Assert.IsFalse(actionExecuted);
        }

        [TestMethod]
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

            Assert.AreEqual("TestString", passedArgumentToAction);
            Assert.AreEqual("TestString", passedArgumentToFilter);
        }



    }
}
