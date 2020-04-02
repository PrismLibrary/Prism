using Prism.DI.Forms.Tests.Mocks;
using Xamarin.Forms;
using Xunit;
using Xunit.Abstractions;

namespace Prism.DI.Forms.Tests.Fixtures
{
    public abstract class FixtureBase
    {
        protected ITestOutputHelper _testOutputHelper { get; }

        public FixtureBase(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            Xamarin.Forms.Mocks.MockForms.Init();
        }

        protected PrismApplicationMock CreateMockApplication(Page view = null)
        {
            var initializer = new XunitPlatformInitializer(_testOutputHelper);
            return view == null ? new PrismApplicationMock(initializer) : new PrismApplicationMock(initializer, view);
        }
    }
}