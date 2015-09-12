﻿using Prism.Services;
using System.Threading.Tasks;

namespace Prism.Forms.Tests.Mocks
{
    public class PageDialogServiceMock : PageDialogService
    {
        private readonly string pressedButton;

        /// <summary>
        /// Create an instance of <see cref="PageDialogServiceMock"/> with the pressed button on any alert/sheet is <paramref name="pressedButton"/>
        /// </summary>
        /// <param name="pressedButton"></param>
        public PageDialogServiceMock(string pressedButton)
        {
            this.pressedButton = pressedButton;
        }

        public override Task<bool> DisplayAlert(string title, string message, string acceptButton, string cancelButton)
        {
            return Task.FromResult(pressedButton.Equals(acceptButton));
        }

        public override async Task DisplayAlert(string title, string message, string cancelButton)
        {

        }

        public override Task<string> DisplayActionSheet(string title, string cancelButton, string destroyButton, params string[] otherButtons)
        {
            return Task.FromResult(pressedButton);
        }
    }
}
