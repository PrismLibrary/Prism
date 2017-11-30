using CommonServiceLocator;
using Grace.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Events;
using Prism.Logging;
using Prism.Modularity;
using Prism.Regions;
using System.Linq;

namespace Prism.Grace.Wpf.Tests
{
    [TestClass]
    public class GraceBootstrapperRunMethodFixture
    {
        [TestMethod]
        public void CanRunBootstrapper()
        {
            var bootstrapper = new DefaultGraceBootstrapper();
            bootstrapper.Run();
        }

        [TestMethod]
        public void RunShouldNotFailIfReturnedNullShell()
        {
            var bootstrapper = new DefaultGraceBootstrapper { ShellObject = null };
            bootstrapper.Run();
        }

        [TestMethod]
        public void RunConfiguresServiceLocatorProvider()
        {
            var bootstrapper = new DefaultGraceBootstrapper();
            bootstrapper.Run();

            Assert.IsTrue(ServiceLocator.Current is GraceServiceLocatorAdapter);
        }

        [TestMethod]
        public void RunShouldInitializeContainer()
        {
            var bootstrapper = new DefaultGraceBootstrapper();
            var container = bootstrapper.BaseContainer;

            Assert.IsNull(container);

            bootstrapper.Run();

            container = bootstrapper.BaseContainer;

            Assert.IsNotNull(container);
            Assert.IsInstanceOfType(container, typeof(DependencyInjectionContainer));
        }

        [TestMethod]
        public void RunAddsCompositionContainerToContainer()
        {
            var bootstrapper = new DefaultGraceBootstrapper();

            var createdContainer = bootstrapper.CallCreateContainer();
            var returnedContainer = createdContainer.Locate<DependencyInjectionContainer>();
            Assert.IsNotNull(returnedContainer);
            Assert.AreEqual(typeof(DependencyInjectionContainer), returnedContainer.GetType());
        }

        [TestMethod]
        public void RunShouldCallInitializeModules()
        {
            var bootstrapper = new DefaultGraceBootstrapper();
            bootstrapper.Run();

            Assert.IsTrue(bootstrapper.InitializeModulesCalled);
        }

        [TestMethod]
        public void RunShouldCallConfigureDefaultRegionBehaviors()
        {
            var bootstrapper = new DefaultGraceBootstrapper();

            bootstrapper.Run();

            Assert.IsTrue(bootstrapper.ConfigureDefaultRegionBehaviorsCalled);
        }

        [TestMethod]
        public void RunShouldCallConfigureRegionAdapterMappings()
        {
            var bootstrapper = new DefaultGraceBootstrapper();

            bootstrapper.Run();

            Assert.IsTrue(bootstrapper.ConfigureRegionAdapterMappingsCalled);
        }

        [TestMethod]
        public void RunShouldAssignRegionManagerToReturnedShell()
        {
            var bootstrapper = new DefaultGraceBootstrapper();

            bootstrapper.Run();

            Assert.IsNotNull(RegionManager.GetRegionManager(bootstrapper.BaseShell));
        }

        [TestMethod]
        public void RunShouldCallCreateLogger()
        {
            var bootstrapper = new DefaultGraceBootstrapper();

            bootstrapper.Run();

            Assert.IsTrue(bootstrapper.CreateLoggerCalled);
        }

        [TestMethod]
        public void RunShouldCallCreateModuleCatalog()
        {
            var bootstrapper = new DefaultGraceBootstrapper();

            bootstrapper.Run();

            Assert.IsTrue(bootstrapper.CreateModuleCatalogCalled);
        }

        [TestMethod]
        public void RunShouldCallConfigureModuleCatalog()
        {
            var bootstrapper = new DefaultGraceBootstrapper();

            bootstrapper.Run();

            Assert.IsTrue(bootstrapper.ConfigureModuleCatalogCalled);
        }

        [TestMethod]
        public void RunShouldCallCreateContainer()
        {
            var bootstrapper = new DefaultGraceBootstrapper();

            bootstrapper.Run();

            Assert.IsTrue(bootstrapper.CreateContainerCalled);
        }

