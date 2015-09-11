using System.Threading.Tasks;
using Prism.Navigation;
using Xamarin.Forms;

namespace Prism.Services
{
    public class PageDialogService : IPageDialogService
    {
        private Page _page;

        #region Implementation of IAlertService

        public async Task<bool> DisplayAlert(string title, string message, string acceptButton, string cancelButton)
        {
            return await _page.DisplayAlert(title, message, acceptButton, cancelButton);
        }

        public async Task DisplayAlert(string title, string message, string cancelButton)
        {
            await _page.DisplayAlert(title, message, cancelButton);
        }

        #endregion

        #region Implementaiton of IActionSheetService

        public async Task<string> DisplayActionSheet(string title, string cancelButton, string destroyButton, params string[] otherButtons)
        {
            return await _page.DisplayActionSheet(title, cancelButton, destroyButton, otherButtons);
        }

        #endregion

        #region Implementation of IPageAware

        Page IPageAware.Page
        {
            get { return _page ?? (_page = Application.Current.MainPage); }
            set { _page = value; }
        }

        #endregion
    }
}