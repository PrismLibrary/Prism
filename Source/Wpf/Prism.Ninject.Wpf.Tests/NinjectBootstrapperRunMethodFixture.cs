using CommonServiceLocator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using Prism.Ninject.Wpf.Tests.Mocks;
using Prism.Regions;

namespace Prism.Ninject.Wpf.Tests
{
    [TestClass]
    public class NinjectBootstrapperRunMethodFixture
    {
        [TestMethod]
        public void CanRunBootstrapper()
        {
            var bootstrapper = new DefaultNinjectBootstrapper();
            bootstrapper.Run();
        }

        [TestMethod]
        public void RunShouldNotFailIfReturnedNullShell()
        {
            var bootstrapper = new DefaultNinjectBootstrapper {ShellObject = null};
            bootstrapper.Run();
        }

        [TestMethod]
        public void RunConfiguresServiceLocatorProvider()
        {
            var bootstrapper = new DefaultNinjectBootstrapper();
            bootstrapper.Run();

            Assert.IsTrue(ServiceLocator.Current is NinjectServiceLocatorAdapter);
        }

        [TestMethod]
        public void RunShouldInitializeKernel()
        {
            var bootstrapper = new DefaultNinjectBootstrapper();
            var kernel = bootstrapper.Kernel;

            Assert.IsNull(kernel);

            bootstrapper.Run();

            kernel = bootstrapper.Kernel;

            Assert.IsNotNull(kernel);
            Assert.IsInstanceOfType(kernel, typeof(IKernel));
        }

        [TestMethod]
        public void RunAddsCompositionKernelToKernel()
        {
            var bootstrapper = new DefaultNinjectBootstrapper();

            var createdKernel = bootstrapper.CallCreateKernel();
            var returnedKernel = createdKernel.Get<IKernel>();
            Assert.IsNotNull(returnedKernel);
            Assert.AreEqual(typeof(StandardKernel), returnedKernel.GetType());
        }

        [TestMethod]
        public void RunShouldCallInitializeModules()
        {
            var bootstrapper = new DefaultNinjectBootstrapper();
            bootstrapper.Run();

            Assert.IsTrue(bootstrapper.InitializeModulesCalled);
        }

        [TestMethod]
        public void RunShouldCallConfigureDefaultRegionBehaviors()
        {
            var bootstrapper = new DefaultNinjectBootstrapper();

            bootstrapper.Run();

            Assert.IsTrue(bootstrapper.ConfigureDefaultRegionBehaviorsCalled);
        }

        [TestMethod]
        public void RunShouldCallConfigureRegionAdapterMappings()
        {
            var bootstrapper = new DefaultNinjectBootstrapper();

            bootstrapper.Run();

            Assert.IsTrue(bootstrapper.ConfigureRegionAdapterMappingsCalled);
        }

        [TestMethod]
        public void RunShouldAssignRegionManagerToReturnedShell()
        {
            var bootstrapper = new DefaultNinjectBootstrapper();

            bootstrapper.Run();

            Assert.IsNotNull(RegionManager.GetRegionManager(bootstrapper.BaseShell));
        }

        [TestMethod]
        public void RunShouldCallCreateLogger()
        {
            var bootstrapper = new DefaultNinjectBootstrapper();

            bootstrapper.Run();

            Assert.IsTrue(bootstrapper.CreateLoggerCalled);
        }

        [TestMethod]
        public void RunShouldCallCreateModuleCatalog()
        {
            var bootstrapper = new DefaultNinjectBootstrapper();

            bootstrapper.Run();

            Assert.IsTrue(bootstrapper.CreateModuleCatalogCalled);
        }

        [TestMethod]
        public void RunShouldCallConfigureModuleCatalog()
        {
            var bootstrapper = new DefaultNinjectBootstrapper();

            bootstrapper.Run();

            Assert.IsTrue(bootstrapper.ConfigureModuleCatalogCalled);
        }

        [TestMethod]
        public void RunShouldCallCreateKernel()
        {
            var bootstrapper = new DefaultNinjectBootstrapper();

            bootstrapper.Run();

            Assert.IsTrue(bootstrapper.CreateKernelCalled);
        }

        [TestMethod]
        public void RunShouldCallCreateShell()
        {
            var bootstrapper = new DefaultNinjectBootstrapper();

            bootstrapper.Run();

            Assert.IsTrue(bootstrapper.CreateShellCalled);
        }

        [TestMethod]
        public void RunShouldCallConfigureKernel()
        {
            var bootstrapper = new DefaultNinjectBootstrapper();

            bootstrapper.Run();

            Assert.IsTrue(bootstrapper.ConfigureKernelCalled);
        }

        [TestMethod]
        public void RunShouldCallConfigureServiceLocator()
        {
            var bootstrapper = new DefaultNinjectBootstrapper();

            bootstrapper.Run();

            Assert.IsTrue(bootstrapper.ConfigureServiceLocatorCalled);
        }

        [TestMethod]
        public void RunShouldCallConfigureViewModelLocator()
        {
            var bootstrapper = new DefaultNinjectBootstrapper();

            bootstrapper.Run();

            Assert.IsTrue(bootstrapper.ConfigureViewModelLocatorCalled);
        }

