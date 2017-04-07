

using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prism.Events;
using Prism.Logging;
using Prism.Modularity;
using Prism.Regions;

namespace Prism.Unity.Wpf.Tests
{
    [TestClass]
    public class UnityBootstrapperRunMethodFixture
    {
        [TestMethod]
        public void CanRunBootstrapper()
        {
            var bootstrapper = new DefaultUnityBootstrapper();
            bootstrapper.Run();
        }

        [TestMethod]
        public void RunShouldNotFailIfReturnedNullShell()
        {
            var bootstrapper = new DefaultUnityBootstrapper { ShellObject = null };
            bootstrapper.Run();
        }

        [TestMethod]
        public void RunConfiguresServiceLocatorProvider()
        {
            var bootstrapper = new DefaultUnityBootstrapper();
            bootstrapper.Run();

            Assert.IsTrue(Microsoft.Practices.ServiceLocation.ServiceLocator.Current is UnityServiceLocatorAdapter);
        }

        [TestMethod]
        public void RunShouldInitializeContainer()
        {
            var bootstrapper = new DefaultUnityBootstrapper();
            var container = bootstrapper.BaseContainer;

            Assert.IsNull(container);

            bootstrapper.Run();

            container = bootstrapper.BaseContainer;

            Assert.IsNotNull(container);
            Assert.IsInstanceOfType(container, typeof(UnityContainer));
        }

        [TestMethod]
        public void RunAddsCompositionContainerToContainer()
        {
            var bootstrapper = new DefaultUnityBootstrapper();

            var createdContainer = bootstrapper.CallCreateContainer();
            var returnedContainer = createdContainer.Resolve<IUnityContainer>();
            Assert.IsNotNull(returnedContainer);
            Assert.AreEqual(typeof(UnityContainer), returnedContainer.GetType());
        }

        [TestMethod]
        public void RunShouldCallInitializeModules()
        {
            var bootstrapper = new DefaultUnityBootstrapper();
            bootstrapper.Run();

            Assert.IsTrue(bootstrapper.InitializeModulesCalled);
        }

        [TestMethod]
        public void RunShouldCallConfigureDefaultRegionBehaviors()
        {
            var bootstrapper = new DefaultUnityBootstrapper();

            bootstrapper.Run();

            Assert.IsTrue(bootstrapper.ConfigureDefaultRegionBehaviorsCalled);
        }

        [TestMethod]
        public void RunShouldCallConfigureRegionAdapterMappings()
        {
            var bootstrapper = new DefaultUnityBootstrapper();

            bootstrapper.Run();

            Assert.IsTrue(bootstrapper.ConfigureRegionAdapterMappingsCalled);
        }

        [TestMethod]
        public void RunShouldAssignRegionManagerToReturnedShell()
        {
            var bootstrapper = new DefaultUnityBootstrapper();

            bootstrapper.Run();

            Assert.IsNotNull(RegionManager.GetRegionManager(bootstrapper.BaseShell));
        }

        [TestMethod]
        public void RunShouldCallCreateLogger()
        {
            var bootstrapper = new DefaultUnityBootstrapper();

            bootstrapper.Run();

            Assert.IsTrue(bootstrapper.CreateLoggerCalled);
        }

        [TestMethod]
        public void RunShouldCallCreateModuleCatalog()
        {
            var bootstrapper = new DefaultUnityBootstrapper();

            bootstrapper.Run();

            Assert.IsTrue(bootstrapper.CreateModuleCatalogCalled);
        }

        [TestMethod]
        public void RunShouldCallConfigureModuleCatalog()
        {
            var bootstrapper = new DefaultUnityBootstrapper();

            bootstrapper.Run();

            Assert.IsTrue(bootstrapper.ConfigureModuleCatalogCalled);
        }

        [TestMethod]
        public void RunShouldCallCreateContainer()
        {
            var bootstrapper = new DefaultUnityBootstrapper();

            bootstrapper.Run();

            Assert.IsTrue(bootstrapper.CreateContainerCalled);
        }

