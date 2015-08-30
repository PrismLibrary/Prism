using System.Threading.Tasks;
using Prism.UserInteraction.Abstractions;

namespace Prism.Forms.Tests.Mocks.UserInteractions
{
    public class UserActionServiceMock : IUserActionService
    {
        private readonly string selectedButton;

        public UserActionServiceMock(string selectedButton)
        {
            this.selectedButton = selectedButton;
        }

        #region Implementation of IUserActionService

        public async Task<string> DisplayActionSheet(string title, string cancelButton, string destroyButton, params string[] otherButtons)
        {
            return selectedButton;
        }

        #endregion
    }
}
