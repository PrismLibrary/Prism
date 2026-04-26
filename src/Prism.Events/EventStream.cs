#if NET8_0_OR_GREATER
using System.Buffers;
using Prism.Events.Properties;

namespace Prism.Events;

/// <summary>
/// A strongly-typed pub/sub event stream that extends <see cref="EventBase"/> with a fixed-size
/// rolling backlog. When a new subscriber calls <c>Subscribe</c>, it immediately receives all
/// events currently held in the backlog via the supplied <c>backlogAction</c> callback, then
/// continues to receive future published events through the main <c>action</c> callback.
/// </summary>
/// <remarks>
/// <para>
/// The backlog is implemented as an in-memory ring buffer whose capacity is determined by
/// <see cref="BacklogSize"/>. When the buffer is full, the oldest entry is evicted to make room
/// for the newest one. The default capacity is 10.
/// </para>
/// <para>
/// This class is only available on .NET 8 and later (<c>NET8_0_OR_GREATER</c>) because it relies
/// on <see cref="System.Buffers.ArrayPool{T}"/> spans and other modern runtime features.
/// </para>
/// <para>
/// Thread safety: all mutations to the subscription list and the ring buffer are performed inside
/// a <see langword="lock"/> on <see cref="EventBase.Subscriptions"/>. Subscriber callbacks are
/// dispatched outside the lock to avoid deadlocks.
/// </para>
/// </remarks>
/// <typeparam name="TPayload">The type of the event payload.</typeparam>
public class EventStream<TPayload> : EventBase
{
    private const int DefaultBacklogSize = 10;

    private Backlog<TPayload> recentEvents;

    /// <summary>
    /// Initializes a new instance of <see cref="EventStream{TPayload}"/> with the default
    /// backlog size of <c>10</c>.
    /// </summary>
    public EventStream()
    {
        recentEvents = new Backlog<TPayload>((int)BacklogSize());
    }

    /// <summary>
    /// Subscribes to the event stream on the publisher's thread without a filter.
    /// Any events currently held in the backlog are immediately delivered to
    /// <paramref name="backlogAction"/>.
    /// </summary>
    /// <param name="action">The callback invoked for each future published event.</param>
    /// <param name="backlogAction">
    /// The callback invoked once for each event currently in the backlog at the time of
    /// subscription. May be <see langword="null"/> to skip backlog replay.
    /// </param>
    /// <returns>
    /// A <see cref="SubscriptionToken"/> that can be used to unsubscribe from the event stream.
    /// </returns>
    public SubscriptionToken Subscribe(Action<TPayload> action, Action<TPayload> backlogAction)
    {
        return Subscribe(action, backlogAction, ThreadOption.PublisherThread, false, null);
    }

    /// <summary>
    /// Subscribes to the event stream on the specified thread without a filter.
    /// Any events currently held in the backlog are immediately delivered to
    /// <paramref name="backlogAction"/>.
    /// </summary>
    /// <param name="action">The callback invoked for each future published event.</param>
    /// <param name="backlogAction">
    /// The callback invoked once for each event currently in the backlog at the time of
    /// subscription. May be <see langword="null"/> to skip backlog replay.
    /// </param>
    /// <param name="threadOption">
    /// Specifies the thread on which <paramref name="action"/> is invoked.
    /// See <see cref="ThreadOption"/> for available options.
    /// </param>
    /// <returns>
    /// A <see cref="SubscriptionToken"/> that can be used to unsubscribe from the event stream.
    /// </returns>
    public SubscriptionToken Subscribe(Action<TPayload> action, Action<TPayload> backlogAction, ThreadOption threadOption)
    {
        return Subscribe(action, backlogAction, threadOption, false, null);
    }

    /// <summary>
    /// Subscribes to the event stream on the specified thread, optionally keeping a strong
    /// reference to the subscriber, without a filter.
    /// Any events currently held in the backlog are immediately delivered to
    /// <paramref name="backlogAction"/>.
    /// </summary>
    /// <param name="action">The callback invoked for each future published event.</param>
    /// <param name="backlogAction">
    /// The callback invoked once for each event currently in the backlog at the time of
    /// subscription. May be <see langword="null"/> to skip backlog replay.
    /// </param>
    /// <param name="threadOption">
    /// Specifies the thread on which <paramref name="action"/> is invoked.
    /// </param>
    /// <param name="keepSubscriberReferenceAlive">
    /// When <see langword="true"/>, the event stream holds a strong reference to
    /// <paramref name="action"/>, preventing it from being garbage-collected. When
    /// <see langword="false"/> a weak reference is used and the subscription is pruned
    /// automatically once the subscriber is collected.
    /// </param>
    /// <returns>
    /// A <see cref="SubscriptionToken"/> that can be used to unsubscribe from the event stream.
    /// </returns>
    public SubscriptionToken Subscribe(Action<TPayload> action, Action<TPayload> backlogAction, ThreadOption threadOption, bool keepSubscriberReferenceAlive)
    {
        return Subscribe(action, backlogAction, threadOption, keepSubscriberReferenceAlive, null);
    }