        [TestMethod]
        public void RunShouldCallTheMethodsInOrder()
        {
            var bootstrapper = new DefaultNinjectBootstrapper();
            bootstrapper.Run();

            Assert.AreEqual("CreateLogger", bootstrapper.MethodCalls[0]);
            Assert.AreEqual("CreateModuleCatalog", bootstrapper.MethodCalls[1]);
            Assert.AreEqual("ConfigureModuleCatalog", bootstrapper.MethodCalls[2]);
            Assert.AreEqual("CreateKernel", bootstrapper.MethodCalls[3]);
            Assert.AreEqual("ConfigureKernel", bootstrapper.MethodCalls[4]);
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
            var bootstrapper = new DefaultNinjectBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsTrue(messages[0].Contains("Logger was created successfully."));
            Assert.IsTrue(messages[1].Contains("Creating module catalog."));
            Assert.IsTrue(messages[2].Contains("Configuring module catalog."));
            Assert.IsTrue(messages[3].Contains("Creating the Ninject kernel."));
            Assert.IsTrue(messages[4].Contains("Configuring the Ninject kernel."));
            Assert.IsTrue(messages[5].Contains("Configuring ServiceLocator singleton."));
            Assert.IsTrue(messages[6].Contains("Configuring the ViewModelLocator to use Ninject."));
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
            var bootstrapper = new DefaultNinjectBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsTrue(messages.Contains(expectedMessageText));
        }

        [TestMethod]
        public void RunShouldLogAboutModuleCatalogCreation()
        {
            const string expectedMessageText = "Creating module catalog.";
            var bootstrapper = new DefaultNinjectBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsTrue(messages.Contains(expectedMessageText));
        }

        [TestMethod]
        public void RunShouldLogAboutConfiguringModuleCatalog()
        {
            const string expectedMessageText = "Configuring module catalog.";
            var bootstrapper = new DefaultNinjectBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsTrue(messages.Contains(expectedMessageText));
        }

        [TestMethod]
        public void RunShouldLogAboutCreatingTheKernel()
        {
            const string expectedMessageText = "Creating the Ninject kernel.";
            var bootstrapper = new DefaultNinjectBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsTrue(messages.Contains(expectedMessageText));
        }

        [TestMethod]
        public void RunShouldLogAboutConfiguringKernel()
        {
            const string expectedMessageText = "Configuring the Ninject kernel.";
            var bootstrapper = new DefaultNinjectBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsTrue(messages.Contains(expectedMessageText));
        }

        [TestMethod]
        public void RunShouldLogAboutConfiguringViewModelLocator()
        {
            const string expectedMessageText = "Configuring the ViewModelLocator to use Ninject.";
            var bootstrapper = new DefaultNinjectBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsTrue(messages.Contains(expectedMessageText));
        }

        [TestMethod]
        public void RunShouldLogAboutConfiguringRegionAdapters()
        {
            const string expectedMessageText = "Configuring region adapters.";
            var bootstrapper = new DefaultNinjectBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsTrue(messages.Contains(expectedMessageText));
        }

        [TestMethod]
        public void RunShouldLogAboutConfiguringRegionBehaviors()
        {
            const string expectedMessageText = "Configuring default region behaviors.";
            var bootstrapper = new DefaultNinjectBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsTrue(messages.Contains(expectedMessageText));
        }

        [TestMethod]
        public void RunShouldLogAboutRegisteringFrameworkExceptionTypes()
        {
            const string expectedMessageText = "Registering Framework Exception Types.";
            var bootstrapper = new DefaultNinjectBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsTrue(messages.Contains(expectedMessageText));
        }

        [TestMethod]
        public void RunShouldLogAboutCreatingTheShell()
        {
            const string expectedMessageText = "Creating the shell.";
            var bootstrapper = new DefaultNinjectBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsTrue(messages.Contains(expectedMessageText));
        }

        [TestMethod]
        public void RunShouldLogAboutInitializingTheShellIfShellCreated()
        {
            const string expectedMessageText = "Initializing the shell.";
            var bootstrapper = new DefaultNinjectBootstrapper();

            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsTrue(messages.Contains(expectedMessageText));
        }

        [TestMethod]
        public void RunShouldNotLogAboutInitializingTheShellIfShellIsNotCreated()
        {
            const string expectedMessageText = "Initializing shell";
            var bootstrapper = new DefaultNinjectBootstrapper {ShellObject = null};

            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsFalse(messages.Contains(expectedMessageText));
        }

        [TestMethod]
        public void RunShouldLogAboutInitializingModules()
        {
            const string expectedMessageText = "Initializing modules.";
            var bootstrapper = new DefaultNinjectBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsTrue(messages.Contains(expectedMessageText));
        }

        [TestMethod]
        public void RunShouldLogAboutRunCompleting()
        {
            const string expectedMessageText = "Bootstrapper sequence completed.";
            var bootstrapper = new DefaultNinjectBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsTrue(messages.Contains(expectedMessageText));
        }
    }
}