using System.Threading.Tasks;
using Prism.Commands;
using Prism.Services;
using Xamarin.Forms;

namespace Prism.Forms.Tests.Mocks
{
    public class PageDialogServiceMock : IPageDialogService
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

        #region Implementation of IUserActionService

        public Task<string> DisplayActionSheet(string title, string cancelButton, string destroyButton, params string[] otherButtons)
        {
            return Task.FromResult(pressedButton);
        }

        #endregion

        #region Implementation of IPageAware

        public Page Page { get; set; }

        #endregion

        #region Implementation of IAlertService

        public Task<bool> DisplayAlert(string title, string message, string acceptButton, string cancelButton)
        {
            return Task.FromResult(pressedButton.Equals(acceptButton));
        }

        public async Task DisplayAlert(string title, string message, string cancelButton)
        {

        }

        #endregion
    }
}
