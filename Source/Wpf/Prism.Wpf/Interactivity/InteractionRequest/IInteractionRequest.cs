

using System;

namespace Prism.Interactivity.InteractionRequest
{
    /// <summary>
    /// Represents a request from user interaction.
    /// </summary>
    /// <remarks>
    /// View models can expose interaction request objects through properties and raise them when user interaction
    /// is required so views associated with the view models can materialize the user interaction using an appropriate
    /// mechanism.
    /// </remarks>
    [Obsolete("Please use the new IDialogService for an improved dialog experience.")]
    public interface IInteractionRequest
    {
        /// <summary>
        /// Fired when the interaction is needed.
        /// </summary>
        event EventHandler<InteractionRequestedEventArgs> Raised;
    }
}
