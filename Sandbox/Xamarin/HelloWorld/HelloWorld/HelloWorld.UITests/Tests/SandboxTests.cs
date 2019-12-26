using NUnit.Framework;
using Xamarin.UITest;

namespace HelloWorld.UITests
{
    public class MockDataTests : BaseTest
    {
        public MockDataTests(Platform platform)
            : base(platform)
        {
        }

        public override void BeforeEachTest()
        {
            base.BeforeEachTest();
        }

        [Test]
        public void Repl()
        {
            var result = app.Query();
            app.Screenshot("Repl");
            app.Repl();
        }
    }
}