        [TestMethod]
        public void RunShouldCallCreateShell()
        {
            var bootstrapper = new DefaultGraceBootstrapper();

            bootstrapper.Run();

            Assert.IsTrue(bootstrapper.CreateShellCalled);
        }

        [TestMethod]
        public void RunShouldCallConfigureContainer()
        {
            var bootstrapper = new DefaultGraceBootstrapper();

            bootstrapper.Run();

            Assert.IsTrue(bootstrapper.ConfigureContainerCalled);
        }

        [TestMethod]
        public void RunRegistersInstanceOfILoggerFacade()
        {
            var bootstrapper = new DefaultGraceBootstrapper();

            bootstrapper.Run();

            var logger = bootstrapper.BaseContainer.Locate<ILoggerFacade>();
            Assert.IsNotNull(logger);
            Assert.IsTrue(logger.GetType().IsClass);
            Assert.IsTrue(logger.GetType().GetInterfaces().Contains(typeof(ILoggerFacade)));
        }

        [TestMethod]
        public void RunRegistersInstanceOfIModuleCatalog()
        {
            var bootstrapper = new DefaultGraceBootstrapper();

            bootstrapper.Run();

            var moduleCatalog = bootstrapper.BaseContainer.Locate<IModuleCatalog>();
            Assert.IsNotNull(moduleCatalog);
            Assert.IsTrue(moduleCatalog.GetType().IsClass);
            Assert.IsTrue(moduleCatalog.GetType().GetInterfaces().Contains(typeof(IModuleCatalog)));
        }

        [TestMethod]
        public void RunRegistersTypeForIServiceLocator()
        {
            var bootstrapper = new DefaultGraceBootstrapper();

            bootstrapper.Run();

            var serviceLocator = bootstrapper.BaseContainer.Locate<IServiceLocator>();
            Assert.IsNotNull(serviceLocator);
            Assert.IsTrue(serviceLocator.GetType().IsClass);
            Assert.AreEqual(typeof(GraceServiceLocatorAdapter), serviceLocator.GetType());
            Assert.IsTrue(serviceLocator.GetType().GetInterfaces().Contains(typeof(IServiceLocator)));
        }

        [TestMethod]
        public void RunRegistersTypeForIModuleInitializer()
        {
            var bootstrapper = new DefaultGraceBootstrapper();

            bootstrapper.Run();

            var moduleInitializer = bootstrapper.BaseContainer.Locate<IModuleInitializer>();
            Assert.IsNotNull(moduleInitializer);
            Assert.IsTrue(moduleInitializer.GetType().IsClass);
            Assert.IsTrue(moduleInitializer.GetType().GetInterfaces().Contains(typeof(IModuleInitializer)));
        }

        [TestMethod]
        public void RunRegistersTypeForIRegionManager()
        {
            var bootstrapper = new DefaultGraceBootstrapper();

            bootstrapper.Run();

            var regionManager = bootstrapper.BaseContainer.Locate<IRegionManager>();
            Assert.IsNotNull(regionManager);
            Assert.IsTrue(regionManager.GetType().IsClass);
            Assert.IsTrue(regionManager.GetType().GetInterfaces().Contains(typeof(IRegionManager)));
        }

        [TestMethod]
        public void RunRegistersTypeForRegionAdapterMappings()
        {
            var bootstrapper = new DefaultGraceBootstrapper();

            bootstrapper.Run();

            var regionAdapterMappings = bootstrapper.BaseContainer.Locate<RegionAdapterMappings>();
            Assert.IsNotNull(regionAdapterMappings);
            Assert.AreEqual(typeof(RegionAdapterMappings), regionAdapterMappings.GetType());
        }

