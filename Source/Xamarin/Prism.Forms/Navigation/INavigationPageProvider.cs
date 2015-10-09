using Xamarin.Forms;

namespace Prism.Navigation
{
    /// <summary>
    /// Defines the methods required to implement a page provider while navigating to a Page.
    /// </summary>
    /// <remarks>The class that implements this interface should be used with the NavigationPageProviderAttribute.  This will allow you to set properties on the taregt page before navigation is completed, 
    /// as well as create a new page to navigate to instead.  The term NavigationPage in this interface name is not related to the Xamarin.Forms.NavigationPage class.
    /// The term NavigationPage refers to a different page type which may wrap, or replace, the current target page during navigation.
    /// </remarks>
    public interface INavigationPageProvider
    {
        /// <summary>
        /// Provides a method in which various properties of the target page can be set or customized prior to navigation completion.
        /// </summary>
        /// <param name="page"></param>
        void Initialize(Page page);

        /// <summary>
        /// Use this method to wrap, or replace, the current target Page with a new page type, such as a NavigationPage.
        /// </summary>
        /// <example>
        /// return new NavigationPage(_page);
        /// </example>
        /// <returns>A new page</returns>
        Page CreatePageForNavigation();
    }
}
