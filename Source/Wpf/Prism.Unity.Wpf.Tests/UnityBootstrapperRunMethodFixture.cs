

using System;
using System.Windows;
using System.Windows.Controls;
using CommonServiceLocator;
using Unity;
using Xunit;
using Moq;
using Prism.Events;
using Prism.Logging;
using Prism.Modularity;
using Prism.Regions;
using Unity.Lifetime;

namespace Prism.Unity.Wpf.Tests
{
    [Collection("ServiceLocator")]
    public class UnityBootstrapperRunMethodFixture
    {
        [StaFact]
        public void CanRunBootstrapper()
        {
            var bootstrapper = new DefaultUnityBootstrapper();
            bootstrapper.Run();
        }

        [StaFact]
        public void RunShouldNotFailIfReturnedNullShell()
        {
            var bootstrapper = new DefaultUnityBootstrapper { ShellObject = null };
            bootstrapper.Run();
        }

        [StaFact]
        public void RunConfiguresServiceLocatorProvider()
        {
            var bootstrapper = new DefaultUnityBootstrapper();
            bootstrapper.Run();

            Assert.True(CommonServiceLocator.ServiceLocator.Current is UnityServiceLocatorAdapter);
        }

        [StaFact]
        public void RunShouldInitializeContainer()
        {
            var bootstrapper = new DefaultUnityBootstrapper();
            var container = bootstrapper.BaseContainer;

            Assert.Null(container);

            bootstrapper.Run();

            container = bootstrapper.BaseContainer;

            Assert.NotNull(container);
            Assert.IsType<UnityContainer>(container);
        }

        [StaFact]
        public void RunAddsCompositionContainerToContainer()
        {
            var bootstrapper = new DefaultUnityBootstrapper();

            var createdContainer = bootstrapper.CallCreateContainer();
            var returnedContainer = createdContainer.Resolve<IUnityContainer>();
            Assert.NotNull(returnedContainer);
            Assert.Equal(typeof(UnityContainer), returnedContainer.GetType());
        }

        [StaFact]
        public void RunShouldCallInitializeModules()
        {
            var bootstrapper = new DefaultUnityBootstrapper();
            bootstrapper.Run();

            Assert.True(bootstrapper.InitializeModulesCalled);
        }

        [StaFact]
        public void RunShouldCallConfigureDefaultRegionBehaviors()
        {
            var bootstrapper = new DefaultUnityBootstrapper();

            bootstrapper.Run();

            Assert.True(bootstrapper.ConfigureDefaultRegionBehaviorsCalled);
        }

        [StaFact]
        public void RunShouldCallConfigureRegionAdapterMappings()
        {
            var bootstrapper = new DefaultUnityBootstrapper();

            bootstrapper.Run();

            Assert.True(bootstrapper.ConfigureRegionAdapterMappingsCalled);
        }

        [StaFact]
        public void RunShouldAssignRegionManagerToReturnedShell()
        {
            var bootstrapper = new DefaultUnityBootstrapper();

            bootstrapper.Run();

            Assert.NotNull(RegionManager.GetRegionManager(bootstrapper.BaseShell));
        }

        [StaFact]
        public void RunShouldCallCreateLogger()
        {
            var bootstrapper = new DefaultUnityBootstrapper();

            bootstrapper.Run();

            Assert.True(bootstrapper.CreateLoggerCalled);
        }

        [StaFact]
        public void RunShouldCallCreateModuleCatalog()
        {
            var bootstrapper = new DefaultUnityBootstrapper();

            bootstrapper.Run();

            Assert.True(bootstrapper.CreateModuleCatalogCalled);
        }

        [StaFact]
        public void RunShouldCallConfigureModuleCatalog()
        {
            var bootstrapper = new DefaultUnityBootstrapper();

            bootstrapper.Run();

            Assert.True(bootstrapper.ConfigureModuleCatalogCalled);
        }

        [StaFact]
        public void RunShouldCallCreateContainer()
        {
            var bootstrapper = new DefaultUnityBootstrapper();

            bootstrapper.Run();

            Assert.True(bootstrapper.CreateContainerCalled);
        }

        [StaFact]
        public void RunShouldCallCreateShell()
        {
            var bootstrapper = new DefaultUnityBootstrapper();

            bootstrapper.Run();

            Assert.True(bootstrapper.CreateShellCalled);
        }

