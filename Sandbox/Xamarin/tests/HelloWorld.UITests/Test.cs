using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
    }
}
