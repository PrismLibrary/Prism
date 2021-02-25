using System;
using System.Linq;
using System.Threading.Tasks;
using Prism.AppModel;
using Prism.Common;
using XFFlow = Xamarin.Forms.FlowDirection;

namespace Prism.Services
{
    /// <summary>
    /// Implementation of the <see cref="IPageDialogService"/>
    /// </summary>
    public class PageDialogService : IPageDialogService
    {
        /// <summary>
        /// Gets the <see cref="IApplicationProvider"/>.
        /// </summary>
        protected IApplicationProvider _applicationProvider { get; }

        /// <summary>
        /// Gets the <see cref="IKeyboardMapper"/>.
        /// </summary>
        protected IKeyboardMapper _keyboardMapper { get; }

        /// <summary>
        /// Creates a new <see cref="IPageDialogService"/>
        /// </summary>
        /// <param name="applicationProvider">The <see cref="IApplicationProvider"/>.</param>
        /// <param name="keyboardMapper">The <see cref="IKeyboardMapper"/>.</param>
        public PageDialogService(IApplicationProvider applicationProvider, IKeyboardMapper keyboardMapper)
        {
            _applicationProvider = applicationProvider;
            _keyboardMapper = keyboardMapper;
        }

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
        public virtual Task<bool> DisplayAlertAsync(string title, string message, string acceptButton, string cancelButton)
        {
            return DisplayAlertAsync(title, message, acceptButton, cancelButton, FlowDirection.MatchParent);
        }

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
        /// <param name="flowDirection">The Text flow direction.</param>
        /// <returns><c>true</c> if non-destructive button pressed; otherwise <c>false</c>/></returns>
        public virtual Task<bool> DisplayAlertAsync(string title, string message, string acceptButton, string cancelButton, FlowDirection flowDirection)
        {
            return _applicationProvider.MainPage.DisplayAlert(title, message, acceptButton, cancelButton, (XFFlow)flowDirection);
        }

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
        public virtual Task DisplayAlertAsync(string title, string message, string cancelButton)
        {
            return _applicationProvider.MainPage.DisplayAlert(title, message, cancelButton);
        }

        /// <summary>
        /// Presents an alert dialog to the application user with a single cancel button.
        /// </summary>
        /// <para>
        /// The <paramref name="message"/> can be empty.
        /// </para>
        /// <param name="title">Title to display.</param>
        /// <param name="message">Message to display.</param>
        /// <param name="cancelButton">Text for the cancel button.</param>
        /// <param name="flowDirection">The Text flow direction.</param>
        /// <returns></returns>
        public virtual Task DisplayAlertAsync(string title, string message, string cancelButton, FlowDirection flowDirection)
        {
            return _applicationProvider.MainPage.DisplayAlert(title, message, cancelButton, (XFFlow)flowDirection);
        }

        /// <summary>
        /// Displays a native platform action sheet, allowing the application user to choose from several buttons.
        /// </summary>
        /// <param name="title">Title to display in view.</param>
        /// <param name="cancelButton">Text for the cancel button.</param>
        /// <param name="destroyButton">Text for the ok button.</param>
        /// <param name="otherButtons">Text for other buttons.</param>
        /// <returns>Text for the pressed button</returns>
        public virtual Task<string> DisplayActionSheetAsync(string title, string cancelButton, string destroyButton, params string[] otherButtons)
        {
            return DisplayActionSheetAsync(title, cancelButton, destroyButton, FlowDirection.MatchParent, otherButtons);
        }

        /// <summary>
        /// Displays a native platform action sheet, allowing the application user to choose from several buttons.
        /// </summary>
        /// <param name="title">Title to display in view.</param>
        /// <param name="cancelButton">Text for the cancel button.</param>
        /// <param name="destroyButton">Text for the ok button.</param>
        /// <param name="flowDirection">The Text flow direction.</param>
        /// <param name="otherButtons">Text for other buttons.</param>
        /// <returns>Text for the pressed button</returns>
        public virtual Task<string> DisplayActionSheetAsync(string title, string cancelButton, string destroyButton, FlowDirection flowDirection, params string[] otherButtons)
        {
            return _applicationProvider.MainPage.DisplayActionSheet(title, cancelButton, destroyButton, (XFFlow)flowDirection, otherButtons);
        }

        /// <summary>
        /// Displays a native platform action sheet, allowing the application user to choose from several buttons.
        /// </summary>
        /// <para>
        /// The text displayed in the action sheet will be the value for <see cref="IActionSheetButton.Text"/> and when pressed
        /// the <see cref="System.Windows.Input.ICommand"/> or <see cref="Action"/> will be executed.
        /// </para>
        /// <param name="title">Text to display in action sheet</param>
        /// <param name="buttons">Buttons displayed in action sheet</param>
        /// <returns></returns>
        public virtual async Task DisplayActionSheetAsync(string title, params IActionSheetButton[] buttons)
        {
            await DisplayActionSheetAsync(title, FlowDirection.MatchParent, buttons);
        }

        /// <summary>
        /// Displays a native platform action sheet, allowing the application user to choose from several buttons.
        /// </summary>
        /// <para>
        /// The text displayed in the action sheet will be the value for <see cref="IActionSheetButton.Text"/> and when pressed
        /// the callback action will be executed.
        /// </para>
        /// <param name="title">Text to display in action sheet</param>
        /// <param name="flowDirection">The Text flow direction.</param>
        /// <param name="buttons">Buttons displayed in action sheet</param>
        /// <returns></returns>
        public virtual async Task DisplayActionSheetAsync(string title, FlowDirection flowDirection, params IActionSheetButton[] buttons)
        {
            if (buttons == null || buttons.All(b => b == null))
                throw new ArgumentException("At least one button needs to be supplied", nameof(buttons));

            var destroyButton = buttons.FirstOrDefault(button => button != null && button.IsDestroy);
            var cancelButton = buttons.FirstOrDefault(button => button != null && button.IsCancel);
            var otherButtonsText = buttons.Where(button => button != null && !(button.IsDestroy || button.IsCancel)).Select(b => b.Text).ToArray();

            var pressedButton = await DisplayActionSheetAsync(title, cancelButton?.Text, destroyButton?.Text, flowDirection, otherButtonsText);

            foreach (var button in buttons.Where(button => button != null && button.Text.Equals(pressedButton)))
            {
                button.PressButton();
                return;
            }
        }

        /// <summary>
        /// Displays a native platform prompt, allowing the application user to enter a string.
        /// </summary>
        /// <param name="title">Title to display</param>
        /// <param name="message">Message to display</param>
        /// <param name="accept">Text for the accept button</param>
        /// <param name="cancel">Text for the cancel button</param>
        /// <param name="placeholder">Placeholder text to display in the prompt</param>
        /// <param name="maxLength">Maximum length of the user response</param>
        /// <param name="keyboardType">Keyboard type to use for the user response</param>
        /// <param name="initialValue">Pre-defined response that will be displayed, and which can be edited</param>
        /// <returns><c>string</c> entered by the user. <c>null</c> if cancel is pressed</returns>
        public virtual Task<string> DisplayPromptAsync(string title, string message, string accept = "OK", string cancel = "Cancel", string placeholder = default, int maxLength = -1, KeyboardType keyboardType = KeyboardType.Default, string initialValue = "")
        {
            var keyboard = _keyboardMapper.Map(keyboardType);
            return _applicationProvider.MainPage.DisplayPromptAsync(title, message, accept, cancel, placeholder, maxLength, keyboard, initialValue);
        }
    }
}
