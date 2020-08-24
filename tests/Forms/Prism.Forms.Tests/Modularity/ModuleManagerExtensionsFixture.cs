using Moq;
using Prism.Forms.Tests.Mocks.Modules;
using Prism.Modularity;
using Xunit;

namespace Prism.Forms.Tests.Modularity
{
    public class ModuleManagerExtensionsFixture
    {
        [Fact]
        public void ModuleManagerExposesIModuleCatalogModules()
        {
            var modules = new[]
            {
                new ModuleInfo(typeof(ModuleA))
            };
            var moduleCatalogMock = new Mock<IModuleCatalog>();
            moduleCatalogMock.Setup(c => c.Modules).Returns(modules);
            IModuleManager manager = new ModuleManager(Mock.Of<IModuleInitializer>(), moduleCatalogMock.Object);

            Assert.Same(modules, manager.Modules);
        }

        [Fact]
        public void ModuleManagerReturnsCorrectModuleStateWithGeneric()
        {
            IModuleInfo moduleInfo = new ModuleInfo(typeof(ModuleA));
            var moduleCatalogMock = new Mock<IModuleCatalog>();
            moduleCatalogMock.Setup(c => c.Modules).Returns(new[] { moduleInfo });
            IModuleManager manager = new ModuleManager(Mock.Of<IModuleInitializer>(), moduleCatalogMock.Object);

            Assert.Equal(moduleInfo.State, manager.GetModuleState<ModuleA>());
            moduleInfo.State = ModuleState.LoadingTypes;
            Assert.Equal(moduleInfo.State, manager.GetModuleState<ModuleA>());
        }

        [Fact]
        public void ModuleManagerReturnsCorrectModuleStateWithName()
        {
            IModuleInfo moduleInfo = new ModuleInfo(typeof(ModuleA));
            var moduleCatalogMock = new Mock<IModuleCatalog>();
            moduleCatalogMock.Setup(c => c.Modules).Returns(new[] { moduleInfo });
            IModuleManager manager = new ModuleManager(Mock.Of<IModuleInitializer>(), moduleCatalogMock.Object);

            Assert.Equal(moduleInfo.State, manager.GetModuleState(nameof(ModuleA)));
            moduleInfo.State = ModuleState.LoadingTypes;
            Assert.Equal(moduleInfo.State, manager.GetModuleState(nameof(ModuleA)));
        }

        [Fact]
        public void ModuleManagerReturnsCorrectInitializationStateWithGeneric()
        {
            IModuleInfo moduleInfo = new ModuleInfo(typeof(ModuleA));
            var moduleCatalogMock = new Mock<IModuleCatalog>();
            moduleCatalogMock.Setup(c => c.Modules).Returns(new[] { moduleInfo });
            IModuleManager manager = new ModuleManager(Mock.Of<IModuleInitializer>(), moduleCatalogMock.Object);

            Assert.False(manager.IsModuleInitialized<ModuleA>());
            moduleInfo.State = ModuleState.Initializing;
            Assert.False(manager.IsModuleInitialized<ModuleA>());
            moduleInfo.State = ModuleState.Initialized;
            Assert.True(manager.IsModuleInitialized<ModuleA>());
        }

        [Fact]
        public void ModuleManagerReturnsCorrectInitializationStateWithName()
        {
            IModuleInfo moduleInfo = new ModuleInfo(typeof(ModuleA));
            var moduleCatalogMock = new Mock<IModuleCatalog>();
            moduleCatalogMock.Setup(c => c.Modules).Returns(new[] { moduleInfo });
            IModuleManager manager = new ModuleManager(Mock.Of<IModuleInitializer>(), moduleCatalogMock.Object);

            Assert.False(manager.IsModuleInitialized(nameof(ModuleA)));
            moduleInfo.State = ModuleState.Initializing;
            Assert.False(manager.IsModuleInitialized(nameof(ModuleA)));
            moduleInfo.State = ModuleState.Initialized;
            Assert.True(manager.IsModuleInitialized(nameof(ModuleA)));
        }

        [Fact]
        public void ModuleManagerLoadModuleGeneric_CallsLoadModuleWithName()
        {
            var managerMock = new Mock<IModuleManager>();
            managerMock.Object.LoadModule<ModuleA>();
            managerMock.Verify(m => m.LoadModule(nameof(ModuleA)));
        }
    }
}
