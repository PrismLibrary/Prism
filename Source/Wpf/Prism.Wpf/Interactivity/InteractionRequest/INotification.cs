

namespace Prism.Interactivity.InteractionRequest
{
    /// <summary>
    /// Represents an interaction request used for notifications.
    /// </summary>
    public interface INotification
    {
        /// <summary>
        /// Gets or sets the title to use for the notification.
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Gets or sets the content of the notification.
        /// </summary>
        object Content { get; set; }
    }
}
