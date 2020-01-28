using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Query = System.Func<Xamarin.UITest.Queries.AppQuery, Xamarin.UITest.Queries.AppQuery>;

namespace HelloWorld.UITests.Pages
{
    public class ViewA : BasePage
    {
        readonly Query navigateToViewBButton;

        protected override PlatformQuery Trait => new PlatformQuery
        {
            Android = x => x.Marked("TitleLabel"),
            iOS = x => x.Marked("TitleLabel")
        };

        public ViewA()
        {
            navigateToViewBButton = x => x.Marked("NavigateToViewBButton");
        }

        public void NavigateToViewB()
        {
            app.WaitForElement(navigateToViewBButton);
            app.Screenshot("Tap Navigate to ViewB Button");
            app.Tap(navigateToViewBButton);
        }
    }
}
