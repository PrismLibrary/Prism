using System.Linq;
using Autofac;
using CommonServiceLocator;
using Xunit;
using Prism.Events;
using Prism.Logging;
using Prism.Modularity;
using Prism.Regions;

namespace Prism.Autofac.Wpf.Tests
{
    
    public class AutofacBootstrapperRunMethodFixture
    {
        [Fact]
        public void CanRunBootstrapper()
        {
            var bootstrapper = new DefaultAutofacBootstrapper();
            bootstrapper.Run();
        }

        [Fact]
        public void RunShouldNotFailIfReturnedNullShell()
        {
            var bootstrapper = new DefaultAutofacBootstrapper { ShellObject = null };
            bootstrapper.Run();
        }

        [Fact]
        public void RunConfiguresServiceLocatorProvider()
        {
            var bootstrapper = new DefaultAutofacBootstrapper();
            bootstrapper.Run();

            Assert.True(ServiceLocator.Current is AutofacServiceLocatorAdapter);
        }

        [Fact]
        public void RunShouldInitializeContainer()
        {
            var bootstrapper = new DefaultAutofacBootstrapper();
            var container = bootstrapper.BaseContainer;

            Assert.Null(container);

            bootstrapper.Run();

            container = bootstrapper.BaseContainer;

            Assert.NotNull(container);
            Assert.IsAssignableFrom<IContainer>(container);
        }

        [Fact]
        public void RunAddsCompositionContainerToContainer()
        {
            var bootstrapper = new DefaultAutofacBootstrapper();

            var createdContainer = bootstrapper.CallCreateContainer();
            var returnedContainer = createdContainer.Resolve<IContainer>();
            Assert.NotNull(returnedContainer);
            Assert.Contains(typeof(IContainer), returnedContainer.GetType().GetInterfaces());
        }

        [Fact]
        public void RunShouldCallInitializeModules()
        {
            var bootstrapper = new DefaultAutofacBootstrapper();
            bootstrapper.Run();

            Assert.True(bootstrapper.InitializeModulesCalled);
        }

        [Fact]
        public void RunShouldCallConfigureDefaultRegionBehaviors()
        {
            var bootstrapper = new DefaultAutofacBootstrapper();

            bootstrapper.Run();

            Assert.True(bootstrapper.ConfigureDefaultRegionBehaviorsCalled);
        }

        [Fact]
        public void RunShouldCallConfigureRegionAdapterMappings()
        {
            var bootstrapper = new DefaultAutofacBootstrapper();

            bootstrapper.Run();

            Assert.True(bootstrapper.ConfigureRegionAdapterMappingsCalled);
        }

        [Fact]
        public void RunShouldAssignRegionManagerToReturnedShell()
        {
            var bootstrapper = new DefaultAutofacBootstrapper();

            bootstrapper.Run();

            Assert.NotNull(RegionManager.GetRegionManager(bootstrapper.BaseShell));
        }

        [Fact]
        public void RunShouldCallCreateLogger()
        {
            var bootstrapper = new DefaultAutofacBootstrapper();

            bootstrapper.Run();

            Assert.True(bootstrapper.CreateLoggerCalled);
        }

        [Fact]
        public void RunShouldCallCreateModuleCatalog()
        {
            var bootstrapper = new DefaultAutofacBootstrapper();

            bootstrapper.Run();

            Assert.True(bootstrapper.CreateModuleCatalogCalled);
        }

        [Fact]
        public void RunShouldCallConfigureModuleCatalog()
        {
            var bootstrapper = new DefaultAutofacBootstrapper();

            bootstrapper.Run();

            Assert.True(bootstrapper.ConfigureModuleCatalogCalled);
        }

        [Fact]
        public void RunShouldCallCreateContainerBuilder()
        {
            var bootstrapper = new DefaultAutofacBootstrapper();

            bootstrapper.Run();

            Assert.True(bootstrapper.CreateContainerBuilderCalled);
        }

