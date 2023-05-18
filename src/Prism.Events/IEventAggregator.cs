



namespace Prism.Events
{
    /// <summary>
    /// Defines an interface to get instances of an event type.
    /// </summary>
    public interface IEventAggregator
    {
        /// <summary>
        /// Gets an instance of an event type.
        /// </summary>
        /// <typeparam name="TEventType">The type of event to get.</typeparam>
        /// <returns>An instance of an event object of type <typeparamref name="TEventType"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        TEventType GetEvent<TEventType>() where TEventType : EventBase, new();
    }
}
