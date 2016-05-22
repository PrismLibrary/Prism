using System.Threading.Tasks;

namespace Prism.Services
{
    /// <summary>
    /// A service which provides access to the DisplayAlert and DisplayActionSheet off of the Xamarin.Forms.Page class.
    /// </summary>
    public interface IPageDialogService
    {
        /// <summary>
        /// Presents an alert dialog to the application user with an accept and a cancel button.
        /// </summary>
        /// <para>
        /// The <paramref name="message"/> can be empty.
        /// </para>
        /// <param name="title">Title to display.</param>
        /// <param name="message">Message to display.</param>
        /// <param name="acceptButton">Text for the accept button.</param>
        /// <param name="cancelButton">Text for the cancel button.</param>
        /// <returns><c>true</c> if non-destructive button pressed; otherwise <c>false</c>/></returns>
        Task<bool> DisplayAlertAsync(string title, string message, string acceptButton, string cancelButton);

        /// <summary>
        /// Presents an alert dialog to the application user with a single cancel button.
        /// </summary>
        /// <para>
        /// The <paramref name="message"/> can be empty.
        /// </para>
        /// <param name="title">Title to display.</param>
        /// <param name="message">Message to display.</param>
        /// <param name="cancelButton">Text for the cancel button.</param>
        /// <returns></returns>
        Task DisplayAlertAsync(string title, string message, string cancelButton);

        /// <summary>
        /// Displays a native platform action sheet, allowing the application user to choose from serveral buttons.
        /// </summary>
        /// <param name="title">Title to display in view.</param>
        /// <param name="cancelButton">Text for the cancel button.</param>
        /// <param name="destroyButton">Text for the ok button.</param>
        /// <param name="otherButtons">Text for other buttons.</param>
        /// <returns>Text for the pressed button</returns>
        Task<string> DisplayActionSheetAsync(string title, string cancelButton, string destroyButton, params string[] otherButtons);

        /// <summary>
        /// Displays a native platform action sheet, allowing the application user to choose from serveral buttons.
        /// </summary>
        /// <para>
        /// The text displayed in the action sheet will be the value for <see cref="IActionSheetButton.Text"/> and when pressed
        /// the <see cref="IActionSheetButton.Command"/> will be executed.
        /// </para>
        /// <param name="title">Text to display in action sheet</param>
        /// <param name="buttons">Buttons displayed in action sheet</param>
        /// <returns></returns>
        Task DisplayActionSheetAsync(string title, params IActionSheetButton[] buttons);
    }
}