        [StaFact]
        public void RunShouldCallConfigureContainer()
        {
            var bootstrapper = new DefaultUnityBootstrapper();

            bootstrapper.Run();

            Assert.True(bootstrapper.ConfigureContainerCalled);
        }

        [StaFact]
        public void RunShouldCallConfigureServiceLocator()
        {
            var bootstrapper = new DefaultUnityBootstrapper();

            bootstrapper.Run();

            Assert.True(bootstrapper.ConfigureServiceLocatorCalled);
        }

        [StaFact]
        public void RunShouldCallConfigureViewModelLocator()
        {
            var bootstrapper = new DefaultUnityBootstrapper();

            bootstrapper.Run();

            Assert.True(bootstrapper.ConfigureViewModelLocatorCalled);
        }

        [StaFact]
        public void RunRegistersInstanceOfILoggerFacade()
        {
            var mockedContainer = new Mock<IUnityContainer>();
            SetupMockedContainerForVerificationTests(mockedContainer);


            var bootstrapper = new MockedContainerBootstrapper(mockedContainer.Object);

            bootstrapper.Run();

            mockedContainer.Verify(c => c.RegisterInstance(typeof(ILoggerFacade), null, bootstrapper.BaseLogger, It.IsAny<LifetimeManager>()), Times.Once());
        }

        [StaFact]
        public void RunRegistersInstanceOfIModuleCatalog()
        {
            var mockedContainer = new Mock<IUnityContainer>();
            SetupMockedContainerForVerificationTests(mockedContainer);

            var bootstrapper = new MockedContainerBootstrapper(mockedContainer.Object);

            bootstrapper.Run();

            mockedContainer.Verify(c => c.RegisterInstance(typeof(IModuleCatalog), null, It.IsAny<object>(), It.IsAny<LifetimeManager>()), Times.Once());
        }

        [StaFact]
        public void RunRegistersTypeForIServiceLocator()
        {
            var mockedContainer = new Mock<IUnityContainer>();
            SetupMockedContainerForVerificationTests(mockedContainer);

            var bootstrapper = new MockedContainerBootstrapper(mockedContainer.Object);

            bootstrapper.Run();

            mockedContainer.Verify(c => c.RegisterType(typeof(IServiceLocator), typeof(UnityServiceLocatorAdapter), null, It.IsAny<LifetimeManager>()), Times.Once());
        }

        [StaFact]
        public void RunRegistersTypeForIModuleInitializer()
        {
            var mockedContainer = new Mock<IUnityContainer>();
            SetupMockedContainerForVerificationTests(mockedContainer);

            var bootstrapper = new MockedContainerBootstrapper(mockedContainer.Object);

            bootstrapper.Run();

            mockedContainer.Verify(c => c.RegisterType(typeof(IModuleInitializer), It.IsAny<Type>(), null, It.IsAny<LifetimeManager>()), Times.Once());
        }

        [StaFact]
        public void RunRegistersTypeForIRegionManager()
        {
            var mockedContainer = new Mock<IUnityContainer>();
            SetupMockedContainerForVerificationTests(mockedContainer);

            var bootstrapper = new MockedContainerBootstrapper(mockedContainer.Object);

            bootstrapper.Run();

            mockedContainer.Verify(c => c.RegisterType(typeof(IRegionManager), It.IsAny<Type>(), null, It.IsAny<LifetimeManager>()), Times.Once());
        }

        [StaFact]
        public void RunRegistersTypeForRegionAdapterMappings()
        {
            var mockedContainer = new Mock<IUnityContainer>();
            SetupMockedContainerForVerificationTests(mockedContainer);

            var bootstrapper = new MockedContainerBootstrapper(mockedContainer.Object);

            bootstrapper.Run();

            mockedContainer.Verify(c => c.RegisterType(typeof(RegionAdapterMappings), It.IsAny<Type>(), null, It.IsAny<LifetimeManager>()), Times.Once());
        }

        [StaFact]
        public void RunRegistersTypeForIRegionViewRegistry()
        {
            var mockedContainer = new Mock<IUnityContainer>();
            SetupMockedContainerForVerificationTests(mockedContainer);

            var bootstrapper = new MockedContainerBootstrapper(mockedContainer.Object);

            bootstrapper.Run();

            mockedContainer.Verify(c => c.RegisterType(typeof(IRegionViewRegistry), It.IsAny<Type>(), null, It.IsAny<LifetimeManager>()), Times.Once());
        }

