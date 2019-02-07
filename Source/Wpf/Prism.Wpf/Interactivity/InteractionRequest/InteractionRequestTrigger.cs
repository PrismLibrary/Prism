using System;
using System.Windows.Interactivity;

namespace Prism.Interactivity.InteractionRequest
{
    /// <summary>
    /// Custom event trigger for using with <see cref="IInteractionRequest"/> objects.
    /// </summary>
    /// <remarks>
    /// The standard <see cref="System.Windows.Interactivity.EventTrigger"/> class can be used instead, as long as the 'Raised' event 
    /// name is specified.
    /// </remarks>
    [Obsolete("Please use the new IDialogService for an improved dialog experience.")]
    public class InteractionRequestTrigger : EventTrigger
    {
        /// <summary>
        /// Specifies the name of the Event this EventTriggerBase is listening for.
        /// </summary>
        /// <returns>This implementation always returns the Raised event name for ease of connection with <see cref="IInteractionRequest"/>.</returns>
        protected override string GetEventName()
        {
            return "Raised";
        }
    }
}
