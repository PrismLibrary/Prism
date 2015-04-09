using System.Linq;
using Autofac;
using Microsoft.Practices.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Events;
using Prism.Logging;
using Prism.Modularity;
using Prism.Regions;

namespace Prism.Autofac.Wpf.Tests
{
    [TestClass]
    public class AutofacBootstrapperRunMethodFixture
    {
        [TestMethod]
        public void CanRunBootstrapper()
        {
            var bootstrapper = new DefaultAutofacBootstrapper();
            bootstrapper.Run();
        }

        [TestMethod]
        public void RunShouldNotFailIfReturnedNullShell()
        {
            var bootstrapper = new DefaultAutofacBootstrapper { ShellObject = null };
            bootstrapper.Run();
        }

        [TestMethod]
        public void RunConfiguresServiceLocatorProvider()
        {
            var bootstrapper = new DefaultAutofacBootstrapper();
            bootstrapper.Run();

            Assert.IsTrue(ServiceLocator.Current is AutofacServiceLocatorAdapter);
        }

        [TestMethod]
        public void RunShouldInitializeContainer()
        {
            var bootstrapper = new DefaultAutofacBootstrapper();
            var container = bootstrapper.BaseContainer;

            Assert.IsNull(container);

            bootstrapper.Run();

            container = bootstrapper.BaseContainer;

            Assert.IsNotNull(container);
            Assert.IsInstanceOfType(container, typeof(IContainer));
        }

        [TestMethod]
        public void RunAddsCompositionContainerToContainer()
        {
            var bootstrapper = new DefaultAutofacBootstrapper();

            var createdContainer = bootstrapper.CallCreateContainer();
            var returnedContainer = createdContainer.Resolve<IContainer>();
            Assert.IsNotNull(returnedContainer);
            Assert.IsTrue(returnedContainer.GetType().GetInterfaces().Contains(typeof(IContainer)));
        }

        [TestMethod]
        public void RunShouldCallInitializeModules()
        {
            var bootstrapper = new DefaultAutofacBootstrapper();
            bootstrapper.Run();

            Assert.IsTrue(bootstrapper.InitializeModulesCalled);
        }

        [TestMethod]
        public void RunShouldCallConfigureDefaultRegionBehaviors()
        {
            var bootstrapper = new DefaultAutofacBootstrapper();

            bootstrapper.Run();

            Assert.IsTrue(bootstrapper.ConfigureDefaultRegionBehaviorsCalled);
        }

        [TestMethod]
        public void RunShouldCallConfigureRegionAdapterMappings()
        {
            var bootstrapper = new DefaultAutofacBootstrapper();

            bootstrapper.Run();

            Assert.IsTrue(bootstrapper.ConfigureRegionAdapterMappingsCalled);
        }

        [TestMethod]
        public void RunShouldAssignRegionManagerToReturnedShell()
        {
            var bootstrapper = new DefaultAutofacBootstrapper();

            bootstrapper.Run();

            Assert.IsNotNull(RegionManager.GetRegionManager(bootstrapper.BaseShell));
        }

        [TestMethod]
        public void RunShouldCallCreateLogger()
        {
            var bootstrapper = new DefaultAutofacBootstrapper();

            bootstrapper.Run();

            Assert.IsTrue(bootstrapper.CreateLoggerCalled);
        }

        [TestMethod]
        public void RunShouldCallCreateModuleCatalog()
        {
            var bootstrapper = new DefaultAutofacBootstrapper();

            bootstrapper.Run();

            Assert.IsTrue(bootstrapper.CreateModuleCatalogCalled);
        }

        [TestMethod]
        public void RunShouldCallConfigureModuleCatalog()
        {
            var bootstrapper = new DefaultAutofacBootstrapper();

            bootstrapper.Run();

            Assert.IsTrue(bootstrapper.ConfigureModuleCatalogCalled);
        }

        [TestMethod]
        public void RunShouldCallCreateContainerBuilder()
        {
            var bootstrapper = new DefaultAutofacBootstrapper();

            bootstrapper.Run();

            Assert.IsTrue(bootstrapper.CreateContainerBuilderCalled);
        }