        [TestMethod]
        public void RunShouldCallCreateShell()
        {
            var bootstrapper = new DefaultUnityBootstrapper();

            bootstrapper.Run();

            Assert.IsTrue(bootstrapper.CreateShellCalled);
        }

        [TestMethod]
        public void RunShouldCallConfigureContainer()
        {
            var bootstrapper = new DefaultUnityBootstrapper();

            bootstrapper.Run();

            Assert.IsTrue(bootstrapper.ConfigureContainerCalled);
        }

        [TestMethod]
        public void RunShouldCallConfigureServiceLocator()
        {
            var bootstrapper = new DefaultUnityBootstrapper();

            bootstrapper.Run();

            Assert.IsTrue(bootstrapper.ConfigureServiceLocatorCalled);
        }

        [TestMethod]
        public void RunShouldCallConfigureViewModelLocator()
        {
            var bootstrapper = new DefaultUnityBootstrapper();

            bootstrapper.Run();

            Assert.IsTrue(bootstrapper.ConfigureViewModelLocatorCalled);
        }

        [TestMethod]
        public void RunRegistersInstanceOfILoggerFacade()
        {
            var mockedContainer = new Mock<IUnityContainer>();
            SetupMockedContainerForVerificationTests(mockedContainer);


            var bootstrapper = new MockedContainerBootstrapper(mockedContainer.Object);

            bootstrapper.Run();

            mockedContainer.Verify(c => c.RegisterInstance(typeof(ILoggerFacade), null, bootstrapper.BaseLogger, It.IsAny<LifetimeManager>()), Times.Once());
        }

        [TestMethod]
        public void RunRegistersInstanceOfIModuleCatalog()
        {
            var mockedContainer = new Mock<IUnityContainer>();
            SetupMockedContainerForVerificationTests(mockedContainer);

            var bootstrapper = new MockedContainerBootstrapper(mockedContainer.Object);

            bootstrapper.Run();

            mockedContainer.Verify(c => c.RegisterInstance(typeof(IModuleCatalog), null, It.IsAny<object>(), It.IsAny<LifetimeManager>()), Times.Once());
        }

        [TestMethod]
        public void RunRegistersTypeForIServiceLocator()
        {
            var mockedContainer = new Mock<IUnityContainer>();
            SetupMockedContainerForVerificationTests(mockedContainer);

            var bootstrapper = new MockedContainerBootstrapper(mockedContainer.Object);

            bootstrapper.Run();

            mockedContainer.Verify(c => c.RegisterType(typeof(IServiceLocator), typeof(UnityServiceLocatorAdapter), null, It.IsAny<LifetimeManager>()), Times.Once());
        }

        [TestMethod]
        public void RunRegistersTypeForIModuleInitializer()
        {
            var mockedContainer = new Mock<IUnityContainer>();
            SetupMockedContainerForVerificationTests(mockedContainer);

            var bootstrapper = new MockedContainerBootstrapper(mockedContainer.Object);

            bootstrapper.Run();

            mockedContainer.Verify(c => c.RegisterType(typeof(IModuleInitializer), It.IsAny<Type>(), null, It.IsAny<LifetimeManager>()), Times.Once());
        }

        [TestMethod]
        public void RunRegistersTypeForIRegionManager()
        {
            var mockedContainer = new Mock<IUnityContainer>();
            SetupMockedContainerForVerificationTests(mockedContainer);

            var bootstrapper = new MockedContainerBootstrapper(mockedContainer.Object);

            bootstrapper.Run();

            mockedContainer.Verify(c => c.RegisterType(typeof(IRegionManager), It.IsAny<Type>(), null, It.IsAny<LifetimeManager>()), Times.Once());
        }

        [TestMethod]
        public void RunRegistersTypeForRegionAdapterMappings()
        {
            var mockedContainer = new Mock<IUnityContainer>();
            SetupMockedContainerForVerificationTests(mockedContainer);

            var bootstrapper = new MockedContainerBootstrapper(mockedContainer.Object);

            bootstrapper.Run();

            mockedContainer.Verify(c => c.RegisterType(typeof(RegionAdapterMappings), It.IsAny<Type>(), null, It.IsAny<LifetimeManager>()), Times.Once());
        }

