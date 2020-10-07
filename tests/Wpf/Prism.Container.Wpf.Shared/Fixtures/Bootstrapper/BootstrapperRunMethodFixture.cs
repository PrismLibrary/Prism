using Moq;
using Prism.Container.Wpf.Mocks;
using Prism.Events;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using Xunit;

namespace Prism.Container.Wpf.Tests.Bootstrapper
{
    [Collection(nameof(ContainerExtension))]
    public partial class BootstrapperRunMethodFixture
    {
        [StaFact]
        public void CanRunBootstrapper()
        {
            var bootstrapper = new MockBootstrapper();
            bootstrapper.Run();
        }

        [StaFact]
        public void RunShouldNotFailIfReturnedNullShell()
        {
            var bootstrapper = new MockBootstrapper { ShellObject = null };
            bootstrapper.Run();
        }

        [StaFact]
        public void RunSetsCurrentContainer()
        {
            var bootstrapper = new MockBootstrapper();
            bootstrapper.Run();

            Assert.NotNull(ContainerLocator.Container);
            Assert.IsType(ContainerHelper.ContainerExtensionType, ContainerLocator.Container);
        }

        [StaFact]
        public void RunShouldInitializeContainer()
        {
            var bootstrapper = new MockBootstrapper();
            var container = bootstrapper.BaseContainer;

            Assert.Null(container);

            bootstrapper.Run();

            container = bootstrapper.BaseContainer;

            Assert.NotNull(container);
            Assert.IsType(ContainerHelper.BaseContainerType, container);
        }

        [StaFact]
        public void RunShouldCallInitializeModules()
        {
            var bootstrapper = new MockBootstrapper();
            bootstrapper.Run();

            Assert.True(bootstrapper.InitializeModulesCalled);
        }

        [StaFact]
        public void RunShouldCallConfigureDefaultRegionBehaviors()
        {
            var bootstrapper = new MockBootstrapper();

            bootstrapper.Run();

            Assert.True(bootstrapper.ConfigureDefaultRegionBehaviorsCalled);
        }

        [StaFact]
        public void RunShouldCallConfigureRegionAdapterMappings()
        {
            var bootstrapper = new MockBootstrapper();

            bootstrapper.Run();

            Assert.True(bootstrapper.ConfigureRegionAdapterMappingsCalled);
        }

        [StaFact]
        public void RunShouldAssignRegionManagerToReturnedShell()
        {
            var bootstrapper = new MockBootstrapper();

            bootstrapper.Run();

            Assert.NotNull(RegionManager.GetRegionManager(bootstrapper.BaseShell));
        }

        [StaFact]
        public void RunShouldCallCreateModuleCatalog()
        {
            var bootstrapper = new MockBootstrapper();

            bootstrapper.Run();

            Assert.True(bootstrapper.CreateModuleCatalogCalled);
        }

        [StaFact]
        public void RunShouldCallConfigureModuleCatalog()
        {
            var bootstrapper = new MockBootstrapper();

            bootstrapper.Run();

            Assert.True(bootstrapper.ConfigureModuleCatalogCalled);
        }

        [StaFact]
        public void RunShouldCallCreateContainer()
        {
            var bootstrapper = new MockBootstrapper();

            bootstrapper.Run();

            Assert.True(bootstrapper.CreateContainerCalled);
        }

        [StaFact]
        public void RunShouldCallCreateShell()
        {
            var bootstrapper = new MockBootstrapper();

            bootstrapper.Run();

            Assert.True(bootstrapper.CreateShellCalled);
        }

        [StaFact]
        public void RunShouldCallRegisterRequiredTypes()
        {
            var bootstrapper = new MockBootstrapper();

            bootstrapper.Run();

            Assert.True(bootstrapper.RegisterRequiredTypesCalled);
        }

        [StaFact]
        public void RunShouldCallRegisterTypes()
        {
            var bootstrapper = new MockBootstrapper();

            bootstrapper.Run();

            Assert.True(bootstrapper.RegisterTypesCalled);
        }

        [StaFact]
        public void SetsContainerLocatorCurrentContainer()
        {
            ContainerLocator.ResetContainer();
            Assert.Null(ContainerLocator.Container);
            var bootstrapper = new MockBootstrapper();

            bootstrapper.Run();

            Assert.NotNull(ContainerLocator.Container);
            Assert.Same(bootstrapper.Container, ContainerLocator.Container);
        }

