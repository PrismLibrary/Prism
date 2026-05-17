



namespace Prism.Events
{
    /// <summary>
    /// Defines an interface to get instances of an event type.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="IEventAggregator"/> is the main entry point for the Pub/Sub event system in Prism.
    /// It maintains a singleton collection of event instances and provides a way to retrieve or create events by type.
    /// </para>
    /// <para>
    /// Typically, you inject <see cref="IEventAggregator"/> into your ViewModels and use it to publish and subscribe to loosely-coupled events.
    /// </para>
    /// </remarks>
    public interface IEventAggregator
    {
        /// <summary>
        /// Gets an instance of an event type.
        /// </summary>
        /// <typeparam name="TEventType">The type of event to get. Must be a type that inherits from <see cref="EventBase"/> and has a parameterless constructor.</typeparam>
        /// <returns>An instance of an event object of type <typeparamref name="TEventType"/>. Multiple calls with the same type return the same instance (singleton).</returns>
        /// <remarks>
        /// This method returns a singleton instance of the requested event type. If the event does not yet exist, it will be created.
        /// Subsequent calls with the same type parameter will return the same instance.
        /// </remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        TEventType GetEvent<TEventType>() where TEventType : EventBase, new();
    }
}