        [TestMethod]
        public void RunRegistersTypeForIRegionViewRegistry()
        {
            var mockedContainer = new Mock<IUnityContainer>();
            SetupMockedContainerForVerificationTests(mockedContainer);

            var bootstrapper = new MockedContainerBootstrapper(mockedContainer.Object);

            bootstrapper.Run();

            mockedContainer.Verify(c => c.RegisterType(typeof(IRegionViewRegistry), It.IsAny<Type>(), null, It.IsAny<LifetimeManager>()), Times.Once());
        }

        [TestMethod]
        public void RunRegistersTypeForIRegionBehaviorFactory()
        {
            var mockedContainer = new Mock<IUnityContainer>();
            SetupMockedContainerForVerificationTests(mockedContainer);

            var bootstrapper = new MockedContainerBootstrapper(mockedContainer.Object);

            bootstrapper.Run();

            mockedContainer.Verify(c => c.RegisterType(typeof(IRegionBehaviorFactory), It.IsAny<Type>(), null, It.IsAny<LifetimeManager>()), Times.Once());
        }

        [TestMethod]
        public void RunRegistersTypeForIEventAggregator()
        {
            var mockedContainer = new Mock<IUnityContainer>();
            SetupMockedContainerForVerificationTests(mockedContainer);

            var bootstrapper = new MockedContainerBootstrapper(mockedContainer.Object);

            bootstrapper.Run();

            mockedContainer.Verify(c => c.RegisterType(typeof(IEventAggregator), It.IsAny<Type>(), null, It.IsAny<LifetimeManager>()), Times.Once());
        }

        [TestMethod]
        public void RunFalseShouldNotRegisterDefaultServicesAndTypes()
        {
            var mockedContainer = new Mock<IUnityContainer>();
            SetupMockedContainerForVerificationTests(mockedContainer);

            var bootstrapper = new MockedContainerBootstrapper(mockedContainer.Object);
            bootstrapper.Run(false);

            mockedContainer.Verify(c => c.RegisterType(typeof(IEventAggregator), It.IsAny<Type>(), null, It.IsAny<LifetimeManager>()), Times.Never());
            mockedContainer.Verify(c => c.RegisterType(typeof(IRegionManager), It.IsAny<Type>(), null, It.IsAny<LifetimeManager>()), Times.Never());
            mockedContainer.Verify(c => c.RegisterType(typeof(RegionAdapterMappings), It.IsAny<Type>(), null, It.IsAny<LifetimeManager>()), Times.Never());
            mockedContainer.Verify(c => c.RegisterType(typeof(IServiceLocator), It.IsAny<Type>(), null, It.IsAny<LifetimeManager>()), Times.Never());
            mockedContainer.Verify(c => c.RegisterType(typeof(IModuleInitializer), It.IsAny<Type>(), null, It.IsAny<LifetimeManager>()), Times.Never());
        }

        [TestMethod]
        public void ModuleManagerRunCalled()
        {
            // Have to use a non-mocked container because of IsRegistered<> extension method, Registrations property,and ContainerRegistration
            var container = new UnityContainer();

            var mockedModuleInitializer = new Mock<IModuleInitializer>();
            var mockedModuleManager = new Mock<IModuleManager>();
            var regionAdapterMappings = new RegionAdapterMappings();
            var serviceLocatorAdapter = new UnityServiceLocatorAdapter(container);
            var regionBehaviorFactory = new RegionBehaviorFactory(serviceLocatorAdapter);


            container.RegisterInstance<IServiceLocator>(serviceLocatorAdapter);
            container.RegisterInstance<UnityBootstrapperExtension>(new UnityBootstrapperExtension());
            container.RegisterInstance<IModuleCatalog>(new ModuleCatalog());
            container.RegisterInstance<IModuleInitializer>(mockedModuleInitializer.Object);
            container.RegisterInstance<IModuleManager>(mockedModuleManager.Object);
            container.RegisterInstance<RegionAdapterMappings>(regionAdapterMappings);

            container.RegisterInstance<SelectorRegionAdapter>(new SelectorRegionAdapter(regionBehaviorFactory));
            container.RegisterInstance<ItemsControlRegionAdapter>(new ItemsControlRegionAdapter(regionBehaviorFactory));
            container.RegisterInstance<ContentControlRegionAdapter>(new ContentControlRegionAdapter(regionBehaviorFactory));

            var bootstrapper = new MockedContainerBootstrapper(container);

            bootstrapper.Run();

            mockedModuleManager.Verify(mm => mm.Run(), Times.Once());
        }