        [Fact]
        public void RunShouldCallCreateContainer()
        {
            var bootstrapper = new DefaultAutofacBootstrapper();

            bootstrapper.Run();

            Assert.True(bootstrapper.CreateContainerCalled);
        }

        [Fact]
        public void RunShouldCallCreateShell()
        {
            var bootstrapper = new DefaultAutofacBootstrapper();

            bootstrapper.Run();

            Assert.True(bootstrapper.CreateShellCalled);
        }

        [Fact]
        public void RunShouldCallConfigureContainerBuilder()
        {
            var bootstrapper = new DefaultAutofacBootstrapper();

            bootstrapper.Run();

            Assert.True(bootstrapper.ConfigureContainerBuilderCalled);
        }

        // unable to mock extension RegisterInstance/RegisterType methods
        // so registration is tested through checking the resolved type against interface
        [Fact]
        public void RunRegistersInstanceOfILoggerFacade()
        {
            var bootstrapper = new DefaultAutofacBootstrapper();

            bootstrapper.Run();

            var logger = bootstrapper.BaseContainer.Resolve<ILoggerFacade>();
            Assert.NotNull(logger);
            Assert.True(logger.GetType().IsClass);
            Assert.Contains(typeof(ILoggerFacade), logger.GetType().GetInterfaces());
        }

        [Fact]
        public void RunRegistersInstanceOfIModuleCatalog()
        {
            var bootstrapper = new DefaultAutofacBootstrapper();

            bootstrapper.Run();

            var moduleCatalog = bootstrapper.BaseContainer.Resolve<IModuleCatalog>();
            Assert.NotNull(moduleCatalog);
            Assert.True(moduleCatalog.GetType().IsClass);
            Assert.Contains(typeof(IModuleCatalog), moduleCatalog.GetType().GetInterfaces());
        }

        [Fact]
        public void RunRegistersTypeForIServiceLocator()
        {
            var bootstrapper = new DefaultAutofacBootstrapper();

            bootstrapper.Run();

            var serviceLocator = bootstrapper.BaseContainer.Resolve<IServiceLocator>();
            Assert.NotNull(serviceLocator);
            Assert.True(serviceLocator.GetType().IsClass);
            Assert.Equal(typeof(AutofacServiceLocatorAdapter), serviceLocator.GetType());
            Assert.Contains(typeof(IServiceLocator), serviceLocator.GetType().GetInterfaces());
        }

        [Fact]
        public void RunRegistersTypeForIModuleInitializer()
        {
            var bootstrapper = new DefaultAutofacBootstrapper();

            bootstrapper.Run();

            var moduleInitializer = bootstrapper.BaseContainer.Resolve<IModuleInitializer>();
            Assert.NotNull(moduleInitializer);
            Assert.True(moduleInitializer.GetType().IsClass);
            Assert.Contains(typeof(IModuleInitializer), moduleInitializer.GetType().GetInterfaces());
        }

        [Fact]
        public void RunRegistersTypeForIRegionManager()
        {
            var bootstrapper = new DefaultAutofacBootstrapper();

            bootstrapper.Run();

            var regionManager = bootstrapper.BaseContainer.Resolve<IRegionManager>();
            Assert.NotNull(regionManager);
            Assert.True(regionManager.GetType().IsClass);
            Assert.Contains(typeof(IRegionManager), regionManager.GetType().GetInterfaces());
        }

        [Fact]
        public void RunRegistersTypeForRegionAdapterMappings()
        {
            var bootstrapper = new DefaultAutofacBootstrapper();

            bootstrapper.Run();

            var regionAdapterMappings = bootstrapper.BaseContainer.Resolve<RegionAdapterMappings>();
            Assert.NotNull(regionAdapterMappings);
            Assert.Equal(typeof(RegionAdapterMappings), regionAdapterMappings.GetType());
        }

