using System;

namespace Prism.Interactivity.InteractionRequest
{
    /// <summary>
    /// Represents an interaction request used for confirmations.
    /// </summary>
    [Obsolete("Please use the new IDialogService for an improved dialog experience.")]
    public interface IConfirmation : INotification
    {
        /// <summary>
        /// Gets or sets a value indicating that the confirmation is confirmed.
        /// </summary>
        bool Confirmed { get; set; }
    }
}
