using Xunit;
using Prism.Logging;
using Prism.Modularity;
using Prism.Regions;
using StructureMap;
using System.Linq;
using Prism.Events;

namespace Prism.StructureMap.Wpf.Tests
{
    
    public class StructureMapBootstrapperRunMethodFixture
    {
        [Fact]
        public void CanRunBootstrapper()
        {
            var bootstrapper = new DefaultStructureMapBootstrapper();
            bootstrapper.Run();
        }

        [Fact]
        public void RunShouldNotFailIfReturnedNullShell()
        {
            var bootstrapper = new DefaultStructureMapBootstrapper { ShellObject = null };
            bootstrapper.Run();
        }

        [Fact]
        public void RunConfiguresServiceLocatorProvider()
        {
            var bootstrapper = new DefaultStructureMapBootstrapper();
            bootstrapper.Run();

            Assert.True(CommonServiceLocator.ServiceLocator.Current is StructureMapServiceLocatorAdapter);
        }

        [Fact]
        public void RunShouldInitializeContainer()
        {
            var bootstrapper = new DefaultStructureMapBootstrapper();
            var container = bootstrapper.BaseContainer;

            Assert.Null(container);

            bootstrapper.Run();

            container = bootstrapper.BaseContainer;

            Assert.NotNull(container);
            Assert.IsType<Container>(container);
        }

        [Fact]
        public void RunAddsCompositionContainerToContainer()
        {
            var bootstrapper = new DefaultStructureMapBootstrapper();

            var createdContainer = bootstrapper.CallCreateContainer();
            var returnedContainer = createdContainer.GetInstance<IContainer>();
            Assert.NotNull(returnedContainer);
            Assert.Equal(typeof(Container), returnedContainer.GetType());
        }

        [Fact]
        public void RunShouldCallInitializeModules()
        {
            var bootstrapper = new DefaultStructureMapBootstrapper();
            bootstrapper.Run();

            Assert.True(bootstrapper.InitializeModulesCalled);
        }

        [Fact]
        public void RunShouldCallConfigureDefaultRegionBehaviors()
        {
            var bootstrapper = new DefaultStructureMapBootstrapper();

            bootstrapper.Run();

            Assert.True(bootstrapper.ConfigureDefaultRegionBehaviorsCalled);
        }

        [Fact]
        public void RunShouldCallConfigureRegionAdapterMappings()
        {
            var bootstrapper = new DefaultStructureMapBootstrapper();

            bootstrapper.Run();

            Assert.True(bootstrapper.ConfigureRegionAdapterMappingsCalled);
        }

        [Fact]
        public void RunShouldAssignRegionManagerToReturnedShell()
        {
            var bootstrapper = new DefaultStructureMapBootstrapper();

            bootstrapper.Run();

            Assert.NotNull(RegionManager.GetRegionManager(bootstrapper.BaseShell));
        }

        [Fact]
        public void RunShouldCallCreateLogger()
        {
            var bootstrapper = new DefaultStructureMapBootstrapper();

            bootstrapper.Run();

            Assert.True(bootstrapper.CreateLoggerCalled);
        }

        [Fact]
        public void RunShouldCallCreateModuleCatalog()
        {
            var bootstrapper = new DefaultStructureMapBootstrapper();

            bootstrapper.Run();

            Assert.True(bootstrapper.CreateModuleCatalogCalled);
        }

        [Fact]
        public void RunShouldCallConfigureModuleCatalog()
        {
            var bootstrapper = new DefaultStructureMapBootstrapper();

            bootstrapper.Run();

            Assert.True(bootstrapper.ConfigureModuleCatalogCalled);
        }

        [Fact]
        public void RunShouldCallCreateContainer()
        {
            var bootstrapper = new DefaultStructureMapBootstrapper();

            bootstrapper.Run();

            Assert.True(bootstrapper.CreateContainerCalled);
        }

        [Fact]
        public void RunShouldCallCreateShell()
        {
            var bootstrapper = new DefaultStructureMapBootstrapper();

            bootstrapper.Run();

            Assert.True(bootstrapper.CreateShellCalled);
        }

        [Fact]
        public void RunShouldCallConfigureContainer()
        {
            var bootstrapper = new DefaultStructureMapBootstrapper();

            bootstrapper.Run();

            Assert.True(bootstrapper.ConfigureContainerCalled);
        }


        // unable to mock Configure methods
        // so registration is tested through checking the resolved type against interface
        [Fact]
        public void RunRegistersInstanceOfILoggerFacade()
        {
            var bootstrapper = new DefaultStructureMapBootstrapper();

            bootstrapper.Run();

            var logger = bootstrapper.BaseContainer.GetInstance<ILoggerFacade>();
            Assert.NotNull(logger);
            Assert.True(logger.GetType().IsClass);
            Assert.Contains(typeof(ILoggerFacade), logger.GetType().GetInterfaces());
        }

