using Prism.Container.Wpf.Mocks;
using Xunit;

namespace Prism.Container.Wpf.Tests.Bootstrapper
{
    [Collection(nameof(ContainerExtension))]
    public class BootstrapperNullModuleManagerFixture
    {
        [Fact]
        public void RunShouldNotCallInitializeModulesWhenModuleManagerNotFound()
        {
            var bootstrapper = new NullModuleManagerBootstrapper();

            bootstrapper.Run();

            Assert.False(bootstrapper.InitializeModulesCalled);
        }
    }
}