        [TestMethod]
        public void RunShouldCallTheMethodsInOrder()
        {
            var bootstrapper = new DefaultUnityBootstrapper();
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
            var bootstrapper = new DefaultUnityBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsTrue(messages[0].Contains("Logger was created successfully."));
            Assert.IsTrue(messages[1].Contains("Creating module catalog."));
            Assert.IsTrue(messages[2].Contains("Configuring module catalog."));
            Assert.IsTrue(messages[3].Contains("Creating Unity container."));
            Assert.IsTrue(messages[4].Contains("Configuring the Unity container."));
            Assert.IsTrue(messages[5].Contains("Adding UnityBootstrapperExtension to container."));
            Assert.IsTrue(messages[6].Contains("Configuring ServiceLocator singleton."));
            Assert.IsTrue(messages[7].Contains("Configuring the ViewModelLocator to use Unity."));
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
            var bootstrapper = new DefaultUnityBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsTrue(messages.Contains(expectedMessageText));
        }
        [TestMethod]
        public void RunShouldLogAboutModuleCatalogCreation()
        {
            const string expectedMessageText = "Creating module catalog.";
            var bootstrapper = new DefaultUnityBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsTrue(messages.Contains(expectedMessageText));
        }

        [TestMethod]
        public void RunShouldLogAboutConfiguringModuleCatalog()
        {
            const string expectedMessageText = "Configuring module catalog.";
            var bootstrapper = new DefaultUnityBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsTrue(messages.Contains(expectedMessageText));
        }

        [TestMethod]
        public void RunShouldLogAboutCreatingTheContainer()
        {
            const string expectedMessageText = "Creating Unity container.";
            var bootstrapper = new DefaultUnityBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsTrue(messages.Contains(expectedMessageText));
        }

        [TestMethod]
        public void RunShouldLogAboutConfiguringContainer()
        {
            const string expectedMessageText = "Configuring the Unity container.";
            var bootstrapper = new DefaultUnityBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsTrue(messages.Contains(expectedMessageText));
        }

        [TestMethod]
        public void RunShouldLogAboutConfiguringViewModelLocator()
        {
            const string expectedMessageText = "Configuring the ViewModelLocator to use Unity.";
            var bootstrapper = new DefaultUnityBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsTrue(messages.Contains(expectedMessageText));
        }

        [TestMethod]
        public void RunShouldLogAboutConfiguringRegionAdapters()
        {
            const string expectedMessageText = "Configuring region adapters.";
            var bootstrapper = new DefaultUnityBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsTrue(messages.Contains(expectedMessageText));
        }

        [TestMethod]
        public void RunShouldLogAboutConfiguringRegionBehaviors()
        {
            const string expectedMessageText = "Configuring default region behaviors.";
            var bootstrapper = new DefaultUnityBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsTrue(messages.Contains(expectedMessageText));
        }

        [TestMethod]
        public void RunShouldLogAboutRegisteringFrameworkExceptionTypes()
        {
            const string expectedMessageText = "Registering Framework Exception Types.";
            var bootstrapper = new DefaultUnityBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsTrue(messages.Contains(expectedMessageText));
        }

