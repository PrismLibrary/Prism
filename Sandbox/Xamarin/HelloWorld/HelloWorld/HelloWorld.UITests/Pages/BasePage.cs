using Xamarin.UITest;

namespace HelloWorld.UITests
{
    public abstract class BasePage
    {
        readonly string pageTitle;

        protected readonly IApp app;
        protected readonly bool OnAndroid;
        protected readonly bool OniOS;

        protected BasePage(IApp app, Platform platform, string pageTitle)
        {
            this.app = app;

            OnAndroid = platform == Platform.Android;
            OniOS = platform == Platform.iOS;

            this.pageTitle = pageTitle;

        }
        public bool IsPageVisible => app.Query(pageTitle).Length > 0;

        public void WaitForPageToLoad()
        {
            app.WaitForElement(pageTitle);
        }
    }
}