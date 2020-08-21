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
        public void RunShouldCallConfigureContainer()
        {
            var bootstrapper = new MockBootstrapper();

            bootstrapper.Run();

            Assert.True(bootstrapper.ConfigureContainerCalled);
        }

        [StaFact]
        public void SetsContainerLocatorCurrentContainer()
        {
            ContainerLocator.ResetContainer();
            Assert.Null(ContainerLocator.Container);
            var bootstrapper = new MockBootstrapper();

            bootstrapper.Run();

            Assert.NotNull(ContainerLocator.Container);
            Assert.Same(bootstrapper.Container, ContainerLocator.Container.GetBaseContainer());
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
            Assert.Equal("CreateModuleCatalog", bootstrapper.MethodCalls[index++]);
            Assert.Equal("ConfigureModuleCatalog", bootstrapper.MethodCalls[index++]);
            Assert.Equal("CreateContainer", bootstrapper.MethodCalls[index++]);
            Assert.Equal("ConfigureContainer", bootstrapper.MethodCalls[index++]);
            Assert.Equal("ConfigureViewModelLocator", bootstrapper.MethodCalls[index++]);
            Assert.Equal("ConfigureRegionAdapterMappings", bootstrapper.MethodCalls[index++]);
            Assert.Equal("ConfigureDefaultRegionBehaviors", bootstrapper.MethodCalls[index++]);
            Assert.Equal("RegisterFrameworkExceptionTypes", bootstrapper.MethodCalls[index++]);
            Assert.Equal("CreateShell", bootstrapper.MethodCalls[index++]);
            Assert.Equal("InitializeShell", bootstrapper.MethodCalls[index++]);
            Assert.Equal("InitializeModules", bootstrapper.MethodCalls[index++]);
        }

        [StaFact]
        public void RunShouldLogBootstrapperSteps()
        {
            var bootstrapper = new MockBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.Messages;

            var index = 0;
            Assert.Contains(ContainerResources.LoggerCreatedSuccessfully, messages[index++]);
            Assert.Contains(ContainerResources.CreatingModuleCatalog, messages[index++]);
            Assert.Contains(ContainerResources.ConfiguringModuleCatalog, messages[index++]);
            Assert.Contains(ContainerResources.CreatingContainer, messages[index++]);
            Assert.Contains(ContainerResources.ConfiguringContainer, messages[index++]);
            Assert.Contains(ContainerResources.ConfiguringViewModelLocator, messages[index++]);
            Assert.Contains(ContainerResources.ConfiguringRegionAdapters, messages[index++]);
            Assert.Contains(ContainerResources.ConfiguringDefaultRegionBehaviors, messages[index++]);
            Assert.Contains(ContainerResources.RegisteringFrameworkExceptionTypes, messages[index++]);
            Assert.Contains(ContainerResources.CreatingShell, messages[index++]);
            Assert.Contains(ContainerResources.SettingTheRegionManager, messages[index++]);
            Assert.Contains(ContainerResources.UpdatingRegions, messages[index++]);
            Assert.Contains(ContainerResources.InitializingShell, messages[index++]);
            Assert.Contains(ContainerResources.InitializingModules, messages[index++]);
            Assert.Contains(ContainerResources.BootstrapperSequenceCompleted, messages[index++]);
        }

        [StaFact]
        public void RunShouldLogLoggerCreationSuccess()
        {
            var bootstrapper = new MockBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.Messages;

            Assert.Contains(ContainerResources.LoggerCreatedSuccessfully, messages);
        }
        [StaFact]
        public void RunShouldLogAboutModuleCatalogCreation()
        {
            var bootstrapper = new MockBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.Messages;
            Assert.Contains(ContainerResources.CreatingModuleCatalog, messages);
        }

        [StaFact]
        public void RunShouldLogAboutConfiguringModuleCatalog()
        {
            var bootstrapper = new MockBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.Messages;

            Assert.Contains(ContainerResources.ConfiguringModuleCatalog, messages);
        }

        [StaFact]
        public void RunShouldLogAboutCreatingTheContainer()
        {
            var bootstrapper = new MockBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.Messages;

            Assert.Contains(ContainerResources.CreatingContainer, messages);
        }

        [StaFact]
        public void RunShouldLogAboutConfiguringContainer()
        {
            var bootstrapper = new MockBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.Messages;

            Assert.Contains(ContainerResources.ConfiguringContainer, messages);
        }

        [StaFact]
        public void RunShouldLogAboutConfiguringViewModelLocator()
        {
            var bootstrapper = new MockBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.Messages;

            Assert.Contains(ContainerResources.ConfiguringViewModelLocator, messages);
        }

        [StaFact]
        public void RunShouldLogAboutConfiguringRegionAdapters()
        {
            var bootstrapper = new MockBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.Messages;

            Assert.Contains(ContainerResources.ConfiguringRegionAdapters, messages);
        }

        [StaFact]
        public void RunShouldLogAboutConfiguringRegionBehaviors()
        {
            var bootstrapper = new MockBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.Messages;

            Assert.Contains(ContainerResources.ConfiguringDefaultRegionBehaviors, messages);
        }

        [StaFact]
        public void RunShouldLogAboutRegisteringFrameworkExceptionTypes()
        {
            var bootstrapper = new MockBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.Messages;

            Assert.Contains(ContainerResources.RegisteringFrameworkExceptionTypes, messages);
        }

        [StaFact]
        public void RunShouldLogAboutCreatingTheShell()
        {
            var bootstrapper = new MockBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.Messages;

            Assert.Contains(ContainerResources.CreatingShell, messages);
        }

        [StaFact]
        public void RunShouldLogAboutInitializingTheShellIfShellCreated()
        {
            var bootstrapper = new MockBootstrapper();

            bootstrapper.Run();
            var messages = bootstrapper.Messages;

            Assert.Contains(ContainerResources.InitializingShell, messages);
        }

        [StaFact]
        public void RunShouldNotLogAboutInitializingTheShellIfShellIsNotCreated()
        {
            var bootstrapper = new MockBootstrapper { ShellObject = null };

            bootstrapper.Run();
            var messages = bootstrapper.Messages;

            Assert.DoesNotContain(ContainerResources.InitializingShell, messages);
        }

        [StaFact]
        public void RunShouldLogAboutInitializingModules()
        {
            var bootstrapper = new MockBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.Messages;

            Assert.Contains(ContainerResources.InitializingModules, messages);
        }

        [StaFact]
        public void RunShouldLogAboutRunCompleting()
        {
            var bootstrapper = new MockBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.Messages;

            Assert.Contains(ContainerResources.BootstrapperSequenceCompleted, messages);
        }
    }
}
