using Prism.Common;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Prism.Navigation.TabbedPages
{
    public static class INavigationServiceExtensions
    {
        /// <summary>
        /// Selects a Tab of the TabbedPage parent.
        /// </summary>
        /// <param name="name">The name of the tab to select</param>
        /// <param name="parameters">The navigation parameters</param>
        public static async Task SelectTab(this INavigationService navigationService, string name, INavigationParameters parameters = null)
        {
            var currentPage = ((IPageAware)navigationService).Page;

            var canNavigate = await PageUtilities.CanNavigateAsync(currentPage, parameters);
            if (!canNavigate)
                return;

            TabbedPage tabbedPage = null;

            if (currentPage.Parent is TabbedPage parent)
            {
                tabbedPage = parent;
            }
            else if (currentPage.Parent is NavigationPage navPage)
            {
                if (navPage.Parent != null && navPage.Parent is TabbedPage parent2)
                {
                    tabbedPage = parent2;
                }
            }

            if (tabbedPage == null)
                return;

            var tabToSelectedType = PageNavigationRegistry.GetPageType(UriParsingHelper.GetSegmentName(name));

            Page target = null;
            foreach (var child in tabbedPage.Children)
            {
                if (child.GetType() == tabToSelectedType)
                {
                    target = child;
                    break;
                }

                if (child is NavigationPage)
                {
                    if (((NavigationPage)child).CurrentPage.GetType() == tabToSelectedType)
                    {
                        target = child;
                        break;
                    }
                }
            }

            var tabParameters = UriParsingHelper.GetSegmentParameters(name, parameters);

            PageUtilities.OnNavigatingTo(target, tabParameters);
            tabbedPage.CurrentPage = target;
            PageUtilities.OnNavigatedFrom(currentPage, tabParameters);
            PageUtilities.OnNavigatedTo(target, tabParameters);
        }
    }
}
