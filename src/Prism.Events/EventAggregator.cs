using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace Prism.Events
{
    /// <summary>
    /// Implements <see cref="IEventAggregator"/>.
    /// </summary>
    public class EventAggregator : IEventAggregator
    {
        private static IEventAggregator _current;

        /// <summary>
        /// Gets or Sets the Current Instance of the <see cref="IEventAggregator"/>
        /// </summary>
        public static IEventAggregator Current
        {
            get => _current ??= new EventAggregator();
            set => _current = value;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="EventAggregator"/>
        /// </summary>
        public EventAggregator()
        {
            if(_current is null)
            {
                _current = this;
            }
        }

        private readonly Dictionary<Type, EventBase> events = new Dictionary<Type, EventBase>();
        // Captures the sync context for the UI thread when constructed on the UI thread
        // in a platform agnostic way so it can be used for UI thread dispatching
        private readonly SynchronizationContext syncContext = SynchronizationContext.Current;

        /// <summary>
        /// Gets the single instance of the event managed by this EventAggregator. Multiple calls to this method with the same <typeparamref name="TEventType"/> returns the same event instance.
        /// </summary>
        /// <typeparam name="TEventType">The type of event to get. This must inherit from <see cref="EventBase"/>.</typeparam>
        /// <returns>A singleton instance of an event object of type <typeparamref name="TEventType"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public TEventType GetEvent<TEventType>() where TEventType : EventBase, new()
        {
            lock (events)
            {
                EventBase existingEvent = null;

                if (!events.TryGetValue(typeof(TEventType), out existingEvent))
                {
                    TEventType newEvent = new TEventType();
                    newEvent.SynchronizationContext = syncContext;
                    events[typeof(TEventType)] = newEvent;

                    return newEvent;
                }
                else
                {
                    return (TEventType)existingEvent;
                }
            }
        }
    }
}
