using Prism.Ioc;
using Prism.Forms;
using Xunit.Abstractions;

namespace Prism.DI.Forms.Tests.Mocks
{
    public class XunitPlatformInitializer : IPlatformInitializer
    {
        ITestOutputHelper _testOutputHelper { get; }

        public XunitPlatformInitializer(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterInstance(_testOutputHelper);
        }
    }
}