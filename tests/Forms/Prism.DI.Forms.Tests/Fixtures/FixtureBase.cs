using System;
using Prism.DI.Forms.Tests.Mocks;
using Prism.Ioc;
using Xamarin.Forms;
using Xunit.Abstractions;

namespace Prism.DI.Forms.Tests.Fixtures
{
    public abstract class FixtureBase : IDisposable
    {
        protected ITestOutputHelper _testOutputHelper { get; }

        protected FixtureBase(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            ContainerLocator.ResetContainer();
            Xamarin.Forms.Mocks.MockForms.Init();
        }

        protected PrismApplicationMock CreateMockApplication(Func<Page> viewFactory = null)
        {
            var initializer = new XunitPlatformInitializer(_testOutputHelper);
            return viewFactory == null ? new PrismApplicationMock(initializer) : new PrismApplicationMock(initializer, viewFactory);
        }

        public void Dispose()
        {
            ContainerLocator.ResetContainer();
        }
    }
}
