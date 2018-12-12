


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Prism.Events
{
    ///<summary>
    /// Defines a base class to publish and subscribe to events.
    ///</summary>
    public abstract class EventBase
    {
        private int _pruneGuard;
        private readonly LinkedList<IEventSubscription> _subscriptions = new LinkedList<IEventSubscription>();

        /// <summary>
        /// Allows the SynchronizationContext to be set by the EventAggregator for UI Thread Dispatching
        /// </summary>
        public SynchronizationContext SynchronizationContext { get; set; }

        /// <summary>
        /// Gets the list of current subscriptions.
        /// </summary>
        /// <value>The current subscribers.</value>
        protected ICollection<IEventSubscription> Subscriptions
        {
            get { return _subscriptions; }
        }

        /// <summary>
        /// To limit memory consumption with a rare Publish, the subscriptions
        /// are automatically cleaned up during subscribe, but only as often as
        /// this delay.
        /// </summary>
        protected virtual TimeSpan AutoPruneDelay => TimeSpan.FromMinutes(1);

        /// <summary>
        /// Adds the specified <see cref="IEventSubscription"/> to the subscribers' collection.
        /// </summary>
        /// <param name="eventSubscription">The subscriber.</param>
        /// <returns>The <see cref="SubscriptionToken"/> that uniquely identifies every subscriber.</returns>
        /// <remarks>
        /// Adds the subscription to the internal list and assigns it a new <see cref="SubscriptionToken"/>.
        /// </remarks>
        protected virtual SubscriptionToken InternalSubscribe(IEventSubscription eventSubscription)
        {
            if (eventSubscription == null) throw new ArgumentNullException(nameof(eventSubscription));

            eventSubscription.SubscriptionToken = new SubscriptionToken(Unsubscribe);

            lock (Subscriptions)
            {
                autoPrune();
                Subscriptions.Add(eventSubscription);
            }
            return eventSubscription.SubscriptionToken;

            async void autoPrune() => await PruneAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Calls all the execution strategies exposed by the list of <see cref="IEventSubscription"/>.
        /// </summary>
        /// <param name="arguments">The arguments that will be passed to the listeners.</param>
        /// <remarks>Before executing the strategies, this class will prune all the subscribers from the
        /// list that return a <see langword="null" /> <see cref="Action{T}"/> when calling the
        /// <see cref="IEventSubscription.GetExecutionStrategy"/> method.</remarks>
        protected virtual void InternalPublish(params object[] arguments)
        {
            List<Action<object[]>> executionStrategies = PruneAndReturnStrategies();
            foreach (var executionStrategy in executionStrategies)
            {
                executionStrategy(arguments);
            }
        }

        /// <summary>
        /// Removes the subscriber matching the <see cref="SubscriptionToken"/>.
        /// </summary>
        /// <param name="token">The <see cref="SubscriptionToken"/> returned by <see cref="EventBase"/> while subscribing to the event.</param>
        public virtual void Unsubscribe(SubscriptionToken token)
        {
            lock (Subscriptions)
            {
                IEventSubscription subscription = Subscriptions.FirstOrDefault(evt => evt.SubscriptionToken == token);
                if (subscription != null)
                {
                    Subscriptions.Remove(subscription);
                }
            }
        }

        /// <summary>
        /// Returns <see langword="true"/> if there is a subscriber matching <see cref="SubscriptionToken"/>.
        /// </summary>
        /// <param name="token">The <see cref="SubscriptionToken"/> returned by <see cref="EventBase"/> while subscribing to the event.</param>
        /// <returns><see langword="true"/> if there is a <see cref="SubscriptionToken"/> that matches; otherwise <see langword="false"/>.</returns>
        public virtual bool Contains(SubscriptionToken token)
        {
            lock (Subscriptions)
            {
                IEventSubscription subscription = Subscriptions.FirstOrDefault(evt => evt.SubscriptionToken == token);
                return subscription != null;
            }
        }

        /// <summary>
        /// Prunes Subscriptions list, throttled according to <see cref="AutoPruneDelay"/>.
        /// </summary>
        /// <returns>A task for when this operation is completed.</returns>
        protected virtual async Task PruneAsync()
        {
            if (Interlocked.CompareExchange(ref _pruneGuard, 1, 0) == 1)
                return;

            try
            {
                await Task.Delay(AutoPruneDelay).ConfigureAwait(false);
                PruneAndReturnStrategies();
            } finally { Interlocked.Exchange(ref _pruneGuard, 0); }
        }

        private List<Action<object[]>> PruneAndReturnStrategies()
        {
            lock (_subscriptions)
            {
                return strategies().ToList();
            }

            IEnumerable<Action<object[]>> strategies()
            {
                LinkedListNode<IEventSubscription> node = _subscriptions.Last;
                while (node != null)
                {
                    var previous = node.Previous;
                    IEventSubscription sub = node.Value;
                    Action<object[]> listItem =  sub?.GetExecutionStrategy();

                    if (listItem != null)
                        yield return listItem;
                    else
                        _subscriptions.Remove(node);

                    node = previous;
                }
            }
        }
    }
}