



namespace Prism.Events
{
    /// <summary>
    /// Specifies on which thread a <see cref="PubSubEvent{TPayload}"/> subscriber will be called.
    /// </summary>
    public enum ThreadOption
    {
        /// <summary>
        /// The call is done on the same thread on which the <see cref="PubSubEvent{TPayload}"/> was published.
        /// </summary>
        PublisherThread,

        /// <summary>
        /// The call is done on the UI thread.
        /// </summary>
        UIThread,

        /// <summary>
        /// The call is done asynchronously on a background thread.
        /// </summary>
        BackgroundThread
    }
}