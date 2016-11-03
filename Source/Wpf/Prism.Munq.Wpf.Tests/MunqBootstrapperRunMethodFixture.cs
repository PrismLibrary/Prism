using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Practices.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prism.Events;
using Prism.Logging;
using Prism.Modularity;
using Prism.Regions;
using Munq;

namespace Prism.Munq.Wpf.Tests
{
    [TestClass]
    public class MunqBootstrapperRunMethodFixture
    {
        [TestMethod]
        public void CanRunBootstrapper()
        {
            var bootstrapper = new DefaultMunqBootstrapper();
            bootstrapper.Run();
        }

        [TestMethod]
        public void RunShouldNotFailIfReturnedNullShell()
        {
            var bootstrapper = new DefaultMunqBootstrapper { ShellObject = null };
            bootstrapper.Run();
        }

        [TestMethod]
        public void RunConfiguresServiceLocatorProvider()
        {
            var bootstrapper = new DefaultMunqBootstrapper();
            bootstrapper.Run();

            Assert.IsTrue(ServiceLocator.Current is MunqServiceLocatorAdapter);
        }

        [TestMethod]
        public void RunShouldInitializeContainer()
        {
            var bootstrapper = new DefaultMunqBootstrapper();
            var container = bootstrapper.BaseContainer;

            Assert.IsNull(container);

            bootstrapper.Run();

            container = bootstrapper.BaseContainer;

            Assert.IsNotNull(container);
            Assert.IsInstanceOfType(container, typeof(MunqContainerWrapper));
        }

        [TestMethod]
        public void RunAddsCompositionContainerToContainer()
        {
            var bootstrapper = new DefaultMunqBootstrapper();

            var createdContainer = bootstrapper.CallCreateContainer();
            var returnedContainer = createdContainer.Resolve<IMunqContainer>();
            Assert.IsNotNull(returnedContainer);
            Assert.AreEqual(typeof(MunqContainerWrapper), returnedContainer.GetType());
        }

        [TestMethod]
        public void RunAddsDependencyResolverToContainer()
        {
            var bootstrapper = new DefaultMunqBootstrapper();

            var createdContainer = bootstrapper.CallCreateContainer();
            var returnedContainer = createdContainer.Resolve<IDependencyResolver>();
            Assert.IsNotNull(returnedContainer);
            Assert.AreEqual(typeof(MunqContainerWrapper), returnedContainer.GetType());
        }

        [TestMethod]
        public void RunAddsDependencyRegistrarToContainer()
        {
            var bootstrapper = new DefaultMunqBootstrapper();

            var createdContainer = bootstrapper.CallCreateContainer();
            var returnedContainer = createdContainer.Resolve<IDependecyRegistrar>();
            Assert.IsNotNull(returnedContainer);
            Assert.AreEqual(typeof(MunqContainerWrapper), returnedContainer.GetType());
        }

        [TestMethod]
        public void RunShouldCallInitializeModules()
        {
            var bootstrapper = new DefaultMunqBootstrapper();
            bootstrapper.Run();

            Assert.IsTrue(bootstrapper.InitializeModulesCalled);
        }

        [TestMethod]
        public void RunShouldCallConfigureDefaultRegionBehaviors()
        {
            var bootstrapper = new DefaultMunqBootstrapper();

            bootstrapper.Run();

            Assert.IsTrue(bootstrapper.ConfigureDefaultRegionBehaviorsCalled);
        }

        [TestMethod]
        public void RunShouldCallConfigureRegionAdapterMappings()
        {
            var bootstrapper = new DefaultMunqBootstrapper();

            bootstrapper.Run();

            Assert.IsTrue(bootstrapper.ConfigureRegionAdapterMappingsCalled);
        }

        [TestMethod]
        public void RunShouldAssignRegionManagerToReturnedShell()
        {
            var bootstrapper = new DefaultMunqBootstrapper();

            bootstrapper.Run();

            Assert.IsNotNull(RegionManager.GetRegionManager(bootstrapper.BaseShell));
        }

        [TestMethod]
        public void RunShouldCallCreateLogger()
        {
            var bootstrapper = new DefaultMunqBootstrapper();

            bootstrapper.Run();

            Assert.IsTrue(bootstrapper.CreateLoggerCalled);
        }

        [TestMethod]
        public void RunShouldCallCreateModuleCatalog()
        {
            var bootstrapper = new DefaultMunqBootstrapper();

            bootstrapper.Run();

            Assert.IsTrue(bootstrapper.CreateModuleCatalogCalled);
        }

        [TestMethod]
        public void RunShouldCallConfigureModuleCatalog()
        {
            var bootstrapper = new DefaultMunqBootstrapper();

            bootstrapper.Run();

            Assert.IsTrue(bootstrapper.ConfigureModuleCatalogCalled);
        }