        [StaFact]
        public void RunShouldCallConfigureViewModelLocator()
        {
            var bootstrapper = new MockBootstrapper();

            bootstrapper.Run();

            Assert.True(bootstrapper.ConfigureViewModelLocatorCalled);
        }

        [StaFact]
        public void ModuleManagerRunCalled()
        {
            // Have to use a non-mocked container because of IsRegistered<> extension method, Registrations property,and ContainerRegistration
            var mockedModuleInitializer = new Mock<IModuleInitializer>();
            var mockedModuleManager = new Mock<IModuleManager>();
            var regionAdapterMappings = new RegionAdapterMappings();
            var container = ContainerHelper.CreateContainerExtension();
            var regionBehaviorFactory = new RegionBehaviorFactory(container);

            container.RegisterInstance<IContainerExtension>(container);
            container.RegisterInstance<IModuleCatalog>(new ModuleCatalog());
            container.RegisterInstance<IModuleInitializer>(mockedModuleInitializer.Object);
            container.RegisterInstance<IModuleManager>(mockedModuleManager.Object);
            container.RegisterInstance<RegionAdapterMappings>(regionAdapterMappings);

            container.RegisterSingleton(typeof(RegionAdapterMappings), typeof(RegionAdapterMappings));
            container.RegisterSingleton(typeof(IRegionManager), typeof(RegionManager));
            container.RegisterSingleton(typeof(IEventAggregator), typeof(EventAggregator));
            container.RegisterSingleton(typeof(IRegionViewRegistry), typeof(RegionViewRegistry));
            container.RegisterSingleton(typeof(IRegionBehaviorFactory), typeof(RegionBehaviorFactory));
            container.RegisterSingleton(typeof(IRegionNavigationJournalEntry), typeof(RegionNavigationJournalEntry));
            container.RegisterSingleton(typeof(IRegionNavigationJournal), typeof(RegionNavigationJournal));
            container.RegisterSingleton(typeof(IRegionNavigationService), typeof(RegionNavigationService));
            container.RegisterSingleton(typeof(IRegionNavigationContentLoader), typeof(RegionNavigationContentLoader));

            container.RegisterInstance<SelectorRegionAdapter>(new SelectorRegionAdapter(regionBehaviorFactory));
            container.RegisterInstance<ItemsControlRegionAdapter>(new ItemsControlRegionAdapter(regionBehaviorFactory));
            container.RegisterInstance<ContentControlRegionAdapter>(new ContentControlRegionAdapter(regionBehaviorFactory));

            var bootstrapper = new MockedContainerBootstrapper(container.GetBaseContainer());
            bootstrapper.Run(false);

            mockedModuleManager.Verify(mm => mm.Run(), Times.Once());
        }

        [StaFact]
        public void RunShouldCallTheMethodsInOrder()
        {
            var bootstrapper = new MockBootstrapper();
            bootstrapper.Run();

            var index = 0;
            Assert.Equal("ConfigureViewModelLocator", bootstrapper.MethodCalls[index++]);
            Assert.Equal("CreateContainerExtension", bootstrapper.MethodCalls[index++]);
            Assert.Equal("CreateModuleCatalog", bootstrapper.MethodCalls[index++]);
            Assert.Equal("RegisterRequiredTypes", bootstrapper.MethodCalls[index++]);
            Assert.Equal("RegisterTypes", bootstrapper.MethodCalls[index++]);
            Assert.Equal("ConfigureModuleCatalog", bootstrapper.MethodCalls[index++]);
            Assert.Equal("ConfigureRegionAdapterMappings", bootstrapper.MethodCalls[index++]);
            Assert.Equal("ConfigureDefaultRegionBehaviors", bootstrapper.MethodCalls[index++]);
            Assert.Equal("RegisterFrameworkExceptionTypes", bootstrapper.MethodCalls[index++]);
            Assert.Equal("CreateShell", bootstrapper.MethodCalls[index++]);
            Assert.Equal("InitializeShell", bootstrapper.MethodCalls[index++]);
            Assert.Equal("InitializeModules", bootstrapper.MethodCalls[index++]);
            Assert.Equal("OnInitialized", bootstrapper.MethodCalls[index++]);
        }
    }
}
