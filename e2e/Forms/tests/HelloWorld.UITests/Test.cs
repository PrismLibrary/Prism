using System;
using System.Threading;
using HelloWorld.UITests.Pages;
using NUnit.Framework;
using Xamarin.UITest;

namespace HelloWorld.UITests
{
    public class Test : BaseTestFixture
    {
        public Test(Platform platform)
            : base(platform)
        {
        }

#if DEBUG
        [Test]
        public void Repl()
        {
            if (TestEnvironment.IsTestCloud)
                Assert.Ignore("Local only");

            app.Repl();
        }
#endif

        [Test]
        public void AppLaunches()
        {
            Thread.Sleep(TimeSpan.FromSeconds(3));
            app.Screenshot("App Launched");
        }

        [Test]
        public void NavigateToViewB()
        {
            new ViewA().NavigateToViewB();

            new ViewB();
        }

        [Test]
        public void NavigateToItemList_ItemDetail_Id2()
        {
            new ViewA().NavigateToItem2();

            var itemDetailPage = new ItemDetailPage();
            var result = itemDetailPage.GetItemIdLabelValue();
            Assert.AreEqual(2, result);
        }

        [Test]
        public void NavigateToItemList_ItemDetail_Id2_Xaml()
        {
            new ViewA().NavigateToItem2();

            var itemDetailPage = new ItemDetailPage();
            var result = itemDetailPage.GetItemIdLabelValue();
            Assert.AreEqual(2, result);
        }
    }
}
