using System;
using System.Threading.Tasks;

namespace Prism.Events
{
    /// <summary>
    /// Defines a contract for asynchronous event subscriptions.
    /// </summary>
    public interface IAsyncEventSubscription : IEventSubscription
    {
        /// <summary>
        /// Gets the asynchronous execution strategy for this subscription.
        /// </summary>
        /// <returns>A <see cref="Func{T, TResult}"/> with the execution strategy, or <see langword="null"/> if the subscription is no longer valid.</returns>
        Func<object[], Task> GetAsyncExecutionStrategy();
    }
}
