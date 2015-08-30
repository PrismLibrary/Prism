using System.Threading.Tasks;
using Prism.Navigation;
using Prism.UserInteraction.Abstractions;

namespace Prism.UserInteraction
{
    public class AlertService : IAlertService
    {
        private readonly IPageAware _pageAware;

        public AlertService(IPageAware pageAware)
        {
            _pageAware = pageAware;
        }

        #region Implementation of IAlertService

        public async Task<bool> DisplayAlert(string title, string message, string accepetButton, string cancelButton)
        {
            return await _pageAware.Page.DisplayAlert(title, message, accepetButton, cancelButton);
        }

        public async Task DisplayAlert(string title, string message, string cancelButton)
        {
            await _pageAware.Page.DisplayAlert(title, message, cancelButton);
        }

        #endregion
    }
}
