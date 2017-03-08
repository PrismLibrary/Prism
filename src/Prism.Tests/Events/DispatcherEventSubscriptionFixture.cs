using System;
using System.Threading;
using Xunit;
using Prism.Events;

namespace Prism.Tests.Events
{
    public class DispatcherEventSubscriptionFixture
    {
        [Fact]
        public void ShouldCallInvokeOnDispatcher()
        {
            DispatcherEventSubscription<object> eventSubscription = null;

            IDelegateReference actionDelegateReference = new MockDelegateReference()
            {
                Target = (Action<object>)(arg =>
                {
                    return;
                })
            };

            IDelegateReference filterDelegateReference = new MockDelegateReference
            {
                Target = (Predicate<object>)(arg => true)
            };
            var mockSyncContext = new MockSynchronizationContext();

            eventSubscription = new DispatcherEventSubscription<object>(actionDelegateReference, filterDelegateReference, mockSyncContext);

            eventSubscription.GetExecutionStrategy().Invoke(new object[0]);

            Assert.True(mockSyncContext.InvokeCalled);
        }

        [Fact]
        public void ShouldCallInvokeOnDispatcherNonGeneric()
        {
            DispatcherEventSubscription eventSubscription = null;

            IDelegateReference actionDelegateReference = new MockDelegateReference()
            {
                Target = (Action)(() =>
                { })
            };

            var mockSyncContext = new MockSynchronizationContext();

            eventSubscription = new DispatcherEventSubscription(actionDelegateReference, mockSyncContext);

            eventSubscription.GetExecutionStrategy().Invoke(new object[0]);

            Assert.True(mockSyncContext.InvokeCalled);
        }

        [Fact]
        public void ShouldPassParametersCorrectly()
        {
            IDelegateReference actionDelegateReference = new MockDelegateReference()
            {
                Target =
                    (Action<object>)(arg1 =>
                    {
                        return;
                    })
            };
            IDelegateReference filterDelegateReference = new MockDelegateReference
            {
                Target = (Predicate<object>)(arg => true)
            };

            var mockSyncContext = new MockSynchronizationContext();

            DispatcherEventSubscription<object> eventSubscription = new DispatcherEventSubscription<object>(actionDelegateReference, filterDelegateReference, mockSyncContext);

            var executionStrategy = eventSubscription.GetExecutionStrategy();
            Assert.NotNull(executionStrategy);

            object argument1 = new object();

            executionStrategy.Invoke(new[] { argument1 });

            Assert.Same(argument1, mockSyncContext.InvokeArg);
        }
    }

    internal class MockSynchronizationContext : SynchronizationContext
    {
        public bool InvokeCalled;
        public object InvokeArg;

        public override void Post(SendOrPostCallback d, object state)
        {
            InvokeCalled = true;
            InvokeArg = state;
        }
    }
}
