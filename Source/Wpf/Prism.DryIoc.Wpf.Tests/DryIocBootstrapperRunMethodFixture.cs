using System.Linq;
using DryIoc;
using CommonServiceLocator;
using Xunit;
using Prism.Events;
using Prism.Logging;
using Prism.Modularity;
using Prism.Regions;

namespace Prism.DryIoc.Wpf.Tests
{
    [Collection("ServiceLocator")]
    public class DryIocBootstrapperRunMethodFixture
    {
        [StaFact]
        public void CanRunBootstrapper()
        {
            var bootstrapper = new DefaultDryIocBootstrapper();
            bootstrapper.Run();
        }

        [StaFact]
        public void RunShouldNotFailIfReturnedNullShell()
        {
            var bootstrapper = new DefaultDryIocBootstrapper { ShellObject = null };
            bootstrapper.Run();
        }

        [StaFact]
        public void RunConfiguresServiceLocatorProvider()
        {
            var bootstrapper = new DefaultDryIocBootstrapper();
            bootstrapper.Run();

            Assert.True(ServiceLocator.Current is DryIocServiceLocatorAdapter);
        }

        [StaFact]
        public void RunShouldInitializeContainer()
        {
            var bootstrapper = new DefaultDryIocBootstrapper();
            var container = bootstrapper.BaseContainer;

            Assert.Null(container);

            bootstrapper.Run();

            container = bootstrapper.BaseContainer;

            Assert.NotNull(container);
            Assert.IsAssignableFrom<IContainer>(container);
        }

        [StaFact]
        public void RunAddsCompositionContainerToContainer()
        {
            var bootstrapper = new DefaultDryIocBootstrapper();

            var createdContainer = bootstrapper.CallCreateContainer();
            var returnedContainer = createdContainer.Resolve<IContainer>();
            Assert.NotNull(returnedContainer);
            Assert.Contains(typeof(IContainer), returnedContainer.GetType().GetInterfaces());
        }

        [StaFact]
        public void RunShouldCallInitializeModules()
        {
            var bootstrapper = new DefaultDryIocBootstrapper();
            bootstrapper.Run();

            Assert.True(bootstrapper.InitializeModulesCalled);
        }

        [StaFact]
        public void RunShouldCallConfigureDefaultRegionBehaviors()
        {
            var bootstrapper = new DefaultDryIocBootstrapper();

            bootstrapper.Run();

            Assert.True(bootstrapper.ConfigureDefaultRegionBehaviorsCalled);
        }

        [StaFact]
        public void RunShouldCallConfigureRegionAdapterMappings()
        {
            var bootstrapper = new DefaultDryIocBootstrapper();

            bootstrapper.Run();

            Assert.True(bootstrapper.ConfigureRegionAdapterMappingsCalled);
        }

        [StaFact]
        public void RunShouldAssignRegionManagerToReturnedShell()
        {
            var bootstrapper = new DefaultDryIocBootstrapper();

            bootstrapper.Run();

            Assert.NotNull(RegionManager.GetRegionManager(bootstrapper.BaseShell));
        }

        [StaFact]
        public void RunShouldCallCreateLogger()
        {
            var bootstrapper = new DefaultDryIocBootstrapper();

            bootstrapper.Run();

            Assert.True(bootstrapper.CreateLoggerCalled);
        }

        [StaFact]
        public void RunShouldCallCreateModuleCatalog()
        {
            var bootstrapper = new DefaultDryIocBootstrapper();

            bootstrapper.Run();

            Assert.True(bootstrapper.CreateModuleCatalogCalled);
        }

        [StaFact]
        public void RunShouldCallConfigureModuleCatalog()
        {
            var bootstrapper = new DefaultDryIocBootstrapper();

            bootstrapper.Run();

            Assert.True(bootstrapper.ConfigureModuleCatalogCalled);
        }

        [StaFact]
        public void RunShouldCallCreateContainer()
        {
            var bootstrapper = new DefaultDryIocBootstrapper();

            bootstrapper.Run();

            Assert.True(bootstrapper.CreateContainerCalled);
        }

        [StaFact]
        public void RunShouldCallCreateShell()
        {
            var bootstrapper = new DefaultDryIocBootstrapper();

            bootstrapper.Run();

            Assert.True(bootstrapper.CreateShellCalled);
        }

        [StaFact]
        public void RunShouldCallConfigureContainer()
        {
            var bootstrapper = new DefaultDryIocBootstrapper();

            bootstrapper.Run();

            Assert.True(bootstrapper.ConfigureContainerCalled);
        }