        [Fact]
        public void RunRegistersInstanceOfIModuleCatalog()
        {
            var bootstrapper = new DefaultStructureMapBootstrapper();

            bootstrapper.Run();

            var moduleCatalog = bootstrapper.BaseContainer.GetInstance<IModuleCatalog>();
            Assert.NotNull(moduleCatalog);
            Assert.True(moduleCatalog.GetType().IsClass);
            Assert.Contains(typeof(IModuleCatalog), moduleCatalog.GetType().GetInterfaces());
        }

        [Fact]
        public void RunRegistersTypeForIModuleInitializer()
        {
            var bootstrapper = new DefaultStructureMapBootstrapper();

            bootstrapper.Run();

            var moduleInitializer = bootstrapper.BaseContainer.GetInstance<IModuleInitializer>();
            Assert.NotNull(moduleInitializer);
            Assert.True(moduleInitializer.GetType().IsClass);
            Assert.Contains(typeof(IModuleInitializer), moduleInitializer.GetType().GetInterfaces());
        }

        [Fact]
        public void RunRegistersTypeForIRegionManager()
        {
            var bootstrapper = new DefaultStructureMapBootstrapper();

            bootstrapper.Run();

            var regionManager = bootstrapper.BaseContainer.GetInstance<IRegionManager>();
            Assert.NotNull(regionManager);
            Assert.True(regionManager.GetType().IsClass);
            Assert.Contains(typeof(IRegionManager), regionManager.GetType().GetInterfaces());
        }

        [Fact]
        public void RunRegistersTypeForRegionAdapterMappings()
        {
            var bootstrapper = new DefaultStructureMapBootstrapper();

            bootstrapper.Run();

            var regionAdapterMappings = bootstrapper.BaseContainer.GetInstance<RegionAdapterMappings>();
            Assert.NotNull(regionAdapterMappings);
            Assert.Equal(typeof(RegionAdapterMappings), regionAdapterMappings.GetType());
        }

        [Fact]
        public void RunRegistersTypeForIRegionViewRegistry()
        {
            var bootstrapper = new DefaultStructureMapBootstrapper();

            bootstrapper.Run();

            var regionViewRegistry = bootstrapper.BaseContainer.GetInstance<IRegionViewRegistry>();
            Assert.NotNull(regionViewRegistry);
            Assert.True(regionViewRegistry.GetType().IsClass);
            Assert.Contains(typeof(IRegionViewRegistry), regionViewRegistry.GetType().GetInterfaces());
        }

        [Fact]
        public void RunRegistersTypeForIRegionBehaviorFactory()
        {
            var bootstrapper = new DefaultStructureMapBootstrapper();

            bootstrapper.Run();

            var regionBehaviorFactory = bootstrapper.BaseContainer.GetInstance<IRegionBehaviorFactory>();
            Assert.NotNull(regionBehaviorFactory);
            Assert.True(regionBehaviorFactory.GetType().IsClass);
            Assert.Contains(typeof(IRegionBehaviorFactory), regionBehaviorFactory.GetType().GetInterfaces());
        }

        [Fact]
        public void RunRegistersTypeForIEventAggregator()
        {
            var bootstrapper = new DefaultStructureMapBootstrapper();

            bootstrapper.Run();

            var eventAggregator = bootstrapper.BaseContainer.GetInstance<IEventAggregator>();
            Assert.NotNull(eventAggregator);
            Assert.True(eventAggregator.GetType().IsClass);
            Assert.Contains(typeof(IEventAggregator), eventAggregator.GetType().GetInterfaces());
        }

        [Fact]
        public void RunShouldCallTheMethodsInOrder()
        {
            var bootstrapper = new DefaultStructureMapBootstrapper();
            bootstrapper.Run();

            Assert.Equal("CreateLogger", bootstrapper.MethodCalls[0]);
            Assert.Equal("CreateModuleCatalog", bootstrapper.MethodCalls[1]);
            Assert.Equal("ConfigureModuleCatalog", bootstrapper.MethodCalls[2]);
            Assert.Equal("CreateContainer", bootstrapper.MethodCalls[3]);
            Assert.Equal("ConfigureContainer", bootstrapper.MethodCalls[4]);
            Assert.Equal("ConfigureServiceLocator", bootstrapper.MethodCalls[5]);
            Assert.Equal("ConfigureRegionAdapterMappings", bootstrapper.MethodCalls[6]);
            Assert.Equal("ConfigureDefaultRegionBehaviors", bootstrapper.MethodCalls[7]);
            Assert.Equal("RegisterFrameworkExceptionTypes", bootstrapper.MethodCalls[8]);
            Assert.Equal("CreateShell", bootstrapper.MethodCalls[9]);
            Assert.Equal("InitializeShell", bootstrapper.MethodCalls[10]);
            Assert.Equal("InitializeModules", bootstrapper.MethodCalls[11]);
        }