        [TestMethod]
        public void RunShouldCallCreateContainer()
        {
            var bootstrapper = new DefaultAutofacBootstrapper();

            bootstrapper.Run();

            Assert.IsTrue(bootstrapper.CreateContainerCalled);
        }

        [TestMethod]
        public void RunShouldCallCreateShell()
        {
            var bootstrapper = new DefaultAutofacBootstrapper();

            bootstrapper.Run();

            Assert.IsTrue(bootstrapper.CreateShellCalled);
        }

        [TestMethod]
        public void RunShouldCallConfigureContainerBuilder()
        {
            var bootstrapper = new DefaultAutofacBootstrapper();

            bootstrapper.Run();

            Assert.IsTrue(bootstrapper.ConfigureContainerBuilderCalled);
        }

        // unable to mock extension RegisterInstance/RegisterType methods
        // so registration is tested through checking the resolved type against interface
        [TestMethod]
        public void RunRegistersInstanceOfILoggerFacade()
        {
            var bootstrapper = new DefaultAutofacBootstrapper();

            bootstrapper.Run();

            var logger = bootstrapper.BaseContainer.Resolve<ILoggerFacade>();
            Assert.IsNotNull(logger);
            Assert.IsTrue(logger.GetType().IsClass);
            Assert.IsTrue(logger.GetType().GetInterfaces().Contains(typeof(ILoggerFacade)));
        }

        [TestMethod]
        public void RunRegistersInstanceOfIModuleCatalog()
        {
            var bootstrapper = new DefaultAutofacBootstrapper();

            bootstrapper.Run();

            var moduleCatalog = bootstrapper.BaseContainer.Resolve<IModuleCatalog>();
            Assert.IsNotNull(moduleCatalog);
            Assert.IsTrue(moduleCatalog.GetType().IsClass);
            Assert.IsTrue(moduleCatalog.GetType().GetInterfaces().Contains(typeof(IModuleCatalog)));
        }

        [TestMethod]
        public void RunRegistersTypeForIServiceLocator()
        {
            var bootstrapper = new DefaultAutofacBootstrapper();

            bootstrapper.Run();

            var serviceLocator = bootstrapper.BaseContainer.Resolve<IServiceLocator>();
            Assert.IsNotNull(serviceLocator);
            Assert.IsTrue(serviceLocator.GetType().IsClass);
            Assert.AreEqual(typeof(AutofacServiceLocatorAdapter), serviceLocator.GetType());
            Assert.IsTrue(serviceLocator.GetType().GetInterfaces().Contains(typeof(IServiceLocator)));
        }

        [TestMethod]
        public void RunRegistersTypeForIModuleInitializer()
        {
            var bootstrapper = new DefaultAutofacBootstrapper();

            bootstrapper.Run();

            var moduleInitializer = bootstrapper.BaseContainer.Resolve<IModuleInitializer>();
            Assert.IsNotNull(moduleInitializer);
            Assert.IsTrue(moduleInitializer.GetType().IsClass);
            Assert.IsTrue(moduleInitializer.GetType().GetInterfaces().Contains(typeof(IModuleInitializer)));
        }

        [TestMethod]
        public void RunRegistersTypeForIRegionManager()
        {
            var bootstrapper = new DefaultAutofacBootstrapper();

            bootstrapper.Run();

            var regionManager = bootstrapper.BaseContainer.Resolve<IRegionManager>();
            Assert.IsNotNull(regionManager);
            Assert.IsTrue(regionManager.GetType().IsClass);
            Assert.IsTrue(regionManager.GetType().GetInterfaces().Contains(typeof(IRegionManager)));
        }

        [TestMethod]
        public void RunRegistersTypeForRegionAdapterMappings()
        {
            var bootstrapper = new DefaultAutofacBootstrapper();

            bootstrapper.Run();

            var regionAdapterMappings = bootstrapper.BaseContainer.Resolve<RegionAdapterMappings>();
            Assert.IsNotNull(regionAdapterMappings);
            Assert.AreEqual(typeof(RegionAdapterMappings), regionAdapterMappings.GetType());
        }

