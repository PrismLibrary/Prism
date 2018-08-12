namespace Prism.Navigation
{
    public static class KnownNavigationParameters
    {
        /// <summary>
        /// Used to dynamically create a Page that will be used as a Tab when navigating to a TabbedPage.
        /// </summary>
        public const string CreateTab = "createTab";

        /// <summary>
        /// Used to select an existing Tab when navigating to a Tabbedpage.
        /// </summary>
        public const string SelectedTab = "selectedTab";

        /// <summary>
        /// Used to control the navigation stack. If <c>true</c> uses PopModalAsync, if <c>false</c> uses PopAsync.
        /// </summary>
        public const string UseModalNavigation = "useModalNavigation";

        /// <summary>
        /// Used to define a navigation parameter that is bound directly to a CommandParameter via <code>{Binding .}</code>.
        /// </summary>
        public const string XamlParam = "xamlParam";
    }
}