        [Fact]
        public void RunShouldLogBootstrapperSteps()
        {
            var bootstrapper = new DefaultStructureMapBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.Contains("Logger was created successfully.", messages[0]);
            Assert.Contains("Creating module catalog.", messages[1]);
            Assert.Contains("Configuring module catalog.", messages[2]);
            Assert.Contains("Creating StructureMap container.", messages[3]);
            Assert.Contains("Configuring the StructureMap container.", messages[4]);
            Assert.Contains("Configuring ServiceLocator singleton.", messages[5]);
            Assert.Contains("Configuring the ViewModelLocator to use StructureMap.", messages[6]);
            Assert.Contains("Configuring region adapters.", messages[7]);
            Assert.Contains("Configuring default region behaviors.", messages[8]);
            Assert.Contains("Registering Framework Exception Types.", messages[9]);
            Assert.Contains("Creating the shell.", messages[10]);
            Assert.Contains("Setting the RegionManager.", messages[11]);
            Assert.Contains("Updating Regions.", messages[12]);
            Assert.Contains("Initializing the shell.", messages[13]);
            Assert.Contains("Initializing modules.", messages[14]);
            Assert.Contains("Bootstrapper sequence completed.", messages[15]);
        }

        [Fact]
        public void RunShouldLogLoggerCreationSuccess()
        {
            const string expectedMessageText = "Logger was created successfully.";
            var bootstrapper = new DefaultStructureMapBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.True(messages.Contains(expectedMessageText));
        }
        [Fact]
        public void RunShouldLogAboutModuleCatalogCreation()
        {
            const string expectedMessageText = "Creating module catalog.";
            var bootstrapper = new DefaultStructureMapBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.True(messages.Contains(expectedMessageText));
        }

        [Fact]
        public void RunShouldLogAboutConfiguringModuleCatalog()
        {
            const string expectedMessageText = "Configuring module catalog.";
            var bootstrapper = new DefaultStructureMapBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.True(messages.Contains(expectedMessageText));
        }

        [Fact]
        public void RunShouldLogAboutCreatingTheContainer()
        {
            const string expectedMessageText = "Creating StructureMap container.";
            var bootstrapper = new DefaultStructureMapBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.True(messages.Contains(expectedMessageText));
        }

        [Fact]
        public void RunShouldLogAboutConfiguringContainer()
        {
            const string expectedMessageText = "Configuring the StructureMap container.";
            var bootstrapper = new DefaultStructureMapBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.True(messages.Contains(expectedMessageText));
        }

        [Fact]
        public void RunShouldLogAboutConfiguringRegionAdapters()
        {
            const string expectedMessageText = "Configuring region adapters.";
            var bootstrapper = new DefaultStructureMapBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.True(messages.Contains(expectedMessageText));
        }


        [Fact]
        public void RunShouldLogAboutConfiguringRegionBehaviors()
        {
            const string expectedMessageText = "Configuring default region behaviors.";
            var bootstrapper = new DefaultStructureMapBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.True(messages.Contains(expectedMessageText));
        }

        [Fact]
        public void RunShouldLogAboutRegisteringFrameworkExceptionTypes()
        {
            const string expectedMessageText = "Registering Framework Exception Types.";
            var bootstrapper = new DefaultStructureMapBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.True(messages.Contains(expectedMessageText));
        }

        [Fact]
        public void RunShouldLogAboutCreatingTheShell()
        {
            const string expectedMessageText = "Creating the shell.";
            var bootstrapper = new DefaultStructureMapBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.True(messages.Contains(expectedMessageText));
        }

        [Fact]
        public void RunShouldLogAboutInitializingTheShellIfShellCreated()
        {
            const string expectedMessageText = "Initializing the shell.";
            var bootstrapper = new DefaultStructureMapBootstrapper();

            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.True(messages.Contains(expectedMessageText));
        }

        [Fact]
        public void RunShouldNotLogAboutInitializingTheShellIfShellIsNotCreated()
        {
            const string expectedMessageText = "Initializing shell";
            var bootstrapper = new DefaultStructureMapBootstrapper { ShellObject = null };

            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.False(messages.Contains(expectedMessageText));
        }

        [Fact]
        public void RunShouldLogAboutInitializingModules()
        {
            const string expectedMessageText = "Initializing modules.";
            var bootstrapper = new DefaultStructureMapBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.True(messages.Contains(expectedMessageText));
        }

        [Fact]
        public void RunShouldLogAboutRunCompleting()
        {
            const string expectedMessageText = "Bootstrapper sequence completed.";
            var bootstrapper = new DefaultStructureMapBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.True(messages.Contains(expectedMessageText));
        }
    }
}