        [StaFact]
        public void RunRegistersTypeForIRegionBehaviorFactory()
        {
            var mockedContainer = new Mock<IUnityContainer>();
            SetupMockedContainerForVerificationTests(mockedContainer);

            var bootstrapper = new MockedContainerBootstrapper(mockedContainer.Object);

            bootstrapper.Run();

            mockedContainer.Verify(c => c.RegisterType(typeof(IRegionBehaviorFactory), It.IsAny<Type>(), null, It.IsAny<LifetimeManager>()), Times.Once());
        }

        [StaFact]
        public void RunRegistersTypeForIEventAggregator()
        {
            var mockedContainer = new Mock<IUnityContainer>();
            SetupMockedContainerForVerificationTests(mockedContainer);

            var bootstrapper = new MockedContainerBootstrapper(mockedContainer.Object);

            bootstrapper.Run();

            mockedContainer.Verify(c => c.RegisterType(typeof(IEventAggregator), It.IsAny<Type>(), null, It.IsAny<LifetimeManager>()), Times.Once());
        }

        [StaFact]
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

        [StaFact]
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
            
            container.RegisterSingleton(typeof(RegionAdapterMappings), typeof(RegionAdapterMappings));
            container.RegisterSingleton(typeof(IRegionManager), typeof(RegionManager));
            container.RegisterSingleton(typeof(IEventAggregator), typeof(EventAggregator));
            container.RegisterSingleton(typeof(IRegionViewRegistry), typeof(RegionViewRegistry));
            container.RegisterSingleton(typeof(IRegionBehaviorFactory), typeof(RegionBehaviorFactory));
            container.RegisterSingleton(typeof(IRegionNavigationJournalEntry), typeof(RegionNavigationJournalEntry));
            container.RegisterSingleton(typeof(IRegionNavigationJournal), typeof(RegionNavigationJournal));
            container.RegisterSingleton(typeof(IRegionNavigationService), typeof(RegionNavigationService));
            container.RegisterSingleton(typeof(IRegionNavigationContentLoader), typeof(Regions.UnityRegionNavigationContentLoader));


            container.RegisterInstance<SelectorRegionAdapter>(new SelectorRegionAdapter(regionBehaviorFactory));
            container.RegisterInstance<ItemsControlRegionAdapter>(new ItemsControlRegionAdapter(regionBehaviorFactory));
            container.RegisterInstance<ContentControlRegionAdapter>(new ContentControlRegionAdapter(regionBehaviorFactory));

            var bootstrapper = new MockedContainerBootstrapper(container);
            bootstrapper.Run(false);

            mockedModuleManager.Verify(mm => mm.Run(), Times.Once());
        }

        [StaFact]
        public void RunShouldCallTheMethodsInOrder()
        {
            var bootstrapper = new DefaultUnityBootstrapper();
            bootstrapper.Run();

            Assert.Equal("CreateLogger", bootstrapper.MethodCalls[0]);
            Assert.Equal("CreateModuleCatalog", bootstrapper.MethodCalls[1]);
            Assert.Equal("ConfigureModuleCatalog", bootstrapper.MethodCalls[2]);
            Assert.Equal("CreateContainer", bootstrapper.MethodCalls[3]);
            Assert.Equal("ConfigureContainer", bootstrapper.MethodCalls[4]);
            Assert.Equal("ConfigureServiceLocator", bootstrapper.MethodCalls[5]);
            Assert.Equal("ConfigureViewModelLocator", bootstrapper.MethodCalls[6]);
            Assert.Equal("ConfigureRegionAdapterMappings", bootstrapper.MethodCalls[7]);
            Assert.Equal("ConfigureDefaultRegionBehaviors", bootstrapper.MethodCalls[8]);
            Assert.Equal("RegisterFrameworkExceptionTypes", bootstrapper.MethodCalls[9]);
            Assert.Equal("CreateShell", bootstrapper.MethodCalls[10]);
            Assert.Equal("InitializeShell", bootstrapper.MethodCalls[11]);
            Assert.Equal("InitializeModules", bootstrapper.MethodCalls[12]);
        }

