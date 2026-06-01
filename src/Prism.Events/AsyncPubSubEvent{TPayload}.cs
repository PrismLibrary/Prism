using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Prism.Events.Properties;

namespace Prism.Events
{
    /// <summary>
    /// Defines a class that manages publication and subscription to asynchronous events.
    /// </summary>
    /// <typeparam name="TPayload">The type of message that will be passed to subscribers.</typeparam>
    public class AsyncPubSubEvent<TPayload> : EventBase
    {
        /// <summary>
        /// Subscribes an asynchronous delegate to an event that will be published on the <see cref="ThreadOption.PublisherThread"/>.
        /// </summary>
        /// <param name="action">The delegate that gets executed when the event is published.</param>
        /// <returns>A <see cref="SubscriptionToken"/> that uniquely identifies the added subscription.</returns>
        public SubscriptionToken Subscribe(Func<TPayload, Task> action)
        {
            return Subscribe(action, ThreadOption.PublisherThread);
        }

        /// <summary>
        /// Subscribes an asynchronous delegate to an event that will be published on the <see cref="ThreadOption.PublisherThread"/>.
        /// </summary>
        /// <param name="action">The delegate that gets executed when the event is published.</param>
        /// <param name="filter">Filter to evaluate if the subscriber should receive the event.</param>
        /// <returns>A <see cref="SubscriptionToken"/> that uniquely identifies the added subscription.</returns>
        public virtual SubscriptionToken Subscribe(Func<TPayload, Task> action, Predicate<TPayload> filter)
        {
            return Subscribe(action, ThreadOption.PublisherThread, false, filter);
        }

        /// <summary>
        /// Subscribes an asynchronous delegate to an event.
        /// </summary>
        /// <param name="action">The delegate that gets executed when the event is published.</param>
        /// <param name="threadOption">Specifies on which thread to receive the delegate callback.</param>
        /// <returns>A <see cref="SubscriptionToken"/> that uniquely identifies the added subscription.</returns>
        public SubscriptionToken Subscribe(Func<TPayload, Task> action, ThreadOption threadOption)
        {
            return Subscribe(action, threadOption, false);
        }

        /// <summary>
        /// Subscribes an asynchronous delegate to an event that will be published on the <see cref="ThreadOption.PublisherThread"/>.
        /// </summary>
        /// <param name="action">The delegate that gets executed when the event is published.</param>
        /// <param name="keepSubscriberReferenceAlive">When <see langword="true"/>, the event keeps a reference to the subscriber so it does not get garbage collected.</param>
        /// <returns>A <see cref="SubscriptionToken"/> that uniquely identifies the added subscription.</returns>
        public SubscriptionToken Subscribe(Func<TPayload, Task> action, bool keepSubscriberReferenceAlive)
        {
            return Subscribe(action, ThreadOption.PublisherThread, keepSubscriberReferenceAlive);
        }

        /// <summary>
        /// Subscribes an asynchronous delegate to an event.
        /// </summary>
        /// <param name="action">The delegate that gets executed when the event is published.</param>
        /// <param name="threadOption">Specifies on which thread to receive the delegate callback.</param>
        /// <param name="keepSubscriberReferenceAlive">When <see langword="true"/>, the event keeps a reference to the subscriber so it does not get garbage collected.</param>
        /// <returns>A <see cref="SubscriptionToken"/> that uniquely identifies the added subscription.</returns>
        public SubscriptionToken Subscribe(Func<TPayload, Task> action, ThreadOption threadOption, bool keepSubscriberReferenceAlive)
        {
            return Subscribe(action, threadOption, keepSubscriberReferenceAlive, null);
        }

        /// <summary>
        /// Subscribes an asynchronous delegate to an event.
        /// </summary>
        /// <param name="action">The delegate that gets executed when the event is published.</param>
        /// <param name="threadOption">Specifies on which thread to receive the delegate callback.</param>
        /// <param name="keepSubscriberReferenceAlive">When <see langword="true"/>, the event keeps a reference to the subscriber so it does not get garbage collected.</param>
        /// <param name="filter">Filter to evaluate if the subscriber should receive the event.</param>
        /// <returns>A <see cref="SubscriptionToken"/> that uniquely identifies the added subscription.</returns>
        public virtual SubscriptionToken Subscribe(Func<TPayload, Task> action, ThreadOption threadOption, bool keepSubscriberReferenceAlive, Predicate<TPayload> filter)
        {
            IDelegateReference actionReference = new DelegateReference(action, keepSubscriberReferenceAlive);
            IDelegateReference filterReference = filter is null
                ? new DelegateReference(new Predicate<TPayload>(delegate { return true; }), true)
                : new DelegateReference(filter, keepSubscriberReferenceAlive);

            IAsyncEventSubscription subscription;
            switch (threadOption)
            {
                case ThreadOption.PublisherThread:
                    subscription = new AsyncEventSubscription<TPayload>(actionReference, filterReference);
                    break;
                case ThreadOption.BackgroundThread:
                    subscription = new BackgroundAsyncEventSubscription<TPayload>(actionReference, filterReference);
                    break;
                case ThreadOption.UIThread:
                    if (SynchronizationContext == null) throw new InvalidOperationException(Resources.EventAggregatorNotConstructedOnUIThread);
                    subscription = new DispatcherAsyncEventSubscription<TPayload>(actionReference, filterReference, SynchronizationContext);
                    break;
                default:
                    subscription = new AsyncEventSubscription<TPayload>(actionReference, filterReference);
                    break;
            }

            return InternalSubscribe(subscription);
        }