        [TestMethod]
        public void RunShouldCallCreateContainer()
        {
            var bootstrapper = new DefaultMunqBootstrapper();

            bootstrapper.Run();

            Assert.IsTrue(bootstrapper.CreateContainerCalled);
        }

        [TestMethod]
        public void RunShouldCallCreateShell()
        {
            var bootstrapper = new DefaultMunqBootstrapper();

            bootstrapper.Run();

            Assert.IsTrue(bootstrapper.CreateShellCalled);
        }

        [TestMethod]
        public void RunShouldCallConfigureContainer()
        {
            var bootstrapper = new DefaultMunqBootstrapper();

            bootstrapper.Run();

            Assert.IsTrue(bootstrapper.ConfigureContainerCalled);
        }

        [TestMethod]
        public void RunShouldCallConfigureServiceLocator()
        {
            var bootstrapper = new DefaultMunqBootstrapper();

            bootstrapper.Run();

            Assert.IsTrue(bootstrapper.ConfigureServiceLocatorCalled);
        }

        [TestMethod]
        public void RunShouldCallConfigureViewModelLocator()
        {
            var bootstrapper = new DefaultMunqBootstrapper();

            bootstrapper.Run();

            Assert.IsTrue(bootstrapper.ConfigureViewModelLocatorCalled);
        }

        [TestMethod]
        public void RunRegistersInstanceOfILoggerFacade()
        {
            var mockedContainer = new Mock<IMunqContainer>();
            SetupMockedContainerForVerificationTests(mockedContainer);

            var bootstrapper = new MockedContainerBootstrapper(mockedContainer.Object);

            bootstrapper.Run();

            mockedContainer.Verify(c => c.RegisterInstance(bootstrapper.BaseLogger), Times.Once());
        }

        [TestMethod]
        public void RunRegistersInstanceOfIModuleCatalog()
        {
            var mockedContainer = new Mock<IMunqContainer>();
            SetupMockedContainerForVerificationTests(mockedContainer);

            var bootstrapper = new MockedContainerBootstrapper(mockedContainer.Object);

            bootstrapper.Run();

            mockedContainer.Verify(c => c.RegisterInstance(It.IsAny<IModuleCatalog>()), Times.Once());
        }

        [TestMethod]
        public void RunRegistersTypeForIServiceLocator()
        {
            var mockedContainer = new Mock<IMunqContainer>();
            SetupMockedContainerForVerificationTests(mockedContainer);

            var bootstrapper = new MockedContainerBootstrapper(mockedContainer.Object);

            bootstrapper.Run();

            mockedContainer.Verify(c => c.Register<IServiceLocator>(It.IsAny<Func<IDependencyResolver, IServiceLocator>>()), Times.Once());
        }

        [TestMethod]
        public void RunRegistersTypeForIModuleInitializer()
        {
            var mockedContainer = new Mock<IMunqContainer>();
            SetupMockedContainerForVerificationTests(mockedContainer);

            var bootstrapper = new MockedContainerBootstrapper(mockedContainer.Object);

            bootstrapper.Run();

            mockedContainer.Verify(c => c.Register(It.IsAny<Func<IDependencyResolver, IModuleInitializer>>()), Times.Once());
        }

        [TestMethod]
        public void RunRegistersTypeForIRegionManager()
        {
            var mockedContainer = new Mock<IMunqContainer>();
            SetupMockedContainerForVerificationTests(mockedContainer);

            var bootstrapper = new MockedContainerBootstrapper(mockedContainer.Object);

            bootstrapper.Run();

            mockedContainer.Verify(c => c.Register(It.IsAny<Func<IDependencyResolver, IRegionManager>>()), Times.Once());
        }

        [TestMethod]
        public void RunRegistersTypeForRegionAdapterMappings()
        {
            var mockedContainer = new Mock<IMunqContainer>();
            SetupMockedContainerForVerificationTests(mockedContainer);

            var bootstrapper = new MockedContainerBootstrapper(mockedContainer.Object);

            bootstrapper.Run();

            mockedContainer.Verify(c => c.Register(It.IsAny<Func<IDependencyResolver, RegionAdapterMappings>>()), Times.Once());
        }

        [TestMethod]
        public void RunRegistersTypeForIRegionViewRegistry()
        {
            var mockedContainer = new Mock<IMunqContainer>();
            SetupMockedContainerForVerificationTests(mockedContainer);

            var bootstrapper = new MockedContainerBootstrapper(mockedContainer.Object);

            bootstrapper.Run();

            mockedContainer.Verify(c => c.Register(It.IsAny<Func<IDependencyResolver, IRegionViewRegistry>>()), Times.Once());
        }