    /// <summary>
    /// Subscribes to the event stream with full control over thread marshalling, reference
    /// lifetime, and payload filtering.
    /// Any events currently held in the backlog that satisfy <paramref name="filter"/> are
    /// immediately delivered to <paramref name="backlogAction"/>.
    /// </summary>
    /// <param name="action">The callback invoked for each future published event.</param>
    /// <param name="backlogAction">
    /// The callback invoked once for each matching event currently in the backlog at the time
    /// of subscription. May be <see langword="null"/> to skip backlog replay.
    /// </param>
    /// <param name="threadOption">
    /// Specifies the thread on which <paramref name="action"/> is invoked.
    /// </param>
    /// <param name="keepSubscriberReferenceAlive">
    /// When <see langword="true"/>, the event stream holds a strong reference to
    /// <paramref name="action"/>. When <see langword="false"/> a weak reference is used.
    /// </param>
    /// <param name="filter">
    /// A predicate applied to each incoming payload. Only payloads for which the predicate
    /// returns <see langword="true"/> are forwarded to <paramref name="action"/>. Pass
    /// <see langword="null"/> to receive all payloads.
    /// </param>
    /// <returns>
    /// A <see cref="SubscriptionToken"/> that can be used to unsubscribe from the event stream.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when <paramref name="threadOption"/> is <see cref="ThreadOption.UIThread"/> and
    /// <see cref="EventBase.SynchronizationContext"/> has not been set.
    /// </exception>
    public SubscriptionToken Subscribe(Action<TPayload> action, Action<TPayload> backlogAction, ThreadOption threadOption, bool keepSubscriberReferenceAlive, Predicate<TPayload> filter)
    {
        IDelegateReference actionReference = new DelegateReference(action, keepSubscriberReferenceAlive);
        IDelegateReference filterReference;
        if (filter != null)
        {
            filterReference = new DelegateReference(filter, keepSubscriberReferenceAlive);
        }
        else
        {
            filterReference = new DelegateReference(new Predicate<TPayload>(delegate { return true; }), true);
        }

        EventSubscription<TPayload> subscription;

        switch (threadOption)
        {
            case ThreadOption.PublisherThread:
                subscription = new EventSubscription<TPayload>(actionReference, filterReference);
                break;
            case ThreadOption.BackgroundThread:
                subscription = new BackgroundEventSubscription<TPayload>(actionReference, filterReference);
                break;
            case ThreadOption.UIThread:
                if (SynchronizationContext == null) throw new InvalidOperationException(Resources.EventAggregatorNotConstructedOnUIThread);
                subscription = new DispatcherEventSubscription<TPayload>(actionReference, filterReference, SynchronizationContext);
                break;
            default:
                subscription = new EventSubscription<TPayload>(actionReference, filterReference);
                break;
        }

        return Subscribe(subscription, backlogAction);
    }

    /// <summary>
    /// Registers an already-constructed <see cref="EventSubscription{TPayload}"/>, assigns it a
    /// new <see cref="SubscriptionToken"/>, and replays the current backlog via
    /// <paramref name="backlogAction"/> while holding the subscription lock.
    /// </summary>
    /// <param name="eventSubscription">The subscription to register.</param>
    /// <param name="backlogAction">
    /// The callback invoked once for each event currently in the backlog. May be
    /// <see langword="null"/> to skip backlog replay.
    /// </param>
    /// <returns>The <see cref="SubscriptionToken"/> assigned to the subscription.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="eventSubscription"/> is <see langword="null"/>.
    /// </exception>
    private SubscriptionToken Subscribe(EventSubscription<TPayload> eventSubscription, Action<TPayload> backlogAction)
    {
        if (eventSubscription == null)
            throw new ArgumentNullException(nameof(eventSubscription));

        eventSubscription.SubscriptionToken = new SubscriptionToken(Unsubscribe);

        TPayload[] backlog = null;

        try
        {
            int backlogCount = 0;

            lock (Subscriptions)
            {
                Subscriptions.Add(eventSubscription);
                var ring = recentEvents.CurrentState(out uint readPosition, out uint writePosition);
                backlog = ArrayPool<TPayload>.Shared.Rent(ring.Length);
                int index = 0;
                TPayload item = default;
                while (index < ring.Length &&
                      Backlog<TPayload>.TryRead(ref ring, ref readPosition, writePosition, out item))
                {
                    backlog[index] = item;
                    index++;
                }

                backlogCount = index;
            }

            if (backlogAction != null && backlogCount > 0)
            {
                foreach (var item in backlog.AsSpan(0, backlogCount))
                {
                    if(eventSubscription.Filter(item))
                    {
                        backlogAction(item);
                    }
                }
            }
        }
        finally
        {
            if (backlog != null)
                ArrayPool<TPayload>.Shared.Return(backlog, clearArray: true);
        }

        return eventSubscription.SubscriptionToken;
    }