        [TestMethod]
        public void RunRegistersTypeForIRegionViewRegistry()
        {
            var bootstrapper = new DefaultAutofacBootstrapper();

            bootstrapper.Run();

            var regionViewRegistry = bootstrapper.BaseContainer.Resolve<IRegionViewRegistry>();
            Assert.IsNotNull(regionViewRegistry);
            Assert.IsTrue(regionViewRegistry.GetType().IsClass);
            Assert.IsTrue(regionViewRegistry.GetType().GetInterfaces().Contains(typeof(IRegionViewRegistry)));
        }

        [TestMethod]
        public void RunRegistersTypeForIRegionBehaviorFactory()
        {
            var bootstrapper = new DefaultAutofacBootstrapper();

            bootstrapper.Run();

            var regionBehaviorFactory = bootstrapper.BaseContainer.Resolve<IRegionBehaviorFactory>();
            Assert.IsNotNull(regionBehaviorFactory);
            Assert.IsTrue(regionBehaviorFactory.GetType().IsClass);
            Assert.IsTrue(regionBehaviorFactory.GetType().GetInterfaces().Contains(typeof(IRegionBehaviorFactory)));
        }

        [TestMethod]
        public void RunRegistersTypeForIEventAggregator()
        {
            var bootstrapper = new DefaultAutofacBootstrapper();

            bootstrapper.Run();

            var eventAggregator = bootstrapper.BaseContainer.Resolve<IEventAggregator>();
            Assert.IsNotNull(eventAggregator);
            Assert.IsTrue(eventAggregator.GetType().IsClass);
            Assert.IsTrue(eventAggregator.GetType().GetInterfaces().Contains(typeof(IEventAggregator)));
        }

        [TestMethod]
        public void RunShouldCallTheMethodsInOrder()
        {
            var bootstrapper = new DefaultAutofacBootstrapper();
            bootstrapper.Run();

            Assert.AreEqual("CreateLogger", bootstrapper.MethodCalls[0]);
            Assert.AreEqual("CreateModuleCatalog", bootstrapper.MethodCalls[1]);
            Assert.AreEqual("ConfigureModuleCatalog", bootstrapper.MethodCalls[2]);
            Assert.AreEqual("CreateContainerBuilder", bootstrapper.MethodCalls[3]);
            Assert.AreEqual("ConfigureContainerBuilder", bootstrapper.MethodCalls[4]);
            Assert.AreEqual("CreateContainer", bootstrapper.MethodCalls[5]);
            Assert.AreEqual("ConfigureServiceLocator", bootstrapper.MethodCalls[6]);
            Assert.AreEqual("CreateContainerBuilder", bootstrapper.MethodCalls[7]); // update container
            Assert.AreEqual("ConfigureRegionAdapterMappings", bootstrapper.MethodCalls[8]);
            Assert.AreEqual("ConfigureDefaultRegionBehaviors", bootstrapper.MethodCalls[9]);
            Assert.AreEqual("RegisterFrameworkExceptionTypes", bootstrapper.MethodCalls[10]);
            Assert.AreEqual("CreateShell", bootstrapper.MethodCalls[11]);
            Assert.AreEqual("InitializeShell", bootstrapper.MethodCalls[12]);
            Assert.AreEqual("InitializeModules", bootstrapper.MethodCalls[13]);
        }

        [TestMethod]
        public void RunShouldLogBootstrapperSteps()
        {
            var bootstrapper = new DefaultAutofacBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsTrue(messages[0].Contains("Logger was created successfully."));
            Assert.IsTrue(messages[1].Contains("Creating module catalog."));
            Assert.IsTrue(messages[2].Contains("Configuring module catalog."));
            Assert.IsTrue(messages[3].Contains("Creating Autofac container builder."));
            Assert.IsTrue(messages[4].Contains("Configuring the Autofac container builder."));
            Assert.IsTrue(messages[5].Contains("Creating Autofac container."));
            Assert.IsTrue(messages[6].Contains("Configuring ServiceLocator singleton."));
            Assert.IsTrue(messages[7].Contains("Configuring the ViewModelLocator to use Autofac."));
            Assert.IsTrue(messages[8].Contains("Configuring region adapters."));
            Assert.IsTrue(messages[9].Contains("Configuring default region behaviors."));
            Assert.IsTrue(messages[10].Contains("Registering Framework Exception Types."));
            Assert.IsTrue(messages[11].Contains("Creating the shell."));
            Assert.IsTrue(messages[12].Contains("Setting the RegionManager."));
            Assert.IsTrue(messages[13].Contains("Updating Regions."));
            Assert.IsTrue(messages[14].Contains("Initializing the shell."));
            Assert.IsTrue(messages[15].Contains("Initializing modules."));
            Assert.IsTrue(messages[16].Contains("Bootstrapper sequence completed."));
        }

