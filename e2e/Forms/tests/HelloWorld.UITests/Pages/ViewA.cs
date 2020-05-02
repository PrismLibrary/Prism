using Query = System.Func<Xamarin.UITest.Queries.AppQuery, Xamarin.UITest.Queries.AppQuery>;

namespace HelloWorld.UITests.Pages
{
    public class ViewA : BasePage
    {
        readonly Query navigateToViewBButton;
        readonly Query navigateToItem2XamlButton;
        readonly Query navigateToItem2Button;

        protected override PlatformQuery Trait => new PlatformQuery
        {
            Android = x => x.Marked("TitleLabel"),
            iOS = x => x.Marked("TitleLabel")
        };

        public ViewA()
        {
            navigateToViewBButton = x => x.Marked("NavigateToViewBButton");
            navigateToItem2XamlButton = x => x.Marked("NavigateToItem2XamlButton");
            navigateToItem2Button = x => x.Marked("NavigateToItem2Button");
        }

        public void NavigateToViewB()
        {
            app.WaitForElement(navigateToViewBButton);
            app.Screenshot("Tap Navigate to ViewB Button");
            app.Tap(navigateToViewBButton);
        }

        public void NavigateToItem2Xaml()
        {
            app.WaitForElement(navigateToItem2XamlButton);
            app.Screenshot("Tap Navigate to Item List > Item Detail - ID 2 (XAML) Button");
            app.Tap(navigateToItem2XamlButton);
        }

        public void NavigateToItem2()
        {
            app.WaitForElement(navigateToItem2Button);
            app.Screenshot("Tap Navigate to Item List > Item Detail - ID 2 Button");
            app.Tap(navigateToItem2Button);
        }
    }
}