    /// <summary>
    /// Removes the first subscription whose action delegate matches
    /// <paramref name="subscriber"/> from the subscribers' list.
    /// </summary>
    /// <param name="subscriber">
    /// The <see cref="Action{TPayload}"/> delegate that was passed to <c>Subscribe</c>.
    /// </param>
    public virtual void Unsubscribe(Action<TPayload> subscriber)
    {
        lock (Subscriptions)
        {
            IEventSubscription eventSubscription = Subscriptions.Cast<EventSubscription<TPayload>>().FirstOrDefault(evt => evt.Action == subscriber);
            if (eventSubscription != null)
            {
                Subscriptions.Remove(eventSubscription);
            }
        }
    }

    /// <summary>
    /// Publishes <paramref name="payload"/> to all active subscribers and appends it to the
    /// rolling backlog so that future subscribers can replay it.
    /// </summary>
    /// <param name="payload">The event payload to publish.</param>
    public virtual void Publish(TPayload payload)
    {
        InternalPublish(payload);
    }

    /// <summary>
    /// Returns <see langword="true"/> if there is a subscriber matching <see cref="Action"/>.
    /// </summary>
    /// <param name="subscriber">The <see cref="Action"/> used when subscribing to the event.</param>
    /// <returns><see langword="true"/> if there is an <see cref="Action"/> that matches; otherwise <see langword="false"/>.</returns>
    public virtual bool Contains(Action<TPayload> subscriber)
    {
        IEventSubscription eventSubscription;
        lock (Subscriptions)
        {
            eventSubscription = Subscriptions.Cast<EventSubscription<TPayload>>().FirstOrDefault(evt => evt.Action == subscriber);
        }
        return eventSubscription != null;
    }

    /// <summary>
    /// Core publish implementation. Writes the payload to the ring-buffer backlog, collects
    /// the execution strategies of all live subscriptions (pruning dead weak-reference
    /// subscriptions in the process), then dispatches outside the lock to avoid deadlocks.
    /// </summary>
    /// <param name="arguments">
    /// The boxed arguments array; element <c>[0]</c> must be of type <typeparamref name="TPayload"/>.
    /// </param>
    override protected void InternalPublish(params object[] arguments)
    {
        List<Action<object[]>> strategies;

        lock (Subscriptions)
        {
            recentEvents.Write((TPayload)arguments[0]);

            strategies = new List<Action<object[]>>(Subscriptions.Count);

            for (int i = Subscriptions.Count - 1; i >= 0; i--)
            {
                var subscription = Subscriptions.ElementAt(i);
                var strategy = subscription.GetExecutionStrategy();
                if (strategy == null)
                    Subscriptions.Remove(subscription);// prune dead weak refs
                else
                    strategies.Add(strategy);
            }
        }

        // Dispatch outside the lock — never invoke callbacks under a lock
        foreach (var strategy in strategies)
            strategy(arguments);
    }

    /// <summary>
    /// Overrides <see cref="EventBase.InternalSubscribe"/> to route subscriptions through
    /// the backlog-aware <c>Subscribe</c> overload. Backlog replay is skipped (<c>null</c>
    /// backlog action) when subscribing via the base-class path.
    /// </summary>
    /// <param name="eventSubscription">The subscription to register.</param>
    /// <returns>The <see cref="SubscriptionToken"/> assigned to the subscription.</returns>
    override protected SubscriptionToken InternalSubscribe(IEventSubscription eventSubscription)
    {
        return Subscribe((EventSubscription<TPayload>)eventSubscription, null);
    }