        [Fact]
        public void RunRegistersTypeForIRegionViewRegistry()
        {
            var bootstrapper = new DefaultAutofacBootstrapper();

            bootstrapper.Run();

            var regionViewRegistry = bootstrapper.BaseContainer.Resolve<IRegionViewRegistry>();
            Assert.NotNull(regionViewRegistry);
            Assert.True(regionViewRegistry.GetType().IsClass);
            Assert.Contains(typeof(IRegionViewRegistry), regionViewRegistry.GetType().GetInterfaces());
        }

        [Fact]
        public void RunRegistersTypeForIRegionBehaviorFactory()
        {
            var bootstrapper = new DefaultAutofacBootstrapper();

            bootstrapper.Run();

            var regionBehaviorFactory = bootstrapper.BaseContainer.Resolve<IRegionBehaviorFactory>();
            Assert.NotNull(regionBehaviorFactory);
            Assert.True(regionBehaviorFactory.GetType().IsClass);
            Assert.Contains(typeof(IRegionBehaviorFactory), regionBehaviorFactory.GetType().GetInterfaces());
        }

        [Fact]
        public void RunRegistersTypeForIEventAggregator()
        {
            var bootstrapper = new DefaultAutofacBootstrapper();

            bootstrapper.Run();

            var eventAggregator = bootstrapper.BaseContainer.Resolve<IEventAggregator>();
            Assert.NotNull(eventAggregator);
            Assert.True(eventAggregator.GetType().IsClass);
            Assert.Contains(typeof(IEventAggregator), eventAggregator.GetType().GetInterfaces());
        }

        [Fact]
        public void RunShouldCallTheMethodsInOrder()
        {
            var bootstrapper = new DefaultAutofacBootstrapper();
            bootstrapper.Run();

            Assert.Equal("CreateLogger", bootstrapper.MethodCalls[0]);
            Assert.Equal("CreateModuleCatalog", bootstrapper.MethodCalls[1]);
            Assert.Equal("ConfigureModuleCatalog", bootstrapper.MethodCalls[2]);
            Assert.Equal("CreateContainerBuilder", bootstrapper.MethodCalls[3]);
            Assert.Equal("ConfigureContainerBuilder", bootstrapper.MethodCalls[4]);
            Assert.Equal("CreateContainer", bootstrapper.MethodCalls[5]);
            Assert.Equal("ConfigureServiceLocator", bootstrapper.MethodCalls[6]);
            Assert.Equal("CreateContainerBuilder", bootstrapper.MethodCalls[7]); // update container
            Assert.Equal("ConfigureRegionAdapterMappings", bootstrapper.MethodCalls[8]);
            Assert.Equal("ConfigureDefaultRegionBehaviors", bootstrapper.MethodCalls[9]);
            Assert.Equal("RegisterFrameworkExceptionTypes", bootstrapper.MethodCalls[10]);
            Assert.Equal("CreateShell", bootstrapper.MethodCalls[11]);
            Assert.Equal("InitializeShell", bootstrapper.MethodCalls[12]);
            Assert.Equal("InitializeModules", bootstrapper.MethodCalls[13]);
        }

        [Fact]
        public void RunShouldLogBootstrapperSteps()
        {
            var bootstrapper = new DefaultAutofacBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.Contains("Logger was created successfully.", messages[0]);
            Assert.Contains("Creating module catalog.", messages[1]);
            Assert.Contains("Configuring module catalog.", messages[2]);
            Assert.Contains("Creating Autofac container builder.", messages[3]);
            Assert.Contains("Configuring the Autofac container builder.", messages[4]);
            Assert.Contains("Creating Autofac container.", messages[5]);
            Assert.Contains("Configuring ServiceLocator singleton.", messages[6]);
            Assert.Contains("Configuring the ViewModelLocator to use Autofac.", messages[7]);
            Assert.Contains("Configuring region adapters.", messages[8]);
            Assert.Contains("Configuring default region behaviors.", messages[9]);
            Assert.Contains("Registering Framework Exception Types.", messages[10]);
            Assert.Contains("Creating the shell.", messages[11]);
            Assert.Contains("Setting the RegionManager.", messages[12]);
            Assert.Contains("Updating Regions.", messages[13]);
            Assert.Contains("Initializing the shell.", messages[14]);
            Assert.Contains("Initializing modules.", messages[15]);
            Assert.Contains("Bootstrapper sequence completed.", messages[16]);
        }