        [TestMethod]
        public void RunRegistersTypeForIRegionViewRegistry()
        {
            var bootstrapper = new DefaultGraceBootstrapper();

            bootstrapper.Run();

            var regionViewRegistry = bootstrapper.BaseContainer.Locate<IRegionViewRegistry>();
            Assert.IsNotNull(regionViewRegistry);
            Assert.IsTrue(regionViewRegistry.GetType().IsClass);
            Assert.IsTrue(regionViewRegistry.GetType().GetInterfaces().Contains(typeof(IRegionViewRegistry)));
        }

        [TestMethod]
        public void RunRegistersTypeForIRegionBehaviorFactory()
        {
            var bootstrapper = new DefaultGraceBootstrapper();

            bootstrapper.Run();

            var regionBehaviorFactory = bootstrapper.BaseContainer.Locate<IRegionBehaviorFactory>();
            Assert.IsNotNull(regionBehaviorFactory);
            Assert.IsTrue(regionBehaviorFactory.GetType().IsClass);
            Assert.IsTrue(regionBehaviorFactory.GetType().GetInterfaces().Contains(typeof(IRegionBehaviorFactory)));
        }

        [TestMethod]
        public void RunRegistersTypeForIEventAggregator()
        {
            var bootstrapper = new DefaultGraceBootstrapper();

            bootstrapper.Run();

            var eventAggregator = bootstrapper.BaseContainer.Locate<IEventAggregator>();
            Assert.IsNotNull(eventAggregator);
            Assert.IsTrue(eventAggregator.GetType().IsClass);
            Assert.IsTrue(eventAggregator.GetType().GetInterfaces().Contains(typeof(IEventAggregator)));
        }

        [TestMethod]
        public void RunShouldCallTheMethodsInOrder()
        {
            var bootstrapper = new DefaultGraceBootstrapper();
            bootstrapper.Run();

            Assert.AreEqual("CreateLogger", bootstrapper.MethodCalls[0]);
            Assert.AreEqual("CreateModuleCatalog", bootstrapper.MethodCalls[1]);
            Assert.AreEqual("ConfigureModuleCatalog", bootstrapper.MethodCalls[2]);
            Assert.AreEqual("CreateContainer", bootstrapper.MethodCalls[3]);
            Assert.AreEqual("ConfigureContainer", bootstrapper.MethodCalls[4]);
            Assert.AreEqual("ConfigureServiceLocator", bootstrapper.MethodCalls[5]);
            Assert.AreEqual("ConfigureViewModelLocator", bootstrapper.MethodCalls[6]);
            Assert.AreEqual("ConfigureRegionAdapterMappings", bootstrapper.MethodCalls[7]);
            Assert.AreEqual("ConfigureDefaultRegionBehaviors", bootstrapper.MethodCalls[8]);
            Assert.AreEqual("RegisterFrameworkExceptionTypes", bootstrapper.MethodCalls[9]);
            Assert.AreEqual("CreateShell", bootstrapper.MethodCalls[10]);
            Assert.AreEqual("InitializeShell", bootstrapper.MethodCalls[11]);
            Assert.AreEqual("InitializeModules", bootstrapper.MethodCalls[12]);
        }

        [TestMethod]
        public void RunShouldLogBootstrapperSteps()
        {
            var bootstrapper = new DefaultGraceBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsTrue(messages[0].Contains("Logger was created successfully."));
            Assert.IsTrue(messages[1].Contains("Creating module catalog."));
            Assert.IsTrue(messages[2].Contains("Configuring module catalog."));
            Assert.IsTrue(messages[3].Contains("Creating Grace container."));
            Assert.IsTrue(messages[4].Contains("Configuring the Grace container."));
            Assert.IsTrue(messages[5].Contains("Configuring ServiceLocator singleton."));
            Assert.IsTrue(messages[6].Contains("Configuring the ViewModelLocator to use Grace."));
            Assert.IsTrue(messages[7].Contains("Configuring region adapters."));
            Assert.IsTrue(messages[8].Contains("Configuring default region behaviors."));
            Assert.IsTrue(messages[9].Contains("Registering Framework Exception Types."));
            Assert.IsTrue(messages[10].Contains("Creating the shell."));
            Assert.IsTrue(messages[11].Contains("Setting the RegionManager."));
            Assert.IsTrue(messages[12].Contains("Updating Regions."));
            Assert.IsTrue(messages[13].Contains("Initializing the shell."));
            Assert.IsTrue(messages[14].Contains("Initializing modules."));
            Assert.IsTrue(messages[15].Contains("Bootstrapper sequence completed."));
        }