        [TestMethod]
        public void RunShouldLogAboutCreatingTheShell()
        {
            const string expectedMessageText = "Creating the shell.";
            var bootstrapper = new DefaultUnityBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsTrue(messages.Contains(expectedMessageText));
        }

        [TestMethod]
        public void RunShouldLogAboutInitializingTheShellIfShellCreated()
        {
            const string expectedMessageText = "Initializing the shell.";
            var bootstrapper = new DefaultUnityBootstrapper();

            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsTrue(messages.Contains(expectedMessageText));
        }

        [TestMethod]
        public void RunShouldNotLogAboutInitializingTheShellIfShellIsNotCreated()
        {
            const string expectedMessageText = "Initializing shell";
            var bootstrapper = new DefaultUnityBootstrapper { ShellObject = null };

            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsFalse(messages.Contains(expectedMessageText));
        }

        [TestMethod]
        public void RunShouldLogAboutInitializingModules()
        {
            const string expectedMessageText = "Initializing modules.";
            var bootstrapper = new DefaultUnityBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsTrue(messages.Contains(expectedMessageText));
        }

        [TestMethod]
        public void RunShouldLogAboutRunCompleting()
        {
            const string expectedMessageText = "Bootstrapper sequence completed.";
            var bootstrapper = new DefaultUnityBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsTrue(messages.Contains(expectedMessageText));
        }

        private static void SetupMockedContainerForVerificationTests(Mock<IUnityContainer> mockedContainer)
        {
            var mockedModuleInitializer = new Mock<IModuleInitializer>();
            var mockedModuleManager = new Mock<IModuleManager>();
            var regionAdapterMappings = new RegionAdapterMappings();
            var serviceLocatorAdapter = new UnityServiceLocatorAdapter(mockedContainer.Object);
            var regionBehaviorFactory = new RegionBehaviorFactory(serviceLocatorAdapter);

            mockedContainer.Setup(c => c.Resolve(typeof(IServiceLocator), (string)null)).Returns(serviceLocatorAdapter);

            mockedContainer.Setup(c => c.RegisterInstance(It.IsAny<Type>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<LifetimeManager>()));

            mockedContainer.Setup(c => c.Resolve(typeof(UnityBootstrapperExtension), (string)null)).Returns(
                new UnityBootstrapperExtension());

            mockedContainer.Setup(c => c.Resolve(typeof(IModuleCatalog), (string)null)).Returns(
                new ModuleCatalog());

            mockedContainer.Setup(c => c.Resolve(typeof(IModuleInitializer), (string)null)).Returns(
                mockedModuleInitializer.Object);

            mockedContainer.Setup(c => c.Resolve(typeof(IModuleManager), (string)null)).Returns(
                mockedModuleManager.Object);


            mockedContainer.Setup(c => c.Resolve(typeof(RegionAdapterMappings), (string)null)).Returns(
                regionAdapterMappings);

            mockedContainer.Setup(c => c.Resolve(typeof(SelectorRegionAdapter), (string)null)).Returns(
                new SelectorRegionAdapter(regionBehaviorFactory));

            mockedContainer.Setup(c => c.Resolve(typeof(ItemsControlRegionAdapter), (string)null)).Returns(
                new ItemsControlRegionAdapter(regionBehaviorFactory));

            mockedContainer.Setup(c => c.Resolve(typeof(ContentControlRegionAdapter), (string)null)).Returns(
                new ContentControlRegionAdapter(regionBehaviorFactory));
        }

        private class MockedContainerBootstrapper : UnityBootstrapper
        {
            private readonly IUnityContainer container;
            public ILoggerFacade BaseLogger
            { get { return base.Logger; } }

            public void CallConfigureContainer()
            {
                base.ConfigureContainer();
            }

            public MockedContainerBootstrapper(IUnityContainer container)
            {
                this.container = container;
            }

            protected override IUnityContainer CreateContainer()
            {
                return container;
            }

            protected override DependencyObject CreateShell()
            {
                return new UserControl();
            }

            protected override void InitializeShell()
            {
                // no op
            }
        }

    }
}
