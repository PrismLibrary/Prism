using HelloWorld.UITests.Pages;
using NUnit.Framework;
using Xamarin.UITest;

namespace HelloWorld.UITests
{
    [TestFixture(Platform.Android)]
    [TestFixture(Platform.iOS)]

    public abstract class BaseTest
    {
        protected IApp app;
        protected Platform platform;

        protected MainPage MainPage;
        protected ModulesPage ModulesPage;

        protected BaseTest(Platform platform)
        {
            this.platform = platform;
        }

        [SetUp]
        virtual public void BeforeEachTest()
        {
            app = AppInitializer.StartApp(platform);
            app.Screenshot("App Initialized");

            MainPage = new MainPage(app, platform);
            ModulesPage = new ModulesPage(app, platform);
        }
    }
}