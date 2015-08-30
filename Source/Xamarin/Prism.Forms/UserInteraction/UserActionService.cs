using System.Threading.Tasks;
using Prism.Navigation;
using Prism.UserInteraction.Abstractions;

namespace Prism.UserInteraction
{
    public class UserActionService : IUserActionService
    {
        private readonly IPageAware _pageAware;

        public UserActionService(IPageAware pageAware)
        {
            _pageAware = pageAware;
        }

        #region Implementation of IUserActionService

        public async Task<string> DisplayActionSheet(string title, string cancelButton, string destroyButton, params string[] otherButtons)
        {
            return await _pageAware.Page.DisplayActionSheet(title, cancelButton, destroyButton, otherButtons);
        }

        #endregion
    }
}
