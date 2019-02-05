

using Prism.Interactivity.InteractionRequest;
using System;

namespace Prism.Interactivity.InteractionRequest
{
    /// <summary>
    /// Interface used by the <see cref="PopupWindowAction"/>.
    /// If the DataContext object of a view that is shown with this action implements this interface
    /// it will be populated with the <see cref="INotification"/> data of the interaction request 
    /// as well as an <see cref="Action"/> to finish the request upon invocation.
    /// </summary>
    [Obsolete("Please use the new IDialogService for an improved dialog experience.")]
    public interface IInteractionRequestAware
    {
        /// <summary>
        /// The <see cref="INotification"/> passed when the interaction request was raised.
        /// </summary>
        INotification Notification { get; set; }

        /// <summary>
        /// An <see cref="Action"/> that can be invoked to finish the interaction.
        /// </summary>
        Action FinishInteraction { get; set; }
    }
}
