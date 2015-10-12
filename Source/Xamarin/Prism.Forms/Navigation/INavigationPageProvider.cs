using Xamarin.Forms;

namespace Prism.Navigation
{
    /// <summary>
    /// Defines the methods required to implement a page provider while navigating to a Page.
    /// </summary>
    /// <remarks>The class that implements this interface should be used with the NavigationPageProviderAttribute. 
    /// The term NavigationPage in this interface name is not related to the Xamarin.Forms.NavigationPage class.
    /// The term NavigationPage refers to a different page type which may wrap, or replace, the current target page during navigation.
    /// </remarks>
    public interface INavigationPageProvider
    {
        /// <summary>
        /// Initializes the INavigationPageProvider and allows various properties of the sourcePage and targetPage can be read/set prior to navigation completion.
        /// </summary>
        /// <param name="sourcePage">The Page that is the source of the navigation. This is the page being navigated away from.</param>
        /// <param name="targetPage">The Page that is the target for navigation. This is the Page being navigated to.</param>
        void Initialize(Page sourcePage, Page targetPage);

        /// <summary>
        /// Use this method to wrap, or replace, the current target Page with a new page type, such as a NavigationPage.
        /// </summary>
        /// <example>
        /// public Page CreatePageForNavigation()
        /// {
        ///     var newPage = new NavigationPage(_page);
        ///     newPage.BarBackgroundColor = Color.Green;
        ///     newPage.BarTextColor = Color.White;
        ///     return newPage;
        /// }
        /// </example>
        /// <returns>A new page</returns>
        /// <remarks>Return null if you do not need to wrap, or replace, the targetPage.</remarks>
        Page CreatePageForNavigation();
    }
}
