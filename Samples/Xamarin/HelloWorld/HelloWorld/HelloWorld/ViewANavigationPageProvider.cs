using Prism.Navigation;
using Xamarin.Forms;

namespace HelloWorld
{
    public class ViewANavigationPageProvider : INavigationPageProvider
    {
        public Page CreatePageForNavigation(Page sourcePage, Page targetPage)
        {
            NavigationPage.SetHasNavigationBar(targetPage, true);
            NavigationPage.SetBackButtonTitle(targetPage, "Go Back Sucka");
            NavigationPage.SetHasBackButton(targetPage, true);

            var newPage = new NavigationPage(targetPage);
            newPage.BarBackgroundColor = Color.Green;
            newPage.BarTextColor = Color.White;
            return newPage;
        }
    }
}
