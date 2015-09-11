using System.Threading.Tasks;
using Prism.Commands;
using Prism.Services;
using Xamarin.Forms;

namespace Prism.Forms.Tests.Mocks
{
    public class ActionSheetButton : IActionSheetButton
    {
        #region Implementation of IActionSheetButton

        public bool IsDestroy { get; internal set; }
        public bool IsCancel { get; internal set; }
        public string Text { get; internal set; }
        public DelegateCommand Callback { get; internal set; }

        #endregion

        public static ActionSheetButton CreateCancelButton(string text, DelegateCommand command)
        {
            return new ActionSheetButton
            {
                Text = text,
                IsCancel = true,
                Callback = command
            };
        }

        public static ActionSheetButton CreateDestructiveButton(string text, DelegateCommand command)
        {
            return new ActionSheetButton
            {
                Text = text,
                IsDestroy = true,
                Callback = command
            };
        }

        public static ActionSheetButton CreateButton(string text, DelegateCommand command)
        {
            return new ActionSheetButton
            {
                Text = text,
                Callback = command
            };
        }
    }

    public class PopupServiceMock : IPopupService
    {
        private readonly string pressedButton;

        /// <summary>
        /// Create an instance of <see cref="PopupServiceMock"/> with the pressed button on any alert/sheet is <paramref name="pressedButton"/>
        /// </summary>
        /// <param name="pressedButton"></param>
        public PopupServiceMock(string pressedButton)
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
