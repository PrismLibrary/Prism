﻿using Prism.Common;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Prism.Services
{
    public class PageDialogService : IPageDialogService, IPageAware
    {
        Page _page;
        Page IPageAware.Page
        {
            get { return _page ?? (_page = Application.Current.MainPage); }
            set { _page = value; }
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
        public virtual async Task<bool> DisplayAlert(string title, string message, string acceptButton, string cancelButton)
        {
            return await _page.DisplayAlert(title, message, acceptButton, cancelButton);
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
        public virtual async Task DisplayAlert(string title, string message, string cancelButton)
        {
            await _page.DisplayAlert(title, message, cancelButton);
        }

        /// <summary>
        /// Displays a native platform action sheet, allowing the application user to choose from serveral buttons.
        /// </summary>
        /// <param name="title">Title to display in view.</param>
        /// <param name="cancelButton">Text for the cancel button.</param>
        /// <param name="destroyButton">Text for the ok button.</param>
        /// <param name="otherButtons">Text for other buttons.</param>
        /// <returns>Text for the pressed button</returns>
        public virtual async Task<string> DisplayActionSheet(string title, string cancelButton, string destroyButton, params string[] otherButtons)
        {
            return await _page.DisplayActionSheet(title, cancelButton, destroyButton, otherButtons);
        }

        /// <summary>
        /// Displays a native platform action sheet, allowing the application user to choose from serveral buttons.
        /// </summary>
        /// <para>
        /// The text displayed in the action sheet will be the value for <see cref="IActionSheetButton.Text"/> and when pressed
        /// the <see cref="IActionSheetButton.Callback"/> will be executed.
        /// </para>
        /// <param name="service">Instance of <see cref="IPageDialogService"/></param>
        /// <param name="title">Text to display in action sheet</param>
        /// <param name="buttons">Buttons displayed in action sheet</param>
        /// <returns></returns>
        public virtual async Task DisplayActionSheet(string title, params IActionSheetButton[] buttons)
        {
            if (buttons == null || buttons.All(b => b == null))
                throw new ArgumentException("At least one button needs to be supplied", "buttons");

            var destroyButton = buttons.FirstOrDefault(button => button != null && button.IsDestroy);
            var cancelButton = buttons.FirstOrDefault(button => button != null && button.IsCancel);
            var otherButtonsText = buttons.Where(button => button != null && !(button.IsDestroy || button.IsCancel)).Select(b => b.Text).ToArray();

            //Appveyor doesn't like this, so until they support it, we need to do it the hard way.
            //var pressedButton = await DisplayActionSheet(title, cancelButton?.Text, destroyButton?.Text, otherButtonsText);
            //TODO: delete when Appveyor suppports new C# 6 features
            var pressedButton = await DisplayActionSheet(title, cancelButton != null ? cancelButton.Text : null, destroyButton != null ? destroyButton.Text : null, otherButtonsText);

            foreach (var button in buttons.Where(button => button != null && button.Text.Equals(pressedButton)))
            {
                if (button.Command.CanExecute(button.Text))
                    button.Command.Execute(button.Text);

                return;
            }
        }
    }
}
