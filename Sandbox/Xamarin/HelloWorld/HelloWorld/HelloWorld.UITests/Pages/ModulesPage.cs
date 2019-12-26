using Xamarin.UITest;

using Query = System.Func<Xamarin.UITest.Queries.AppQuery, Xamarin.UITest.Queries.AppQuery>;

namespace HelloWorld.UITests.Pages
{
    public class ModulesPage : BasePage
    {
        public ModulesPage(IApp app, Platform platform)
            : base(app, platform, "Modules")
        {
        }
    }
}
