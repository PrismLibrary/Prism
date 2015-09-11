using System.Threading.Tasks;
using Prism.Navigation;

namespace Prism.Services
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPageDialogService : IPageAware
    {
        /// <summary>
        /// Display an alert view with two buttons with <paramref name="title"/> and <paramref name="message"/>.
        /// </summary>
        /// <para>
        /// The <paramref name="message"/> can be empty.
        /// </para>
        /// <param name="title">Title to display.</param>
        /// <param name="message">Message to display.</param>
        /// <param name="acceptButton">Text for the accept button.</param>
        /// <param name="cancelButton">Text for the cancel button.</param>
        /// <returns><c>true</c> iff non-destructive button pressed; otherwise <c>false</c>/></returns>
        Task<bool> DisplayAlert(string title, string message, string acceptButton, string cancelButton);

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

        /// <summary>
        /// Display a view with multiple options
        /// </summary>
        /// <param name="title">Title to display in view.</param>
        /// <param name="cancelButton">Text for the cancel button.</param>
        /// <param name="destroyButton">Text for the ok button.</param>
        /// <param name="otherButtons">Text for other buttons.</param>
        /// <returns>Text for the pressed button</returns>
        Task<string> DisplayActionSheet(string title, string cancelButton, string destroyButton, params string[] otherButtons);
    }
}
