﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Prism.Common;
using Xamarin.Forms;

namespace Prism.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class PageDialogService : IPageDialogService
    {
        IApplicationProvider _applicationProvider;

        public PageDialogService(IApplicationProvider applicationProvider)
        {
            _applicationProvider = applicationProvider;
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
            return _applicationProvider.MainPage.DisplayAlert(title, message, acceptButton, cancelButton);
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
        /// Displays a native platform action sheet, allowing the application user to choose from serveral buttons.
        /// </summary>
        /// <param name="title">Title to display in view.</param>
        /// <param name="cancelButton">Text for the cancel button.</param>
        /// <param name="destroyButton">Text for the ok button.</param>
        /// <param name="otherButtons">Text for other buttons.</param>
        /// <returns>Text for the pressed button</returns>
        public virtual Task<string> DisplayActionSheetAsync(string title, string cancelButton, string destroyButton, params string[] otherButtons)
        {
            return GetCurrentPage().DisplayActionSheet(title, cancelButton, destroyButton, otherButtons);
        }

        /// <summary>
        /// Displays a native platform action sheet, allowing the application user to choose from serveral buttons.
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
            if (buttons == null || buttons.All(b => b == null))
                throw new ArgumentException("At least one button needs to be supplied", nameof(buttons));

            var destroyButton = buttons.FirstOrDefault(button => button != null && button.IsDestroy);
            var cancelButton = buttons.FirstOrDefault(button => button != null && button.IsCancel);
            var otherButtonsText = buttons.Where(button => button != null && !(button.IsDestroy || button.IsCancel)).Select(b => b.Text).ToArray();

            var pressedButton = await DisplayActionSheetAsync(title, cancelButton?.Text, destroyButton?.Text, otherButtonsText);

            foreach (var button in buttons.Where(button => button != null && button.Text.Equals(pressedButton)))
            {
                button.PressButton();
                return;
            }
        }

        /// <summary>
        /// Displays a native platform promt, allowing the application user to enter a string.
        /// </summary>
        /// <param name="title">Title to display</param>
        /// <param name="message">Message to display</param>
        /// <param name="accept">Text for the accept button</param>
        /// <param name="cancel">Text for the cancel button</param>
        /// <param name="placeholder">Placeholder text to display in the prompt</param>
        /// <param name="maxLength">Maximum length of the user response</param>
        /// <param name="keyboard">Keyboard type to use for the user response</param>
        /// <param name="initialValue">Pre-defined response that will be displayed, and which can be edited</param>
        /// <returns><c>string</c> entered by the user. <c>null</c> if cancel is pressed</returns>
        public virtual Task<string> DisplayPromptAsync(string title, string message, string accept = "OK", string cancel = "Cancel", string placeholder = default, int maxLength = -1, Xamarin.Forms.Keyboard keyboard = default, string initialValue = "")
        {
            return _applicationProvider.MainPage.DisplayPromptAsync(title, message, accept, cancel, placeholder, maxLength, keyboard, initialValue);
        }

        private Page GetCurrentPage()
        {
            Page page;
            if (_applicationProvider.MainPage.Navigation.ModalStack.Count > 0)
                page = _applicationProvider.MainPage.Navigation.ModalStack.LastOrDefault();
            else
                page = _applicationProvider.MainPage.Navigation.NavigationStack.LastOrDefault();

            if (page == null)
                page = _applicationProvider.MainPage;

            return page;
        }
    }
}