        [TestMethod]
        public void RunShouldLogLoggerCreationSuccess()
        {
            const string expectedMessageText = "Logger was created successfully.";
            var bootstrapper = new DefaultAutofacBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsTrue(messages.Contains(expectedMessageText));
        }
        [TestMethod]
        public void RunShouldLogAboutModuleCatalogCreation()
        {
            const string expectedMessageText = "Creating module catalog.";
            var bootstrapper = new DefaultAutofacBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsTrue(messages.Contains(expectedMessageText));
        }

        [TestMethod]
        public void RunShouldLogAboutConfiguringModuleCatalog()
        {
            const string expectedMessageText = "Configuring module catalog.";
            var bootstrapper = new DefaultAutofacBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsTrue(messages.Contains(expectedMessageText));
        }

        [TestMethod]
        public void RunShouldLogAboutCreatingTheContainerBuilder()
        {
            const string expectedMessageText = "Creating Autofac container builder.";
            var bootstrapper = new DefaultAutofacBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsTrue(messages.Contains(expectedMessageText));
        }

        [TestMethod]
        public void RunShouldLogAboutCreatingTheContainer()
        {
            const string expectedMessageText = "Creating Autofac container.";
            var bootstrapper = new DefaultAutofacBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsTrue(messages.Contains(expectedMessageText));
        }

        [TestMethod]
        public void RunShouldLogAboutConfiguringContainerBuilder()
        {
            const string expectedMessageText = "Configuring the Autofac container builder.";
            var bootstrapper = new DefaultAutofacBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsTrue(messages.Contains(expectedMessageText));
        }

        [TestMethod]
        public void RunShouldLogAboutConfiguringRegionAdapters()
        {
            const string expectedMessageText = "Configuring region adapters.";
            var bootstrapper = new DefaultAutofacBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsTrue(messages.Contains(expectedMessageText));
        }


        [TestMethod]
        public void RunShouldLogAboutConfiguringRegionBehaviors()
        {
            const string expectedMessageText = "Configuring default region behaviors.";
            var bootstrapper = new DefaultAutofacBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsTrue(messages.Contains(expectedMessageText));
        }

        [TestMethod]
        public void RunShouldLogAboutRegisteringFrameworkExceptionTypes()
        {
            const string expectedMessageText = "Registering Framework Exception Types.";
            var bootstrapper = new DefaultAutofacBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsTrue(messages.Contains(expectedMessageText));
        }

        [TestMethod]
        public void RunShouldLogAboutCreatingTheShell()
        {
            const string expectedMessageText = "Creating the shell.";
            var bootstrapper = new DefaultAutofacBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsTrue(messages.Contains(expectedMessageText));
        }

        [TestMethod]
        public void RunShouldLogAboutInitializingTheShellIfShellCreated()
        {
            const string expectedMessageText = "Initializing the shell.";
            var bootstrapper = new DefaultAutofacBootstrapper();

            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsTrue(messages.Contains(expectedMessageText));
        }

        [TestMethod]
        public void RunShouldNotLogAboutInitializingTheShellIfShellIsNotCreated()
        {
            const string expectedMessageText = "Initializing shell";
            var bootstrapper = new DefaultAutofacBootstrapper { ShellObject = null };

            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsFalse(messages.Contains(expectedMessageText));
        }

        [TestMethod]
        public void RunShouldLogAboutInitializingModules()
        {
            const string expectedMessageText = "Initializing modules.";
            var bootstrapper = new DefaultAutofacBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsTrue(messages.Contains(expectedMessageText));
        }

        [TestMethod]
        public void RunShouldLogAboutRunCompleting()
        {
            const string expectedMessageText = "Bootstrapper sequence completed.";
            var bootstrapper = new DefaultAutofacBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsTrue(messages.Contains(expectedMessageText));
        }
    }
}
