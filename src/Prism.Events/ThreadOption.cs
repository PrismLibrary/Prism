



namespace Prism.Events
{
    /// <summary>
    /// Specifies on which thread a <see cref="PubSubEvent{TPayload}"/> subscriber will be called.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This enumeration is used when subscribing to Pub/Sub events to control where the subscription callback is executed.
    /// The choice affects threading behavior and should be considered based on the nature of the work being performed.
    /// </para>
    /// </remarks>
    public enum ThreadOption
    {
        /// <summary>
        /// The call is done on the same thread on which the <see cref="PubSubEvent{TPayload}"/> was published.
        /// </summary>
        /// <remarks>
        /// This is the default and most performant option. Use this when the callback doesn't need to interact with the UI.
        /// </remarks>
        PublisherThread,

        /// <summary>
        /// The call is done on the UI thread.
        /// </summary>
        /// <remarks>
        /// Use this option when the callback needs to update UI elements. On XAML platforms, this typically means the thread
        /// that created the UI dispatcher. On non-UI platforms, this behaves like PublisherThread.
        /// </remarks>
        UIThread,

        /// <summary>
        /// The call is done asynchronously on a background thread.
        /// </summary>
        /// <remarks>
        /// Use this option when the callback performs long-running operations that shouldn't block the publisher thread.
        /// </remarks>
        BackgroundThread
    }
}
