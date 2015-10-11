using Prism.Navigation;
using Xamarin.Forms;

namespace HelloWorld
{
    public class ViewANavigationPageProvider : INavigationPageProvider
    {
        Page _page = null;

        public void Initialize(Page sourcePage, Page targetPage)
        {
            _page = targetPage;
            NavigationPage.SetHasNavigationBar(_page, true);
            NavigationPage.SetBackButtonTitle(_page, "Go Back Sucka");
            NavigationPage.SetHasBackButton(_page, true);
        }

        public Page CreatePageForNavigation()
        {
            var newPage = new NavigationPage(_page);
            newPage.BarBackgroundColor = Color.Green;
            newPage.BarTextColor = Color.White;
            return newPage;
        }
    }
}
