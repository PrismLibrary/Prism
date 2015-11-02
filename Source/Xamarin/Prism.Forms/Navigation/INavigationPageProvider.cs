using Xamarin.Forms;

namespace Prism.Navigation
{
    /// <summary>
    /// Defines the methods required to implement a page provider while navigating to a <see cref="Xamarin.Forms.Page"/>.
    /// </summary>
    /// <remarks>The class that implements this interface should be used with the <see cref="Prism.Navigation.NavigationPageProviderAttribute"/>. 
    /// The term NavigationPage in this interface name is not related to the <see cref="<see cref="Xamarin.Forms.NavigationPage"/>"/>.
    /// The term NavigationPage refers to a different page type which may wrap, or replace, the current target page during navigation.
    /// </remarks>
    public interface INavigationPageProvider
    {
        /// <summary>
        /// Use this method to wrap, or replace, the current target <see cref="Xamarin.Forms.Page"/> with a new page, such as a <see cref="Xamarin.Forms.NavigationPage"/>. The source page is also provided
        /// to enable you to make decisions based on the source of the navigation.
        /// </summary>
        /// <param name="sourcePage">The <see cref="Xamarin.Forms.Page"/> that is the source of the navigation. This is the page being navigated away from.</param>
        /// <param name="targetPage">The <see cref="Xamarin.Forms.Page"/> that is the target for navigation. This is the Page being navigated to.</param>
        /// <returns>A new page</returns>
        /// <remarks>Returning null will have no affect on navigation. The targetPage will remain the navigation target.</remarks>
        /// <example>
        /// public Page CreatePageForNavigation(Page sourcePage, Page targetPage)
        /// {
        ///     NavigationPage.SetHasNavigationBar(targetPage, true);
        ///     NavigationPage.SetBackButtonTitle(targetPage, "Go Back Sucka");
        ///     NavigationPage.SetHasBackButton(targetPage, true);
        /// 
        ///     var newPage = new NavigationPage(targetPage);
        ///     newPage.BarBackgroundColor = Color.Green;
        ///     newPage.BarTextColor = Color.White;
        ///     return newPage;
        /// }
        /// </example>
        Page CreatePageForNavigation(Page sourcePage, Page targetPage);
    }
}