        [TestMethod]
        public void RunRegistersTypeForIRegionBehaviorFactory()
        {
            var mockedContainer = new Mock<IMunqContainer>();
            SetupMockedContainerForVerificationTests(mockedContainer);

            var bootstrapper = new MockedContainerBootstrapper(mockedContainer.Object);

            bootstrapper.Run();

            mockedContainer.Verify(c => c.Register(It.IsAny<Func<IDependencyResolver, IRegionBehaviorFactory>>()), Times.Once());
        }

        [TestMethod]
        public void RunRegistersTypeForIEventAggregator()
        {
            var mockedContainer = new Mock<IMunqContainer>();
            SetupMockedContainerForVerificationTests(mockedContainer);

            var bootstrapper = new MockedContainerBootstrapper(mockedContainer.Object);

            bootstrapper.Run();

            mockedContainer.Verify(c => c.Register(It.IsAny<Func<IDependencyResolver, IEventAggregator>>()), Times.Once());
        }

        [TestMethod]
        public void RunFalseShouldNotRegisterDefaultServicesAndTypes()
        {
            var mockedContainer = new Mock<IMunqContainer>();
            SetupMockedContainerForVerificationTests(mockedContainer);

            var bootstrapper = new MockedContainerBootstrapper(mockedContainer.Object);
            bootstrapper.Run(false);

            mockedContainer.Verify(c => c.Register(It.IsAny<Func<IDependencyResolver, IEventAggregator>>()), Times.Never());
            mockedContainer.Verify(c => c.Register(It.IsAny<Func<IDependencyResolver, IRegionManager>>()), Times.Never());
            mockedContainer.Verify(c => c.Register(It.IsAny<Func<IDependencyResolver, RegionAdapterMappings>>()), Times.Never());
            mockedContainer.Verify(c => c.Register(It.IsAny<Func<IDependencyResolver, IServiceLocator>>()), Times.Never());
            mockedContainer.Verify(c => c.Register(It.IsAny<Func<IDependencyResolver, IModuleInitializer>>()), Times.Never());
        }

        [TestMethod]
        public void ModuleManagerRunCalled()
        {
            // Have to use a non-mocked container because of IsRegistered<> extension method, Registrations property,and ContainerRegistration
            var container = new MunqContainerWrapper();

            var mockedModuleInitializer = new Mock<IModuleInitializer>();
            var mockedModuleManager = new Mock<IModuleManager>();
            var regionAdapterMappings = new RegionAdapterMappings();
            var serviceLocatorAdapter = new MunqServiceLocatorAdapter(container);
            var regionBehaviorFactory = new RegionBehaviorFactory(serviceLocatorAdapter);


            container.RegisterInstance<IServiceLocator>(serviceLocatorAdapter);
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
            var bootstrapper = new DefaultMunqBootstrapper();
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
            var bootstrapper = new DefaultMunqBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsTrue(messages[0].Contains("Logger was created successfully."));
            Assert.IsTrue(messages[1].Contains("Creating module catalog."));
            Assert.IsTrue(messages[2].Contains("Configuring module catalog."));
            Assert.IsTrue(messages[3].Contains("Creating Munq container."));
            Assert.IsTrue(messages[4].Contains("Configuring the Munq container."));
            Assert.IsTrue(messages[5].Contains("Configuring ServiceLocator singleton."));
            Assert.IsTrue(messages[6].Contains("Configuring the ViewModelLocator to use Munq."));
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
            var bootstrapper = new DefaultMunqBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsTrue(messages.Contains(expectedMessageText));
        }
        [TestMethod]
        public void RunShouldLogAboutModuleCatalogCreation()
        {
            const string expectedMessageText = "Creating module catalog.";
            var bootstrapper = new DefaultMunqBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsTrue(messages.Contains(expectedMessageText));
        }

        [TestMethod]
        public void RunShouldLogAboutConfiguringModuleCatalog()
        {
            const string expectedMessageText = "Configuring module catalog.";
            var bootstrapper = new DefaultMunqBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsTrue(messages.Contains(expectedMessageText));
        }

        [TestMethod]
        public void RunShouldLogAboutCreatingTheContainer()
        {
            const string expectedMessageText = "Creating Munq container.";
            var bootstrapper = new DefaultMunqBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsTrue(messages.Contains(expectedMessageText));
        }

        [TestMethod]
        public void RunShouldLogAboutConfiguringContainer()
        {
            const string expectedMessageText = "Configuring the Munq container.";
            var bootstrapper = new DefaultMunqBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsTrue(messages.Contains(expectedMessageText));
        }