        // unable to mock extension RegisterInstance/RegisterType methods
        // so registration is tested through checking the resolved type against interface
        [StaFact]
        public void RunRegistersInstanceOfILoggerFacade()
        {
            var bootstrapper = new DefaultDryIocBootstrapper();

            bootstrapper.Run();

            var logger = bootstrapper.BaseContainer.Resolve<ILoggerFacade>();
            Assert.NotNull(logger);
            Assert.True(logger.GetType().IsClass);
            Assert.Contains(typeof(ILoggerFacade), logger.GetType().GetInterfaces());
        }

        [StaFact]
        public void RunRegistersInstanceOfIModuleCatalog()
        {
            var bootstrapper = new DefaultDryIocBootstrapper();

            bootstrapper.Run();

            var moduleCatalog = bootstrapper.BaseContainer.Resolve<IModuleCatalog>();
            Assert.NotNull(moduleCatalog);
            Assert.True(moduleCatalog.GetType().IsClass);
            Assert.Contains(typeof(IModuleCatalog), moduleCatalog.GetType().GetInterfaces());
        }

        [StaFact]
        public void RunRegistersTypeForIServiceLocator()
        {
            var bootstrapper = new DefaultDryIocBootstrapper();

            bootstrapper.Run();

            var serviceLocator = bootstrapper.BaseContainer.Resolve<IServiceLocator>();
            Assert.NotNull(serviceLocator);
            Assert.True(serviceLocator.GetType().IsClass);
            Assert.Equal(typeof(DryIocServiceLocatorAdapter), serviceLocator.GetType());
            Assert.Contains(typeof(IServiceLocator), serviceLocator.GetType().GetInterfaces());
        }

        [StaFact]
        public void RunRegistersTypeForIModuleInitializer()
        {
            var bootstrapper = new DefaultDryIocBootstrapper();

            bootstrapper.Run();

            var moduleInitializer = bootstrapper.BaseContainer.Resolve<IModuleInitializer>();
            Assert.NotNull(moduleInitializer);
            Assert.True(moduleInitializer.GetType().IsClass);
            Assert.Contains(typeof(IModuleInitializer), moduleInitializer.GetType().GetInterfaces());
        }

        [StaFact]
        public void RunRegistersTypeForIRegionManager()
        {
            var bootstrapper = new DefaultDryIocBootstrapper();

            bootstrapper.Run();

            var regionManager = bootstrapper.BaseContainer.Resolve<IRegionManager>();
            Assert.NotNull(regionManager);
            Assert.True(regionManager.GetType().IsClass);
            Assert.Contains(typeof(IRegionManager), regionManager.GetType().GetInterfaces());
        }

        [StaFact]
        public void RunRegistersTypeForRegionAdapterMappings()
        {
            var bootstrapper = new DefaultDryIocBootstrapper();

            bootstrapper.Run();

            var regionAdapterMappings = bootstrapper.BaseContainer.Resolve<RegionAdapterMappings>();
            Assert.NotNull(regionAdapterMappings);
            Assert.Equal(typeof(RegionAdapterMappings), regionAdapterMappings.GetType());
        }

        [StaFact]
        public void RunRegistersTypeForIRegionViewRegistry()
        {
            var bootstrapper = new DefaultDryIocBootstrapper();

            bootstrapper.Run();

            var regionViewRegistry = bootstrapper.BaseContainer.Resolve<IRegionViewRegistry>();
            Assert.NotNull(regionViewRegistry);
            Assert.True(regionViewRegistry.GetType().IsClass);
            Assert.Contains(typeof(IRegionViewRegistry), regionViewRegistry.GetType().GetInterfaces());
        }

        [StaFact]
        public void RunRegistersTypeForIRegionBehaviorFactory()
        {
            var bootstrapper = new DefaultDryIocBootstrapper();

            bootstrapper.Run();

            var regionBehaviorFactory = bootstrapper.BaseContainer.Resolve<IRegionBehaviorFactory>();
            Assert.NotNull(regionBehaviorFactory);
            Assert.True(regionBehaviorFactory.GetType().IsClass);
            Assert.Contains(typeof(IRegionBehaviorFactory), regionBehaviorFactory.GetType().GetInterfaces());
        }