        [Fact]
        public void RunShouldLogLoggerCreationSuccess()
        {
            const string expectedMessageText = "Logger was created successfully.";
            var bootstrapper = new DefaultAutofacBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.True(messages.Contains(expectedMessageText));
        }
        [Fact]
        public void RunShouldLogAboutModuleCatalogCreation()
        {
            const string expectedMessageText = "Creating module catalog.";
            var bootstrapper = new DefaultAutofacBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.True(messages.Contains(expectedMessageText));
        }

        [Fact]
        public void RunShouldLogAboutConfiguringModuleCatalog()
        {
            const string expectedMessageText = "Configuring module catalog.";
            var bootstrapper = new DefaultAutofacBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.True(messages.Contains(expectedMessageText));
        }

        [Fact]
        public void RunShouldLogAboutCreatingTheContainerBuilder()
        {
            const string expectedMessageText = "Creating Autofac container builder.";
            var bootstrapper = new DefaultAutofacBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.True(messages.Contains(expectedMessageText));
        }

        [Fact]
        public void RunShouldLogAboutCreatingTheContainer()
        {
            const string expectedMessageText = "Creating Autofac container.";
            var bootstrapper = new DefaultAutofacBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.True(messages.Contains(expectedMessageText));
        }

        [Fact]
        public void RunShouldLogAboutConfiguringContainerBuilder()
        {
            const string expectedMessageText = "Configuring the Autofac container builder.";
            var bootstrapper = new DefaultAutofacBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.True(messages.Contains(expectedMessageText));
        }

        [Fact]
        public void RunShouldLogAboutConfiguringRegionAdapters()
        {
            const string expectedMessageText = "Configuring region adapters.";
            var bootstrapper = new DefaultAutofacBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.True(messages.Contains(expectedMessageText));
        }


        [Fact]
        public void RunShouldLogAboutConfiguringRegionBehaviors()
        {
            const string expectedMessageText = "Configuring default region behaviors.";
            var bootstrapper = new DefaultAutofacBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.True(messages.Contains(expectedMessageText));
        }

        [Fact]
        public void RunShouldLogAboutRegisteringFrameworkExceptionTypes()
        {
            const string expectedMessageText = "Registering Framework Exception Types.";
            var bootstrapper = new DefaultAutofacBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.True(messages.Contains(expectedMessageText));
        }

        [Fact]
        public void RunShouldLogAboutCreatingTheShell()
        {
            const string expectedMessageText = "Creating the shell.";
            var bootstrapper = new DefaultAutofacBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.True(messages.Contains(expectedMessageText));
        }

        [Fact]
        public void RunShouldLogAboutInitializingTheShellIfShellCreated()
        {
            const string expectedMessageText = "Initializing the shell.";
            var bootstrapper = new DefaultAutofacBootstrapper();

            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.True(messages.Contains(expectedMessageText));
        }

        [Fact]
        public void RunShouldNotLogAboutInitializingTheShellIfShellIsNotCreated()
        {
            const string expectedMessageText = "Initializing shell";
            var bootstrapper = new DefaultAutofacBootstrapper { ShellObject = null };

            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.False(messages.Contains(expectedMessageText));
        }

        [Fact]
        public void RunShouldLogAboutInitializingModules()
        {
            const string expectedMessageText = "Initializing modules.";
            var bootstrapper = new DefaultAutofacBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.True(messages.Contains(expectedMessageText));
        }

        [Fact]
        public void RunShouldLogAboutRunCompleting()
        {
            const string expectedMessageText = "Bootstrapper sequence completed.";
            var bootstrapper = new DefaultAutofacBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.True(messages.Contains(expectedMessageText));
        }
    }
}