        [TestMethod]
        public void RunShouldLogAboutConfiguringViewModelLocator()
        {
            const string expectedMessageText = "Configuring the ViewModelLocator to use Munq.";
            var bootstrapper = new DefaultMunqBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsTrue(messages.Contains(expectedMessageText));
        }

        [TestMethod]
        public void RunShouldLogAboutConfiguringRegionAdapters()
        {
            const string expectedMessageText = "Configuring region adapters.";
            var bootstrapper = new DefaultMunqBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsTrue(messages.Contains(expectedMessageText));
        }

        [TestMethod]
        public void RunShouldLogAboutConfiguringRegionBehaviors()
        {
            const string expectedMessageText = "Configuring default region behaviors.";
            var bootstrapper = new DefaultMunqBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsTrue(messages.Contains(expectedMessageText));
        }

        [TestMethod]
        public void RunShouldLogAboutRegisteringFrameworkExceptionTypes()
        {
            const string expectedMessageText = "Registering Framework Exception Types.";
            var bootstrapper = new DefaultMunqBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsTrue(messages.Contains(expectedMessageText));
        }

        [TestMethod]
        public void RunShouldLogAboutCreatingTheShell()
        {
            const string expectedMessageText = "Creating the shell.";
            var bootstrapper = new DefaultMunqBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsTrue(messages.Contains(expectedMessageText));
        }

        [TestMethod]
        public void RunShouldLogAboutInitializingTheShellIfShellCreated()
        {
            const string expectedMessageText = "Initializing the shell.";
            var bootstrapper = new DefaultMunqBootstrapper();

            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsTrue(messages.Contains(expectedMessageText));
        }

        [TestMethod]
        public void RunShouldNotLogAboutInitializingTheShellIfShellIsNotCreated()
        {
            const string expectedMessageText = "Initializing shell";
            var bootstrapper = new DefaultMunqBootstrapper { ShellObject = null };

            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsFalse(messages.Contains(expectedMessageText));
        }

        [TestMethod]
        public void RunShouldLogAboutInitializingModules()
        {
            const string expectedMessageText = "Initializing modules.";
            var bootstrapper = new DefaultMunqBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsTrue(messages.Contains(expectedMessageText));
        }

        [TestMethod]
        public void RunShouldLogAboutRunCompleting()
        {
            const string expectedMessageText = "Bootstrapper sequence completed.";
            var bootstrapper = new DefaultMunqBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.IsTrue(messages.Contains(expectedMessageText));
        }

        private static void SetupMockedContainerForVerificationTests(Mock<IMunqContainer> mockedContainer)
        {
            var mockedModuleInitializer = new Mock<IModuleInitializer>();
            var mockedModuleManager = new Mock<IModuleManager>();
            var regionAdapterMappings = new RegionAdapterMappings();
            var serviceLocatorAdapter = new MunqServiceLocatorAdapter(mockedContainer.Object);
            var regionBehaviorFactory = new RegionBehaviorFactory(serviceLocatorAdapter);

            mockedContainer
                .Setup(c => c.Resolve<IServiceLocator>())
                .Returns(serviceLocatorAdapter);

            mockedContainer.Setup(c => c.RegisterInstance(It.IsAny<string>(), It.IsAny<Type>(), It.IsAny<object>()));

            mockedContainer
                .Setup(c => c.Resolve<IModuleCatalog>())
                .Returns(new ModuleCatalog());

            mockedContainer
                .Setup(c => c.Resolve<IModuleInitializer>())
                .Returns(mockedModuleInitializer.Object);

            mockedContainer
                .Setup(c => c.Resolve<IModuleManager>())
                .Returns(mockedModuleManager.Object);

            mockedContainer
                .Setup(c => c.Resolve<RegionAdapterMappings>())
                .Returns(regionAdapterMappings);

            mockedContainer
                .Setup(c => c.Resolve<SelectorRegionAdapter>())
                .Returns(new SelectorRegionAdapter(regionBehaviorFactory));

            mockedContainer.Setup(c => c.Resolve<ItemsControlRegionAdapter>())
                .Returns(new ItemsControlRegionAdapter(regionBehaviorFactory));

            mockedContainer.Setup(c => c.Resolve<ContentControlRegionAdapter>())
                .Returns(new ContentControlRegionAdapter(regionBehaviorFactory));
        }

        private class MockedContainerBootstrapper : MunqBootstrapper
        {
            private readonly IMunqContainer container;

            public ILoggerFacade BaseLogger
            { get { return base.Logger; } }

            public void CallConfigureContainer()
            {
                base.ConfigureContainer();
            }

            public MockedContainerBootstrapper(IMunqContainer container)
            {
                this.container = container;
            }

            protected override IMunqContainer CreateContainer()
            {
                return this.container;
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
