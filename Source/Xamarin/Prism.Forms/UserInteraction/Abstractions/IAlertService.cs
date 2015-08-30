using System.Threading.Tasks;

namespace Prism.UserInteraction.Abstractions
{

    /// <summary>
    /// Service to display an alert view.
    /// </summary>
    public interface IAlertService
    {
        /// <summary>
        /// Display an alert view with two buttons with <paramref name="title"/> and <paramref name="message"/>.
        /// </summary>
        /// <para>
        /// The <paramref name="message"/> can be empty.
        /// </para>
        /// <param name="title">Title to display.</param>
        /// <param name="message">Message to display.</param>
        /// <param name="accepetButton">Text for the accept button.</param>
        /// <param name="cancelButton">Text for the cancel button.</param>
        /// <returns><c>true</c> iff non-destructive button pressed; otherwise <c>false</c>/></returns>
        Task<bool> DisplayAlert(string title, string message, string accepetButton, string cancelButton);

        /// <summary>
        /// Display an alert view with one button with <paramref name="title"/> and <paramref name="message"/>.
        /// </summary>
        /// <para>
        /// The <paramref name="message"/> can be empty.
        /// </para>
        /// <param name="title">Title to display.</param>
        /// <param name="message">Message to display.</param>
        /// <param name="cancelButton">Text for the cancel button.</param>
        /// <returns></returns>
        Task DisplayAlert(string title, string message, string cancelButton);
    }
}