    /// <summary>
    /// Returns the capacity of the rolling backlog ring buffer.
    /// Override this method in a derived class to change the number of events retained for
    /// late subscribers. The default value is <c>10</c>.
    /// </summary>
    /// <returns>The number of recent events to retain in the backlog.</returns>
    virtual protected uint BacklogSize() => DefaultBacklogSize;

    /// <summary>
    /// A fixed-capacity, thread-unsafe ring buffer used to store the most recent
    /// <typeparamref name="T"/> items published to the stream. When the buffer is full the
    /// oldest item is silently overwritten. Intended for use only inside the
    /// <see cref="EventStream{TPayload}"/> lock region.
    /// </summary>
    /// <typeparam name="T">The element type stored in the ring buffer.</typeparam>
    private class Backlog<T>
    {
        private readonly int size;
        private readonly T[] ring;
        private uint writePosition;
        private uint readPosition;

        /// <summary>
        /// Initializes a new <see cref="Backlog{T}"/> with the specified capacity.
        /// </summary>
        /// <param name="capacity">Maximum number of items the ring buffer can hold.</param>
        public Backlog(int capacity)
        {
            size = capacity;
            ring = new T[capacity];
            writePosition = 0;
            readPosition = 0;
        }

        /// <summary>
        /// Writes <paramref name="item"/> to the ring buffer. If the buffer is full, the
        /// oldest unread item is evicted by advancing the read position before writing.
        /// </summary>
        /// <param name="item">The item to write.</param>
        public void Write(T item)
        {
            bool isFull = writePosition - readPosition >= ring.Length;


            if (isFull)
            {
                readPosition++;
            }

            ring[writePosition % size] = item;
            writePosition++;
        }

        /// <summary>
        /// Attempts to read the next item from the ring buffer, advancing the internal read
        /// cursor on success.
        /// </summary>
        /// <param name="item">
        /// When this method returns <see langword="true"/>, contains the next item; otherwise
        /// the default value of <typeparamref name="T"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if an item was read; <see langword="false"/> if the buffer
        /// is empty.
        /// </returns>
        public bool TryRead(out T item)
        {
            var span = new ReadOnlySpan<T>(ring);
            return TryRead(ref span, ref readPosition, writePosition, out item);
        }

        /// <summary>
        /// Returns a snapshot of the underlying ring array as a <see cref="ReadOnlySpan{T}"/>
        /// together with the current read and write cursor positions, allowing a caller to
        /// iterate the backlog without mutating this instance.
        /// </summary>
        /// <param name="readPosition">
        /// Receives the current read cursor. Pass this value to
        /// <see cref="TryRead(ref ReadOnlySpan{T}, ref uint, uint, out T)"/> to begin
        /// iterating from the oldest available item.
        /// </param>
        /// <param name="writePosition">
        /// Receives the current write cursor. Pass this value to
        /// <see cref="TryRead(ref ReadOnlySpan{T}, ref uint, uint, out T)"/> as the stop
        /// condition.
        /// </param>
        /// <returns>A <see cref="ReadOnlySpan{T}"/> over the internal ring array.</returns>
        public ReadOnlySpan<T> CurrentState(out uint readPosition, out uint writePosition)
        {
            readPosition = this.readPosition;
            writePosition = this.writePosition;
            return ring;
        }

        /// <summary>
        /// Stateless overload that reads the next item from an externally supplied ring span
        /// using caller-managed cursor values. This allows concurrent snapshot reads without
        /// mutating the <see cref="Backlog{T}"/> instance.
        /// </summary>
        /// <param name="ring">A <see cref="ReadOnlySpan{T}"/> over the ring array.</param>
        /// <param name="readPosition">
        /// The current read cursor. Incremented by one on a successful read.
        /// </param>
        /// <param name="writePosition">The write cursor used as the upper bound.</param>
        /// <param name="item">
        /// When this method returns <see langword="true"/>, contains the item at
        /// <paramref name="readPosition"/>; otherwise the default value of
        /// <typeparamref name="T"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if an item was read; <see langword="false"/> if
        /// <paramref name="readPosition"/> has reached <paramref name="writePosition"/>
        /// (buffer is empty or fully consumed).
        /// </returns>
        public static bool TryRead(ref ReadOnlySpan<T> ring, ref uint readPosition, uint writePosition, out T item)
        {
            bool isEmpty = readPosition >= writePosition;
            if (isEmpty)
            {
                item = default(T);
                return false;
            }

            item = ring[(int)readPosition % ring.Length];
            readPosition++;
            return true;
        }
    }
}
#endif