        [StaFact]
        public void RunShouldLogBootstrapperSteps()
        {
            var bootstrapper = new DefaultUnityBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.Contains("Logger was created successfully.", messages[0]);
            Assert.Contains("Creating module catalog.", messages[1]);
            Assert.Contains("Configuring module catalog.", messages[2]);
            Assert.Contains("Creating Unity container.", messages[3]);
            Assert.Contains("Configuring the Unity container.", messages[4]);
            Assert.Contains("Adding UnityBootstrapperExtension to container.", messages[5]);
            Assert.Contains("Configuring ServiceLocator singleton.", messages[6]);
            Assert.Contains("Configuring the ViewModelLocator to use Unity.", messages[7]);
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

        [StaFact]
        public void RunShouldLogLoggerCreationSuccess()
        {
            const string expectedMessageText = "Logger was created successfully.";
            var bootstrapper = new DefaultUnityBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.True(messages.Contains(expectedMessageText));
        }
        [StaFact]
        public void RunShouldLogAboutModuleCatalogCreation()
        {
            const string expectedMessageText = "Creating module catalog.";
            var bootstrapper = new DefaultUnityBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.True(messages.Contains(expectedMessageText));
        }

        [StaFact]
        public void RunShouldLogAboutConfiguringModuleCatalog()
        {
            const string expectedMessageText = "Configuring module catalog.";
            var bootstrapper = new DefaultUnityBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.True(messages.Contains(expectedMessageText));
        }

        [StaFact]
        public void RunShouldLogAboutCreatingTheContainer()
        {
            const string expectedMessageText = "Creating Unity container.";
            var bootstrapper = new DefaultUnityBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.True(messages.Contains(expectedMessageText));
        }

        [StaFact]
        public void RunShouldLogAboutConfiguringContainer()
        {
            const string expectedMessageText = "Configuring the Unity container.";
            var bootstrapper = new DefaultUnityBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.True(messages.Contains(expectedMessageText));
        }

        [StaFact]
        public void RunShouldLogAboutConfiguringViewModelLocator()
        {
            const string expectedMessageText = "Configuring the ViewModelLocator to use Unity.";
            var bootstrapper = new DefaultUnityBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.True(messages.Contains(expectedMessageText));
        }

        [StaFact]
        public void RunShouldLogAboutConfiguringRegionAdapters()
        {
            const string expectedMessageText = "Configuring region adapters.";
            var bootstrapper = new DefaultUnityBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.True(messages.Contains(expectedMessageText));
        }

        [StaFact]
        public void RunShouldLogAboutConfiguringRegionBehaviors()
        {
            const string expectedMessageText = "Configuring default region behaviors.";
            var bootstrapper = new DefaultUnityBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.True(messages.Contains(expectedMessageText));
        }

        [StaFact]
        public void RunShouldLogAboutRegisteringFrameworkExceptionTypes()
        {
            const string expectedMessageText = "Registering Framework Exception Types.";
            var bootstrapper = new DefaultUnityBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.True(messages.Contains(expectedMessageText));
        }

        [StaFact]
        public void RunShouldLogAboutCreatingTheShell()
        {
            const string expectedMessageText = "Creating the shell.";
            var bootstrapper = new DefaultUnityBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.True(messages.Contains(expectedMessageText));
        }

        [StaFact]
        public void RunShouldLogAboutInitializingTheShellIfShellCreated()
        {
            const string expectedMessageText = "Initializing the shell.";
            var bootstrapper = new DefaultUnityBootstrapper();

            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.True(messages.Contains(expectedMessageText));
        }

        [StaFact]
        public void RunShouldNotLogAboutInitializingTheShellIfShellIsNotCreated()
        {
            const string expectedMessageText = "Initializing shell";
            var bootstrapper = new DefaultUnityBootstrapper { ShellObject = null };

            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.False(messages.Contains(expectedMessageText));
        }

        [StaFact]
        public void RunShouldLogAboutInitializingModules()
        {
            const string expectedMessageText = "Initializing modules.";
            var bootstrapper = new DefaultUnityBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.True(messages.Contains(expectedMessageText));
        }

        [StaFact]
        public void RunShouldLogAboutRunCompleting()
        {
            const string expectedMessageText = "Bootstrapper sequence completed.";
            var bootstrapper = new DefaultUnityBootstrapper();
            bootstrapper.Run();
            var messages = bootstrapper.BaseLogger.Messages;

            Assert.True(messages.Contains(expectedMessageText));
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