        [StaFact]
        public void RunRegistersTypeForIEventAggregator()
        {
            var bootstrapper = new DefaultDryIocBootstrapper();

            bootstrapper.Run();

            var eventAggregator = bootstrapper.BaseContainer.Resolve<IEventAggregator>();
            Assert.NotNull(eventAggregator);
            Assert.True(eventAggregator.GetType().IsClass);
            Assert.Contains(typeof(IEventAggregator), eventAggregator.GetType().GetInterfaces());
        }

        [StaFact]
        public void RunShouldCallTheMethodsInOrder()
        {
            var bootstrapper = new DefaultDryIocBootstrapper();
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

        [StaFact]
        public void RunShouldLogBootstrapperSteps()
        {
            var bootstrapper = new DefaultDryIocBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.Contains("Logger was created successfully.", messages[0]);
            Assert.Contains("Creating module catalog.", messages[1]);
            Assert.Contains("Configuring module catalog.", messages[2]);
            Assert.Contains("Creating DryIoc container.", messages[3]);
            Assert.Contains("Configuring the DryIoc container.", messages[4]);
            Assert.Contains("Configuring ServiceLocator singleton.", messages[5]);
            Assert.Contains("Configuring the ViewModelLocator to use DryIoc.", messages[6]);
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

        [StaFact]
        public void RunShouldLogLoggerCreationSuccess()
        {
            const string expectedMessageText = "Logger was created successfully.";
            var bootstrapper = new DefaultDryIocBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.True(messages.Contains(expectedMessageText));
        }
        [StaFact]
        public void RunShouldLogAboutModuleCatalogCreation()
        {
            const string expectedMessageText = "Creating module catalog.";
            var bootstrapper = new DefaultDryIocBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.True(messages.Contains(expectedMessageText));
        }

        [StaFact]
        public void RunShouldLogAboutConfiguringModuleCatalog()
        {
            const string expectedMessageText = "Configuring module catalog.";
            var bootstrapper = new DefaultDryIocBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.True(messages.Contains(expectedMessageText));
        }

        [StaFact]
        public void RunShouldLogAboutCreatingTheContainer()
        {
            const string expectedMessageText = "Creating DryIoc container.";
            var bootstrapper = new DefaultDryIocBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.True(messages.Contains(expectedMessageText));
        }

        [StaFact]
        public void RunShouldLogAboutConfiguringContainerBuilder()
        {
            const string expectedMessageText = "Configuring the DryIoc container.";
            var bootstrapper = new DefaultDryIocBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.True(messages.Contains(expectedMessageText));
        }

        [StaFact]
        public void RunShouldLogAboutConfiguringRegionAdapters()
        {
            const string expectedMessageText = "Configuring region adapters.";
            var bootstrapper = new DefaultDryIocBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.True(messages.Contains(expectedMessageText));
        }


        [StaFact]
        public void RunShouldLogAboutConfiguringRegionBehaviors()
        {
            const string expectedMessageText = "Configuring default region behaviors.";
            var bootstrapper = new DefaultDryIocBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.True(messages.Contains(expectedMessageText));
        }

        [StaFact]
        public void RunShouldLogAboutRegisteringFrameworkExceptionTypes()
        {
            const string expectedMessageText = "Registering Framework Exception Types.";
            var bootstrapper = new DefaultDryIocBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.True(messages.Contains(expectedMessageText));
        }

        [StaFact]
        public void RunShouldLogAboutCreatingTheShell()
        {
            const string expectedMessageText = "Creating the shell.";
            var bootstrapper = new DefaultDryIocBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.True(messages.Contains(expectedMessageText));
        }

        [StaFact]
        public void RunShouldLogAboutInitializingTheShellIfShellCreated()
        {
            const string expectedMessageText = "Initializing the shell.";
            var bootstrapper = new DefaultDryIocBootstrapper();

            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.True(messages.Contains(expectedMessageText));
        }

        [StaFact]
        public void RunShouldNotLogAboutInitializingTheShellIfShellIsNotCreated()
        {
            const string expectedMessageText = "Initializing shell";
            var bootstrapper = new DefaultDryIocBootstrapper { ShellObject = null };

            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.False(messages.Contains(expectedMessageText));
        }

        [StaFact]
        public void RunShouldLogAboutInitializingModules()
        {
            const string expectedMessageText = "Initializing modules.";
            var bootstrapper = new DefaultDryIocBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.True(messages.Contains(expectedMessageText));
        }

        [StaFact]
        public void RunShouldLogAboutRunCompleting()
        {
            const string expectedMessageText = "Bootstrapper sequence completed.";
            var bootstrapper = new DefaultDryIocBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.True(messages.Contains(expectedMessageText));
        }
    }
}
