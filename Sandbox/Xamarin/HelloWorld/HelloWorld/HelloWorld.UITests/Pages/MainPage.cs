using System;
using Xamarin.UITest;

using Query = System.Func<Xamarin.UITest.Queries.AppQuery, Xamarin.UITest.Queries.AppQuery>;

namespace HelloWorld.UITests.Pages
{
    public class MainPage : BasePage
    {
        public MainPage(IApp app, Platform platform)
            : base(app, platform, "MainPage")
        {
        }
    }
}
