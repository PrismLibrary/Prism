using System.Threading.Tasks;
using Prism.UserInteraction.Abstractions;

namespace Prism.UserInteraction
{
    /// <summary>
    /// Facade to use user interaction services.
    /// </summary>
    public class UserInteractionFacade : IUserInteractionFacade
    {
        private readonly IAlertService alertService;
        private readonly IUserActionService userActionService;

        public UserInteractionFacade(IAlertService alertService, IUserActionService userActionService)
        {
            this.alertService = alertService;
            this.userActionService = userActionService;
        }

        public async Task DisplayAlert(string title, string message, string cancelButton)
        {
            await alertService.DisplayAlert(title, message, cancelButton);
        }

        public async Task<bool> DisplayAlert(string title, string message, string cancelButton, string okButton)
        {
            return await alertService.DisplayAlert(title, message, okButton, cancelButton);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="cancelButton"></param>
        /// <param name="destroyButton"></param>
        /// <returns>The index for the pressed button. 
        /// <para>
        /// <paramref name="cancelButton"/> is assigned index zero and <paramref name="destroyButton"/> is assigned one.
        /// </para>
        /// </returns>
        public async Task<int> DisplayActionSheet(string title, string cancelButton, string destroyButton = null)
        {
            return await DisplayActionSheet(title, cancelButton, destroyButton, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="cancelButton"></param>
        /// <param name="destroyButton"></param>
        /// <param name="otherButtons"></param>
        /// <returns>The index for the pressed button. 
        /// <para>
        /// <paramref name="cancelButton"/> is assigned index zero and <paramref name="destroyButton"/> is assigned one.
        /// The <paramref name="otherButtons"/> are assigned there respective index plus two.
        /// </para>
        /// </returns>
        public async Task<int> DisplayActionSheet(string title, string cancelButton, string destroyButton = null,
            params string[] otherButtons)
        {
            var pressedButtonText = await userActionService.DisplayActionSheet(title, cancelButton, destroyButton, otherButtons);
            // Check if destroy button was pressed
            if (pressedButtonText.Equals(destroyButton))
            {
                return 1;
            }
            if (otherButtons == null)
            {
                return 0;
            }
            for (var i = 0; i < otherButtons.Length; ++i)
            {
                if (pressedButtonText.Equals(otherButtons[i]))
                {
                    return i + 2;
                }
            }
            // Cancel button pressed
            return 0;
        }
    }
}