        /// <summary>
        /// Publishes the <see cref="AsyncPubSubEvent{TPayload}"/> and completes when all asynchronous subscribers complete.
        /// </summary>
        /// <param name="payload">Message to pass to subscribers.</param>
        /// <returns>A <see cref="Task"/> that completes when all subscribers complete.</returns>
        public virtual Task PublishAsync(TPayload payload)
        {
            return PublishAsync(payload, CancellationToken.None);
        }

        /// <summary>
        /// Publishes the <see cref="AsyncPubSubEvent{TPayload}"/> and completes when all asynchronous subscribers complete or waiting is cancelled.
        /// </summary>
        /// <param name="payload">Message to pass to subscribers.</param>
        /// <param name="cancellationToken">Cancels waiting for subscribers to complete. It does not cancel the subscribers.</param>
        /// <returns>A <see cref="Task"/> that completes when all subscribers complete.</returns>
        public virtual async Task PublishAsync(TPayload payload, CancellationToken cancellationToken)
        {
            var executionStrategies = PruneAndReturnAsyncStrategies();
            var publishTask = Task.WhenAll(executionStrategies.Select(x => x(new object[] { payload })));
            await WaitAsync(publishTask, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Removes the first subscriber matching <see cref="Func{TPayload, Task}"/> from the subscribers list.
        /// </summary>
        /// <param name="subscriber">The delegate used when subscribing to the event.</param>
        public virtual void Unsubscribe(Func<TPayload, Task> subscriber)
        {
            lock (Subscriptions)
            {
                var eventSubscription = Subscriptions.Cast<AsyncEventSubscription<TPayload>>().FirstOrDefault(evt => evt.Action == subscriber);
                if (eventSubscription != null)
                {
                    Subscriptions.Remove(eventSubscription);
                }
            }
        }

        /// <summary>
        /// Returns <see langword="true"/> if there is a subscriber matching <see cref="Func{TPayload, Task}"/>.
        /// </summary>
        /// <param name="subscriber">The delegate used when subscribing to the event.</param>
        /// <returns><see langword="true"/> if there is a subscriber that matches; otherwise <see langword="false"/>.</returns>
        public virtual bool Contains(Func<TPayload, Task> subscriber)
        {
            IAsyncEventSubscription eventSubscription;
            lock (Subscriptions)
            {
                eventSubscription = Subscriptions.Cast<AsyncEventSubscription<TPayload>>().FirstOrDefault(evt => evt.Action == subscriber);
            }

            return eventSubscription != null;
        }

        private List<Func<object[], Task>> PruneAndReturnAsyncStrategies()
        {
            var returnList = new List<Func<object[], Task>>();

            lock (Subscriptions)
            {
                for (var i = Subscriptions.Count - 1; i >= 0; i--)
                {
                    var listItem = ((IAsyncEventSubscription)Subscriptions.ElementAt(i)).GetAsyncExecutionStrategy();
                    if (listItem == null)
                    {
                        Subscriptions.Remove(Subscriptions.ElementAt(i));
                    }
                    else
                    {
                        returnList.Add(listItem);
                    }
                }
            }

            return returnList;
        }

        private static async Task WaitAsync(Task task, CancellationToken cancellationToken)
        {
            if (!cancellationToken.CanBeCanceled)
            {
                await task.ConfigureAwait(false);
                return;
            }

            var taskCompletionSource = new TaskCompletionSource<object>();
            using (cancellationToken.Register(state => ((TaskCompletionSource<object>)state).TrySetCanceled(), taskCompletionSource))
            {
                var completed = await Task.WhenAny(task, taskCompletionSource.Task).ConfigureAwait(false);
                await completed.ConfigureAwait(false);
            }
        }
    }
}
