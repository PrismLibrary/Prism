using Moq;
using Prism.Modularity;
using Prism.Wpf.Tests.Mocks.Modules;
using Xunit;

namespace Prism.Wpf.Tests.Modularity
{
    public class ModuleManagerExtensionsFixture
    {
        [Fact]
        public void ModuleManagerExposesIModuleCatalogModules()
        {
            var modules = new[]
            {
                new ModuleInfo(typeof(MockModuleA))
            };
            var moduleCatalogMock = new Mock<IModuleCatalog>();
            moduleCatalogMock.Setup(c => c.Modules).Returns(modules);
            IModuleManager manager = new ModuleManager(Mock.Of<IModuleInitializer>(), moduleCatalogMock.Object);

            Assert.Same(modules, manager.Modules);
        }

        [Fact]
        public void ModuleManagerReturnsCorrectModuleStateWithGeneric()
        {
            var moduleInfo = new ModuleInfo(typeof(MockModuleA));
            var moduleCatalogMock = new Mock<IModuleCatalog>();
            moduleCatalogMock.Setup(c => c.Modules).Returns(new[] { moduleInfo });
            IModuleManager manager = new ModuleManager(Mock.Of<IModuleInitializer>(), moduleCatalogMock.Object);

            Assert.Equal(moduleInfo.State, manager.GetModuleState<MockModuleA>());
            moduleInfo.State = ModuleState.LoadingTypes;
            Assert.Equal(moduleInfo.State, manager.GetModuleState<MockModuleA>());
        }

        [Fact]
        public void ModuleManagerReturnsCorrectModuleStateWithName()
        {
            var moduleInfo = new ModuleInfo(typeof(MockModuleA));
            var moduleCatalogMock = new Mock<IModuleCatalog>();
            moduleCatalogMock.Setup(c => c.Modules).Returns(new[] { moduleInfo });
            IModuleManager manager = new ModuleManager(Mock.Of<IModuleInitializer>(), moduleCatalogMock.Object);

            Assert.Equal(moduleInfo.State, manager.GetModuleState(nameof(MockModuleA)));
            moduleInfo.State = ModuleState.LoadingTypes;
            Assert.Equal(moduleInfo.State, manager.GetModuleState(nameof(MockModuleA)));
        }

        [Fact]
        public void ModuleManagerReturnsCorrectInitializationStateWithGeneric()
        {
            var moduleInfo = new ModuleInfo(typeof(MockModuleA));
            var moduleCatalogMock = new Mock<IModuleCatalog>();
            moduleCatalogMock.Setup(c => c.Modules).Returns(new[] { moduleInfo });
            IModuleManager manager = new ModuleManager(Mock.Of<IModuleInitializer>(), moduleCatalogMock.Object);

            Assert.False(manager.IsModuleInitialized<MockModuleA>());
            moduleInfo.State = ModuleState.Initializing;
            Assert.False(manager.IsModuleInitialized<MockModuleA>());
            moduleInfo.State = ModuleState.Initialized;
            Assert.True(manager.IsModuleInitialized<MockModuleA>());
        }

        [Fact]
        public void ModuleManagerReturnsCorrectInitializationStateWithName()
        {
            var moduleInfo = new ModuleInfo(typeof(MockModuleA));
            var moduleCatalogMock = new Mock<IModuleCatalog>();
            moduleCatalogMock.Setup(c => c.Modules).Returns(new[] { moduleInfo });
            IModuleManager manager = new ModuleManager(Mock.Of<IModuleInitializer>(), moduleCatalogMock.Object);

            Assert.False(manager.IsModuleInitialized(nameof(MockModuleA)));
            moduleInfo.State = ModuleState.Initializing;
            Assert.False(manager.IsModuleInitialized(nameof(MockModuleA)));
            moduleInfo.State = ModuleState.Initialized;
            Assert.True(manager.IsModuleInitialized(nameof(MockModuleA)));
        }

        [Fact]
        public void ModuleManagerLoadModuleGeneric_CallsLoadModuleWithName()
        {
            var managerMock = new Mock<IModuleManager>();
            managerMock.Object.LoadModule<MockModuleA>();
            managerMock.Verify(m => m.LoadModule(nameof(MockModuleA)));
        }
    }
}
