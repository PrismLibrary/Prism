using System.Reflection;
using System.Threading.Tasks;
using Prism.Common;
using Xamarin.Forms;

namespace Prism.Forms.Tests.Mocks
{
    public class ApplicationProviderMock : IApplicationProvider
    {
        private Page _mainPage;

        public ApplicationProviderMock()
        {
            MainPage = new ContentPage()
            {
                Title = "MainPage"
            };
        }

        public ApplicationProviderMock(Page page)
        {
            MainPage = page;
        }

        public Page MainPage
        {
            get { return _mainPage; }
            set
            {
                _mainPage = value;
                if (_mainPage != null)
                {
                    var pageNavigation = new PageNavigation();
                    _mainPage.SetInner(pageNavigation);
                    pageNavigation.PushAsync(_mainPage);
                }
            }
        }
    }
}