        [TestMethod]
        public void RunShouldLogLoggerCreationSuccess()
        {
            const string expectedMessageText = "Logger was created successfully.";
            var bootstrapper = new DefaultGraceBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsTrue(messages.Contains(expectedMessageText));
        }
        [TestMethod]
        public void RunShouldLogAboutModuleCatalogCreation()
        {
            const string expectedMessageText = "Creating module catalog.";
            var bootstrapper = new DefaultGraceBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsTrue(messages.Contains(expectedMessageText));
        }

        [TestMethod]
        public void RunShouldLogAboutConfiguringModuleCatalog()
        {
            const string expectedMessageText = "Configuring module catalog.";
            var bootstrapper = new DefaultGraceBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsTrue(messages.Contains(expectedMessageText));
        }

        [TestMethod]
        public void RunShouldLogAboutCreatingTheContainer()
        {
            const string expectedMessageText = "Creating Grace container.";
            var bootstrapper = new DefaultGraceBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsTrue(messages.Contains(expectedMessageText));
        }

        [TestMethod]
        public void RunShouldLogAboutConfiguringContainerBuilder()
        {
            const string expectedMessageText = "Configuring the Grace container.";
            var bootstrapper = new DefaultGraceBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsTrue(messages.Contains(expectedMessageText));
        }

        [TestMethod]
        public void RunShouldLogAboutConfiguringRegionAdapters()
        {
            const string expectedMessageText = "Configuring region adapters.";
            var bootstrapper = new DefaultGraceBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsTrue(messages.Contains(expectedMessageText));
        }


        [TestMethod]
        public void RunShouldLogAboutConfiguringRegionBehaviors()
        {
            const string expectedMessageText = "Configuring default region behaviors.";
            var bootstrapper = new DefaultGraceBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsTrue(messages.Contains(expectedMessageText));
        }

        [TestMethod]
        public void RunShouldLogAboutRegisteringFrameworkExceptionTypes()
        {
            const string expectedMessageText = "Registering Framework Exception Types.";
            var bootstrapper = new DefaultGraceBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsTrue(messages.Contains(expectedMessageText));
        }

        [TestMethod]
        public void RunShouldLogAboutCreatingTheShell()
        {
            const string expectedMessageText = "Creating the shell.";
            var bootstrapper = new DefaultGraceBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsTrue(messages.Contains(expectedMessageText));
        }

        [TestMethod]
        public void RunShouldLogAboutInitializingTheShellIfShellCreated()
        {
            const string expectedMessageText = "Initializing the shell.";
            var bootstrapper = new DefaultGraceBootstrapper();

            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsTrue(messages.Contains(expectedMessageText));
        }

        [TestMethod]
        public void RunShouldNotLogAboutInitializingTheShellIfShellIsNotCreated()
        {
            const string expectedMessageText = "Initializing shell";
            var bootstrapper = new DefaultGraceBootstrapper { ShellObject = null };

            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsFalse(messages.Contains(expectedMessageText));
        }

        [TestMethod]
        public void RunShouldLogAboutInitializingModules()
        {
            const string expectedMessageText = "Initializing modules.";
            var bootstrapper = new DefaultGraceBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsTrue(messages.Contains(expectedMessageText));
        }

        [TestMethod]
        public void RunShouldLogAboutRunCompleting()
        {
            const string expectedMessageText = "Bootstrapper sequence completed.";
            var bootstrapper = new DefaultGraceBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsTrue(messages.Contains(expectedMessageText));
        }
    }
}
